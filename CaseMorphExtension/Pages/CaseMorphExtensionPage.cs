using CaseMorphExtension.Core;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CaseMorphExtension;

internal sealed partial class CaseMorphExtensionPage : DynamicListPage
{
    private readonly List<ListItem> _items = [];
    private readonly IReadOnlyList<MethodInfo> _transformationMethods;

    public CaseMorphExtensionPage()
    {
        Title = "Case Morph";
        PlaceholderText = "Type your text here...";
        Name = "Case Morph";
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");

        _transformationMethods = GetTransformationMethods();
    }
    private static IReadOnlyList<MethodInfo> GetTransformationMethods()
    {
        var methods = new List<MethodInfo>();
        Type transformerType = typeof(TextTransformer);
        methods = transformerType.GetMethods(BindingFlags.Public | BindingFlags.Static).ToList();
        return methods.AsReadOnly();
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
            });
        }

        return _items.ToArray();
    }

    public override void UpdateSearchText(string oldSearch, string newSearch) => RaiseItemsChanged(_items.Count);
}