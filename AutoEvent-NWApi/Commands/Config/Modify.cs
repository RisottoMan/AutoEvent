// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         ReloadConfigs.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/13/2023 4:29 PM
//    Created Date:     09/13/2023 4:29 PM
// -----------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using AutoEvent.API.OptionModificationEngine;
using AutoEvent.Interfaces;
using CommandSystem;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PluginAPI.Core;
using YamlDotNet.Serialization.Schemas;
#if EXILED
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
#endif
namespace AutoEvent.Commands.Config;


public class Modify : ICommand, IUsageProvider
{
    public string Command => nameof(Modify);
    public string[] Aliases => Array.Empty<string>();
    public string Description => "Modifies an option in a config or preset for an event.";
    public string[] Usage => new string[] { "event", "preset", "option", "[value]" };

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {

#if EXILED
        if (!((CommandSender)sender).CheckPermission("ev.config.modify"))
        {
            response = "You do not have permission to use this command";
            return false;
        }
#else
        var config = AutoEvent.Singleton.Config;
        var player = Player.Get(sender);
        if (sender is ServerConsoleSender || sender is CommandSender cmdSender && cmdSender.FullPermissions)
        {
            goto skipPermissionCheck;
        }

        if (!config.PermissionList.Contains(ServerStatic.PermissionsHandler._members[player.UserId]))
        {
            response = "<color=red>You do not have permission to use this command!</color>";
            return false;
        }
#endif
        skipPermissionCheck:
        response = "";

        // Event Missing
        if (arguments.Count < 1)
        {
            response = $"Please select an event to modify.\n";
            goto BasicUsage;
        }

        // Event Not Found
        Event ev = Event.GetEvent(arguments.At(0));
        if (ev is null)
        {
            DebugLogger.LogDebug($"Event is null.", LogLevel.Debug);
            response = $"Could not find event \"{arguments.At(0)}\".";
            return false;
        }
        
        // Preset Missing
        if (arguments.Count < 2)
        {
            response = $"Please select a preset to modify.\n";
            goto BasicUsage;
        }
        
        // Preset List is null - this shouldn't be possible.
        if (ev.ConfigPresets is null)
        {
            DebugLogger.LogDebug($"Config Presets List is null.", LogLevel.Debug);
            response = "An error has occured.";
            return false;
        }

        // Config doesnt exist.
        var conf = ev.ConfigPresets.FirstOrDefault(x => x.PresetName.ToLower() == arguments.At(1).ToLower());
        if (conf is null)
        {
            response = $"Could not find preset \"{arguments.At(1)}\"";
            return false;
        }

        // List available options in a preset. 
        if (arguments.Count < 3)
        {
            string resp = $"Available options for preset \"{conf.PresetName}\" (event {ev.Name}):\n";
            var properties = conf.GetType().GetProperties();
            foreach (var prop in properties)
            {
                string desc = "";
                var description = prop.GetCustomAttribute<DescriptionAttribute>();
                if (description is not null)
                {
                    desc = description.Description;
                }

                resp +=
                    $"  <color=yellow>{prop.Name}</color> [{prop.PropertyType.Name}]{(desc == "" ? "" : " - " + desc)}, \n";
            }

            response = resp;
            return true;
        }
        
        // Process Config Modifications
        var engine = new OptionModificationEngine(arguments.ToArray(), ev, conf);
        return engine.Process(ref response);
        
        
        // List basic command usage.
        BasicUsage:
        response += $"Command Usage: \n" +
                    $"  <color=yellow>modify [event] [config / preset] [option] [new value]  \n</color>" +
                    $"   For modifying lists or dictionaries, use the following actions: \n" +
                    "      <color=yellow>Add [key*] [value]</color>       -> Add a new entry to the. *Note: Key is not necessary for lists.*\n" +
                    "      <color=yellow>Remove [key]</color>             -> Remove an existing entry.\n" +
                    "      <color=yellow>Modify [key] [new value]</color> -> Modify an existing entry." +
                    "    For complex configs, you can use json or yaml to serialize the config.";


        return false;
    }
}
