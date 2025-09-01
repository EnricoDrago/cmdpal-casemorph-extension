using CaseMorphExtension.Core;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.System;

namespace CaseMorphExtension;

internal sealed partial class CaseMorphExtensionPage : DynamicListPage
{
    private readonly List<ListItem> _items = [];
    private readonly SettingsManager _settingsManager;
    private readonly Dictionary<string, MethodInfo> _methodsByName;

    public CaseMorphExtensionPage(SettingsManager settingsManager)
    {
        Name = "Case Morph";
        Title = "Case Morph";
        PlaceholderText = "Type your text here...";
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        
        _settingsManager = settingsManager;
        _methodsByName = typeof(TextTransformer)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Select(m => (method: m, attr: m.GetCustomAttribute<TransformationDisplayNameAttribute>()!))
            .ToDictionary(t => t.attr.DisplayName, t => t.method);
    }

    public override IListItem[] GetItems()
    {
        string currentSearchText = string.IsNullOrEmpty(SearchText) ? ClipboardHelper.GetText() : SearchText;
        _items.Clear();

        foreach (var displayName in _settingsManager.GetOrderedEnabledTransformations())
        {
            var attr = _methodsByName[displayName].GetCustomAttribute<TransformationDisplayNameAttribute>()!;
            string? transformedText = _methodsByName[attr.DisplayName].Invoke(null, [currentSearchText]) as string;
            transformedText = string.IsNullOrWhiteSpace(transformedText) ? " " : transformedText;
            ICommand primary = _settingsManager.DefaultOutputCommand == "copy" ? new CopyTextCommand(transformedText) : new TypeCommand(transformedText);
            ICommand secondary = _settingsManager.DefaultOutputCommand == "type" ? new CopyTextCommand(transformedText) : new TypeCommand(transformedText);

            _items.Add(new ListItem(primary)
            {
                Title = transformedText,
                Subtitle = attr.DisplayName,
                Icon = new IconInfo(attr.Glyph),

                MoreCommands = [
                    new CommandContextItem(secondary),
                    new CommandContextItem(
                        title: $"Disable {attr.DisplayName}",
                        name: $"Disable {attr.DisplayName}",
                        action: () => {
                            _settingsManager._toggles[attr.DisplayName].Value = false;
                            _settingsManager.SyncOrderSettingWithEnabledToggles();
                            _settingsManager.SaveSettings();
                            RaiseItemsChanged(1);
                        },
                        result: CommandResult.KeepOpen())
                    {
                        Icon = new IconInfo("\uE711"),
                        IsCritical = true,
                    },

                    new CommandContextItem(_settingsManager.Settings.SettingsPage)
                ],
            });
        }
        return _items.ToArray();
    }

    public override void UpdateSearchText(string oldSearch, string newSearch) => RaiseItemsChanged(_items.Count);
}