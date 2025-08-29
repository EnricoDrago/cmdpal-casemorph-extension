using CaseMorphExtension.Core;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace CaseMorphExtension;

public class SettingsManager : JsonSettingsManager
{
    private static readonly string _namespace = "casemorph";
    private static string Namespaced(string propertyName) => $"{_namespace}.{propertyName}";
    internal readonly Dictionary<string, ToggleSetting> _toggles = new();

    internal static string SettingsJsonPath()
    {
        var directory = Utilities.BaseSettingsPath("CaseMorphExtension");
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, "settings.json");
    }

    private readonly ChoiceSetSetting defaultOutputSetting = new(
        Namespaced("defaultOutput"),
        "Default Output Command",
        "Select the preferred output method",
        [ new ChoiceSetSetting.Choice("Copy", "copy"), new ChoiceSetSetting.Choice("Type", "type") ]
    );

    private readonly TextSetting orderSetting;

    public SettingsManager()
    {
        FilePath = SettingsJsonPath();

        Settings.Add(defaultOutputSetting);

        var methods = typeof(TextTransformer).GetMethods(BindingFlags.Public | BindingFlags.Static);
        foreach (var method in methods)
        {
            var attr = method.GetCustomAttribute<Core.TransformationDisplayNameAttribute>();
            var toggle = new ToggleSetting(
                Namespaced(attr.DisplayName),
                attr.DisplayName,
                "",
                true
            );

            _toggles[attr.DisplayName] = toggle;
            Settings.Add(toggle);
        }
        
        orderSetting = new TextSetting(
            Namespaced("order"),
            "Displayed order of Transformations",
            "Define the order of the enabled transformations. Delete all and click 'Save' to revert to default order",
            string.Join(", ", _toggles.Where(kv => kv.Value.Value).Select(kv => kv.Key).ToList())
        );

        Settings.Add(orderSetting);
        LoadSettings();

        Settings.SettingsChanged += (s, a) => {
            SyncOrderSettingWithEnabledToggles();
            SaveSettings();
        };
    }

    private IEnumerable<string> ComputeOrderedEnabledTransformations()
    {
        var current = (orderSetting.Value ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(name => _toggles.ContainsKey(name) && _toggles[name].Value)
            .Concat(_toggles.Keys.Where(k => _toggles[k].Value && !(orderSetting.Value?.Split(',').Select(s => s.Trim()) ?? Enumerable.Empty<string>()).Contains(k)));

        return current;
    }

    public IReadOnlyList<string> GetOrderedEnabledTransformations() => ComputeOrderedEnabledTransformations().ToList();
    public void SyncOrderSettingWithEnabledToggles() => orderSetting.Value = string.Join(", ", ComputeOrderedEnabledTransformations());
    public string OrderSettingValue => orderSetting.Value ?? string.Join(", ", _toggles.Keys);
    public string DefaultOutputCommand => defaultOutputSetting.Value ?? "copy";
    public bool IsEnabled(string displayName) => _toggles[displayName].Value;
}
