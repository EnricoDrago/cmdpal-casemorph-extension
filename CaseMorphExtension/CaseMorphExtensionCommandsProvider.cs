// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System;

namespace CaseMorphExtension;

public partial class CaseMorphExtensionCommandsProvider : CommandProvider
{
    private readonly ICommandItem[] _commands;

    public CaseMorphExtensionCommandsProvider()
    {
        DisplayName = "Case Morph";
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        _commands = [new CommandItem(new CaseMorphExtensionPage()) 
        { 
            Title = DisplayName, 
            Subtitle = "Transform text to different formats!"
        }];
    }

    public override ICommandItem[] TopLevelCommands() => _commands;
}