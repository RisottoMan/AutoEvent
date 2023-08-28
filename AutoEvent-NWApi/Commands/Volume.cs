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
using CommandSystem;
using MEC;
using PluginAPI.Core;
using SCPSLAudioApi.AudioCore;

namespace AutoEvent.Commands;

public class Volume : ICommand
{
    public string Command => "volume";
        public string Description => "Set the global music volume, takes on 1 argument - the volume from 0% - 200%.";
        public string[] Aliases => null;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                if (!AutoEvent.Singleton.Config.PermissionList.Contains(
                        ServerStatic.PermissionsHandler._members[Player.Get(sender).UserId]))
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

                response = $"<color=green>The volume has been set!</color>";
                return true;
            }
            catch (Exception e)
            {
                response = $"Could not set the volume due to an error. This could be a bug. Ensure audio is playing while using this command.";
                Log.Debug($"An error has occured while trying to set the volume. \n{e}");
                return false;
            }
        }
}