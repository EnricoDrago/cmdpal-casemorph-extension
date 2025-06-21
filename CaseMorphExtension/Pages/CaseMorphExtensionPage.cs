using CaseMorphExtension.Core;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CaseMorphExtension;

internal sealed partial class CaseMorphExtensionPage : DynamicListPage
{
    private readonly List<ListItem> _items = [];
    private readonly IReadOnlyList<MethodInfo> _transformationMethods;
    public static bool LoadClipboardContent = true;

    public CaseMorphExtensionPage()
    {
        Title = "Case Morph";
        PlaceholderText = "Type your text here...";
        Name = "Case Morph";
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");

        _transformationMethods = GetTransformationMethods();
    }

    public override IListItem[] GetItems()
    {
        if (LoadClipboardContent && string.IsNullOrEmpty(SearchText))
        {
            InitializeItems(ClipboardHelper.GetText());
            LoadClipboardContent = false;
            
            _ = Task.Run(async () =>
            {
                await Task.Delay(2000);
                Interlocked.Exchange(ref LoadClipboardContent, true);
            });
        }

        return _items.ToArray();
    }

    private static IReadOnlyList<MethodInfo> GetTransformationMethods()
    {
        var methods = new List<MethodInfo>();
        Type transformerType = typeof(TextTransformer);
        methods = transformerType.GetMethods(BindingFlags.Public | BindingFlags.Static).ToList();
        return methods.AsReadOnly();
    }

    private void InitializeItems(string currentSearchText)
    {
        _items.Clear();

        foreach (var method in _transformationMethods)
        {
            var displayNameAttribute = method.GetCustomAttribute<TransformationDisplayNameAttribute>();
            string displayName = displayNameAttribute?.DisplayName ?? method.Name;
            string? transformedText = string.IsNullOrEmpty(currentSearchText) ? "No input available yet" : (string)method.Invoke(null, new object[] { currentSearchText });

            _items.Add(new ListItem(new CopyTextCommand(transformedText))
            {
                Title = transformedText,
                Subtitle = displayName,
                Icon = IconHelpers.FromRelativePath("Assets\\Logo.png"),
            });
        }
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        if (newSearch != oldSearch)
        {
            string textToUse = string.IsNullOrEmpty(newSearch) ? ClipboardHelper.GetText() : newSearch;
            LoadClipboardContent = string.IsNullOrEmpty(newSearch) ? true : false;

            InitializeItems(textToUse);
            RaiseItemsChanged(_items.Count);
        }
    }
}