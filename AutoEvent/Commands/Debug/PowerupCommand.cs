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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoEvent.Interfaces;
using CommandSystem;
using PluginAPI.Core;
using UnityEngine;
using Player = PluginAPI.Core.Player;
#if EXILED
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
#endif
namespace AutoEvent.Commands.Debug;


public class PowerupCommand : ICommand, IPermission, IUsageProvider
{
    public string Command => nameof(Interfaces.Powerup);
    public string[] Aliases => Array.Empty<string>();
    public string Description => "Manages powerups.";
    public string[] Usage => new []{ "Give / Remove / Spawn / List", "Player / Powerup",  "Powerup / Position X", "Position Y", "Position Z" };

    public string Permission { get; set; } = "ev.debug";
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission(((IPermission)this).Permission, out bool IsConsoleCommandSender))
        {
            response = "<color=red>You do not have permission to use this command!</color>";
            return false;
        }

        if (arguments.Count < 1)
        {
            goto usage;
        }
        Player ply = null!;
        Interfaces.Powerup powerup = null!;
        
        switch (arguments.At(0).ToLower())
        {
            case "give":
                goto give;
            case "remove":
                goto remove;
            case "spawn":
                goto spawn;
            case "list":
                goto list;
            case "colliders":
                goto colliders;
            default:
                goto usage;
        }
        colliders:
        foreach (var powerup1 in PowerupManager.RegisteredPowerups)
        {
            //powerup1.ToggleColliders();
        }
        
        response = "Enabled ColliderVision";
        return true;
        
        
        
        list:
        response = "Available Powerups:\n";
        foreach (Powerup powerupItem in PowerupManager.RegisteredPowerups)
        {
            if (!IsConsoleCommandSender)
                response += $"\n<color=Red>{powerupItem.Name}</color> [<color=yellow>{powerupItem.GetType().Name}</color>]: <color=white>{powerupItem.Description}</color>";
            else
                response += $"\n{powerupItem.Name} [{powerupItem.GetType().Name}]: {powerupItem.Description}";
        }

        return true;
        give:

        if (arguments.Count - 1 < 2)
        {
            goto usage;
        }

        for (int i = 1; i < arguments.Count; i++)
        {
            switch (i-1)
            {
                case 0:
                    ply = Player.Get(arguments.At(i));
                    if (ply is null)
                    {
                        response = $"Could not find player \"{arguments.At(i)}\"";
                        return false;
                    }
                    break;
                case 1:
                    powerup = PowerupManager.GetPowerup(arguments.At(i));
                    if (powerup is null)
                    {
                        response = $"Could not find powerup \"{arguments.At(i)}\"";
                        return false;
                    }
                    break;
            }
        }
        powerup.ApplyPowerup(ply);
        response = $"Successfully gave powerup {powerup.Name} to player {ply.Nickname}.";
        return true;
        
        remove:
        if (arguments.Count - 1 < 2)
        {
            goto usage;
        }


        for (int i = 1; i < arguments.Count; i++)
        {
            switch (i - 1)
            {
                case 0:
                    ply = Player.Get(arguments.At(i));
                    if (ply is null)
                    {
                        response = $"Could not find player \"{arguments.At(i)}\"";
                        return false;
                    }

                    break;
                case 1:
                    if (arguments.At(i) == "*")
                    {
                        foreach (Interfaces.Powerup powerupInstance in PowerupManager.RegisteredPowerups)
                        {
                            if (powerupInstance.ActivePlayers.ContainsKey(ply))
                            {
                                powerupInstance.RemovePowerup(ply);
                            }
                        }

                        response = $"Successfully removed all powerups from player {ply.Nickname}.";
                        return true;
                    }

                    powerup = PowerupManager.GetPowerup(arguments.At(i));
                    if (powerup is null)
                    {
                        response = $"Could not find powerup \"{arguments.At(i)}\"";
                        return false;
                    }

                    break;
            }
        }
        powerup.RemovePowerup(ply);
        response = $"Successfully removed powerup {powerup.Name} from player {ply.Nickname}.";
        return true;
        
        spawn:
        if (arguments.Count - 1 < 4)
        {
            goto usage;
        }

        Vector3 pos = Vector3.zero;
        Vector3 relativePos = Vector3.zero;
        float scale = 1;
        float colliderScale = 1;
        for (int i = 1; i < arguments.Count; i++)
        {
            switch (i-1)
            {
                case 0:
                    powerup = PowerupManager.GetPowerup(arguments.At(i));
                    if (powerup is null)
                    {
                        response = $"Could not find powerup \"{arguments.At(i)}\"";
                        return false;
                    }
                    break;
                case 1:
                    if (arguments.At(i).StartsWith("~"))
                    {
                        string relPosX = arguments.At(i).Replace("~", "");
                        if (string.IsNullOrWhiteSpace(relPosX))
                        {
                            relativePos.x = 0;
                        }
                        else
                            float.TryParse(relPosX, out relativePos.x);
                        pos.x = Player.Get(sender).Position.x + relativePos.x;
                        break;
                    }
                    if (!float.TryParse(arguments.At(i), out pos.x))
                    {
                        response = $"Could not parse float \"{arguments.At(i)}\".";
                        return false;
                    }
                    break;
                case 2:
                    if (arguments.At(i).StartsWith("~"))
                    {
                        string relPosY = arguments.At(i).Replace("~", "");
                        if (string.IsNullOrWhiteSpace(relPosY))
                        {
                            relativePos.y = 0;
                        }
                        else
                            float.TryParse(relPosY, out relativePos.y);
                        pos.y = Player.Get(sender).Position.y + relativePos.y;
                        break;
                    }
                    if (!float.TryParse(arguments.At(i), out pos.y))
                    {
                        response = $"Could not parse float \"{arguments.At(i)}\".";
                        return false;
                    }
                    break;
                case 3:
                    if (arguments.At(i).StartsWith("~"))
                    {
                        string relPosZ = arguments.At(i).Replace("~", "");
                        if (string.IsNullOrWhiteSpace(relPosZ))
                        {
                            relativePos.z = 0;
                        }
                        else
                            float.TryParse(relPosZ, out relativePos.z);
                        pos.z = Player.Get(sender).Position.z + relativePos.z;
                        break;
                    }
                    if (!float.TryParse(arguments.At(i), out pos.z))
                    {
                        response = $"Could not parse float \"{arguments.At(i)}\".";
                        return false;
                    }
                    break;
                case 4:
                    if (!float.TryParse(arguments.At(i), out scale))
                    {
                        response = $"Could not parse float \"{arguments.At(i)}\".";
                        return false;
                    }
                    break;
                case 5:
                    if (!float.TryParse(arguments.At(i), out colliderScale))
                    {
                        response = $"Could not parse float \"{arguments.At(i)}\".";
                        return false;
                    }
                    break;
            }
        }
        powerup.SpawnPickup(pos, scale, colliderScale);
        
        response = $"Successfully spawned powerup {powerup.Name} at position ({pos.x}, {pos.y}, {pos.z})";
        return true;
        usage:
        response = $"Please enter a valid subcommand: \n {this.Command}";
        foreach (var x in this.Usage)
        {
            if(sender is not ServerConsoleSender)
                response += $"[{x}]";
            else    
                response += $"[{x}]";
        }

        response += $" -> {this.Description}";
        return false;
    }

}