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
    private readonly IReadOnlyList<MethodInfo> _transformationMethods;

    public CaseMorphExtensionPage()
    {
        Name = "Case Morph";
        Title = "Case Morph";
        PlaceholderText = "Type your text here...";
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");

        _transformationMethods = typeof(TextTransformer).GetMethods(BindingFlags.Public | BindingFlags.Static).ToList().AsReadOnly();
    }

    public override IListItem[] GetItems()
    {
        string currentSearchText = string.IsNullOrEmpty(SearchText) ? ClipboardHelper.GetText() : SearchText;

        _items.Clear();

        foreach (var method in _transformationMethods)
        {
            string transformedText = (string)method.Invoke(null, new object[] { currentSearchText });
            _items.Add(new ListItem(new CopyTextCommand(transformedText))
            {
                Title = transformedText,
                Subtitle = method.GetCustomAttribute<TransformationDisplayNameAttribute>().DisplayName,
                Icon = IconHelpers.FromRelativePath("Assets\\Logo.png"),

                MoreCommands = [
                new CommandContextItem(new TypeCommand(transformedText))
                {
                    RequestedShortcut = KeyChordHelpers.FromModifiers(ctrl: true, vkey: VirtualKey.O)
                }],
            });
        }
        return _items.ToArray();
    }

    public override void UpdateSearchText(string oldSearch, string newSearch) => RaiseItemsChanged(_items.Count);
}