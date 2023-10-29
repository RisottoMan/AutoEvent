// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         BuildInfo.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/24/2023 10:42 PM
//    Created Date:     10/24/2023 10:42 PM
// -----------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommandSystem;

namespace AutoEvent.Commands;

[CommandHandler(typeof(GameConsoleCommandHandler))]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(ClientCommandHandler))]
public class BuildInfo : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        PermissionSystem.CheckPermission(sender, "ev.buildinfo.basic", out bool isConsole);
        bool showBuildInfo = false; 
        bool showDependencies = false;
        bool showFeatures = false;
        bool showReleaseInfo = false;
                if (arguments.Count >= 1)
                {
                    for (int i = 0; i < arguments.Count; i++)
                    {
                        string arg = arguments.At(i).ToLower();
                        bool subtract = arg.StartsWith("-");
                        if (subtract)
                            arg = arg.Substring(1, arg.Length - 1);
                        switch (arg)
                        {
                            case "all" or "*":
                                // showDependencies = true;
                                showBuildInfo = true;
                                showReleaseInfo = true;
                                showFeatures = true;
                                break;
                            case "features" or "feats":
                                showFeatures = !subtract;
                                break;
                            case "dependencies" or "deps":
                                showDependencies = !subtract;
                                break;
                            case "buildinfo" or "build":
                                showBuildInfo = !subtract;
                                break;
                            case "releaseinfo" or "release":
                                showReleaseInfo = !subtract;
                                break;
                        }
                    }
                }
        
        string colorAqua = isConsole ? "" : "<color=aqua>";
        string colorOrange = isConsole ? "" : "<color=pumpkin>";
        string colorYellow = isConsole ? "" : "<color=yellow>";
        string colorpink = isConsole ? "" : "<color=magenta>";
        string colorRed = isConsole ? "" : "<color=red>";
        string colorGreen = isConsole ? "" : "<color=green>";
        string colorBlue = isConsole ? "" : "<color=blue>";
        string colorLime = isConsole ? "" : "<color=lime>";
        string colorNickel = isConsole ? "" : "<color=nickel>";
        string colorWhite = isConsole ? "" : "<color=white>";
        string activeFeatures = "";
        if (showFeatures)
        {

            activeFeatures = "Active Features:\n";
            foreach (Enum feature in Enum.GetValues(typeof(ActiveFeatures)))
            {
                if (feature is ActiveFeatures.None or ActiveFeatures.All)
                    continue;

                if (VersionInfo.ActiveFeatures.HasFlag(feature))
                {
                    activeFeatures += $"  - {colorAqua}{feature}{colorWhite}\n";
                }
            }

            activeFeatures += "\n";
        }

        var CurrentRelease = global::AutoEvent.VersionInfo.Releases.OrderByDescending(x => x.SemanticVersion).First();
        string releaseInfo = "";
        if (showReleaseInfo)
        {
            releaseInfo =
                $"{CurrentRelease.Name} ({CurrentRelease.ReleaseDate.Month}/{CurrentRelease.ReleaseDate.Day}/{CurrentRelease.ReleaseDate.Year})\n" +
                $"Changelog:\n" +
                $"{CurrentRelease.ChangeLog}\n" +
                $"\n";
        }

        string dependencyInfo = "";
        if (showDependencies)
        {
            dependencyInfo = "\nBuild Dependencies: \n";
            foreach (var x in VersionInfo.Assemblies)
            {
                dependencyInfo += $"  - {x.Name} {(x.Version != "" ? $" (v{x.Version})" : "")} \n" +
                                  $"    - [Hash: {x.Hash}] \n";
            }

            dependencyInfo += "\n";
        }
        string buildInfo = 
            $"{colorNickel}Build Info:\n" +
            $"  - Build Branch - {colorYellow}{VersionInfo.CommitBranch}{colorWhite} (Commit {colorpink}{VersionInfo.CommitHash}{colorNickel})\n" +
            $"  - Build Tag    - {VersionInfo.CommitVersion}{colorNickel}\n" +
            $"  - Built by     - {VersionInfo.BuildUser}\n" +
            $"  - Build Date   - {VersionInfo.BuildTime.Month}/{VersionInfo.BuildTime.Day}/{VersionInfo.BuildTime.Year} [{VersionInfo.BuildTime.Hour:00}:{VersionInfo.BuildTime.Minute:00}.{VersionInfo.BuildTime.Second:00}]\n";
        string versionString =
            $"\nAuto Event Version - {colorAqua}{(CurrentRelease.Version)}{colorWhite}-{(Loader.IsExiledPlugin ? "Exiled" : "NWApi")}{(AutoEvent.BetaRelease ? $"{colorRed} [Beta]{colorWhite}" : "")}\n{colorWhite}" +
            $"SL Version - {(DebugLogger.SLVersion)}\n" +
            $"\n" +
            $"{activeFeatures}" +
            $"{releaseInfo}" +
            $"{(showBuildInfo ? buildInfo : "")}" +
            $"{dependencyInfo}";
        /*
         * Auto Event Version - v9.2.0-NWApi [beta]
         * SL Version 13.2.2-beta
         * 
         * Active Features:
         *   - Minigames20
         *   - Lobby
         *   - Vote
         *   - Powerups 
         *   - SchematicApi
         *   - BuildInfo
         *
         * Release 9.7.3-beta (9/13/23)
         * Changelog:
         * 12345 cool things happened
         *
         * Build Info:
         *   - Build Branch - Main (Commit 0b38ec3)
         *   - Build Tag    - 9.3.0
         *   - Built By     - Redforce04
         *   - Build Date   - 07/06/23
 
         */  
        response = versionString;
        return true;
    }

    public string Command => "ev"+ nameof(BuildInfo);
    public string[] Aliases => new[] { "evinfo", "evbuild" };
    public string Description => $"Gets the info for the current build of AutoEvent and other important info.";
}
