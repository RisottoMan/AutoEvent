// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         DebugCommand.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/05/2023 6:58 PM
//    Created Date:     09/05/2023 6:58 PM
// -----------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using CommandSystem;
using PluginAPI.Core;
using PluginAPI.Helpers;

namespace AutoEvent.Commands;

public class Debug : ICommand
{
     public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
     {
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
          skipPermissionCheck:
          if (arguments.Count < 1)
          {
               goto list;
          }
          
          switch (arguments.At(0).ToLower())
          {
               case "list":
                    goto list;
               case "enable":
                    DebugLogger.Debug = true;
                    response = "Debug mode has been enabled.";
                    return true;
               case "disable":
                    DebugLogger.Debug = false;
                    response = "Debug mode has been disabled.";
                    return true;
               case "write":
                    try
                    {
                         DebugLogger.WriteOutput("debug-output.log");
                         response = "Output written to debug file.";
                         return true;
                    }
                    catch (Exception e)
                    {
                         Log.Debug($"Could not write debug log to path. Exception: \n {e}");
                    }

                    response = "An error has occured while trying to write file to path. Check logs for details.";
                    return false;
               // ReSharper disable once StringLiteralTypo
               case "antiend":
                    DebugLogger.AntiEnd = !DebugLogger.AntiEnd;
                    response = $"Anti-End has been {(DebugLogger.AntiEnd ? "Enabled" : "Disabled")}.";
                    return true;
          }
          response = $"Debug mode is currently {( AutoEvent.Debug ? "enabled" : "disabled")}";
          return true;
          list:
          response = $"Valid Debug Subcommands:\n" +
                     $"<color=yellow>Enable<color=white> -> Enables Debug Mode\n" +
                     $"<color=yellow>Disable<color=white> -> Disables Debug Mode\n" +
                     $"<color=yellow>List<color=white> -> Lists all debug options.\n" +
                     $"<color=yellow>Write<color=white> -> Writes all debug output to a log. (including past logs)\n" +
                     $"<color=yellow>AntiEnd<color=white> -> Prevents events from ending.\n";
          return true;

     }

     public string Command { get; } = "Debug";
     public string[] Aliases { get; } = Array.Empty<string>();
     public string Description { get; } = "Does debugging things.";
}