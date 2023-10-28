// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         Volume.cs
//    Author:           Redforce04#4091
//    Revision Date:    08/28/2023 12:45 PM
//    Created Date:     08/28/2023 12:45 PM
// -----------------------------------------

using System;
using AutoEvent.Interfaces;
using CommandSystem;
using MEC;
using PluginAPI.Core;
using SCPSLAudioApi.AudioCore;
using Player = Exiled.Events.Handlers.Player;

namespace AutoEvent.Commands;

public class Volume : ICommand, IUsageProvider, IPermission
{
    public string Command => nameof(Volume);
        public string Description => "Set the global music volume, takes on 1 argument - the volume from 0%-200%";
        public string[] Aliases => new string[] { };
        public string[] Usage => new string[] { "Volume %" };
        public string Permission { get; set; } = "ev.volume";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            
            try
            {
                if (!sender.CheckPermission(((IPermission)this).Permission, out bool IsConsoleCommandSender))
                {
                    response = "<color=red>You do not have permission to use this command!</color>";
                    return false;
                }
                if (arguments.Count != 1)
                {
                    response =
                        $"The current volume is {AutoEvent.Singleton.Config.Volume}%. Please specify the new volume from 0% - 200% to set it!";
                    return false;
                }

                float newVolume = float.Parse(arguments.At(0));
                AutoEvent.Singleton.Config.Volume = newVolume;
                if (Extensions.AudioBot is not null)
                {
                    var audioPlayer = AudioPlayerBase.Get(Extensions.AudioBot).Volume = newVolume;
                }

                response = $"The volume has been set!";
                return true;
            }
            catch (Exception e)
            {
                response = $"Could not set the volume due to an error. This could be a bug. Ensure audio is playing while using this command.";
                DebugLogger.LogDebug($"An error has occured while trying to set the volume.", LogLevel.Warn, true);
                DebugLogger.LogDebug($"{e}", LogLevel.Debug);
                return false;
            }
        }

}