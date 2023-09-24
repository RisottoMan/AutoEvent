// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         InfiniteAmmo.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/23/2023 11:44 AM
//    Created Date:     09/23/2023 11:44 AM
// -----------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AutoEvent.API;
using AutoEvent.API.Enums;
using CommandSystem;
#if EXILED
using Exiled.Permissions.Extensions;
#endif
using PlayerRoles;
using PluginAPI.Core;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace AutoEvent.Commands.Debug;

public class SetRole : ICommand, IUsageProvider
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {

#if EXILED
        if (!sender.CheckPermission("ev.debug"))
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

        // ev debug setrole [ply] [class / none] [effect 1 / none] [infinite ammo (infinite / endless / none)] [size] [health] [artificial health] [stamina] [items] [flags]
        // ev debug setrole lfg sci none none 1 100 100 1 none 0
        if (arguments.Count >= 1)
        {
            Player ply = Player.GetPlayers().FirstOrDefault(pl => pl.Nickname.ToLower().Contains(arguments.At(0).ToLower()));
            if (ply is null)
            {
                response = $"Player \"{arguments.At(0)}\" not found. Please specify a valid player.";
                return false;
            }

            LoadoutFlags flags = LoadoutFlags.None;
            Loadout loadout = new Loadout();
            loadout.Roles = new Dictionary<RoleTypeId, int>();
            
            for (int i = 1; i < arguments.Count; i++)
            {
                switch (i)
                {
                    case 1:
                        try
                        {
                            if (arguments.At(i).ToLower() is "" or "none")
                            {
                                continue;
                            }
                            List<string> roles = new List<string>();
                            foreach (string roleName in Enum.GetNames(typeof(RoleTypeId)))
                            {
                                roles.Add(roleName);
                            }
                            var role = (RoleTypeId)Enum.Parse(typeof(RoleTypeId), roles.First<string>(x => x.ToLower().Contains(arguments.At(i).ToLower())), true);
                            loadout.Roles.Add(role, 100);
                        }
                        catch (Exception e)
                        {
                            response = "Please specify a valid role.";
                            return false;
                        }
                        break;
                    case 2:
                        try
                        {
                            loadout.Effects = new List<Effect>();
                            if (arguments.At(i).ToLower() == "none")
                                continue;
                            List<string> effects = new List<string>();
                            foreach (string roleName in Enum.GetNames(typeof(StatusEffect)))
                            {
                                effects.Add(roleName);
                            }

                            var effect = (StatusEffect)Enum.Parse(typeof(StatusEffect),
                                effects.First<string>(x => x.ToLower().Contains(arguments.At(i).ToLower())), true);
                            loadout.Effects.Add(new Effect(effect, 1, 10f, true));
                        }
                        catch (Exception e)
                        {
                            DebugLogger.LogDebug(e.ToString());
                            response = "Please specify a valid effect.";
                            return false;
                        }
                        break;
                    case 3:
                        switch (arguments.At(i).ToLower())
                        {
                            case "endless":
                                loadout.InfiniteAmmo = AmmoMode.EndlessClip;
                                break;
                            case "infinite":
                                loadout.InfiniteAmmo = AmmoMode.InfiniteAmmo;
                                break;
                            default:
                                loadout.InfiniteAmmo = AmmoMode.None;
                                break;
                        }
                        break;
                    case 4:
                        if (!float.TryParse(arguments.At(i), out float size))
                        {
                            response = "Please specify a valid float.";
                            return false;
                        }

                        loadout.Size = new Vector3(size, size, size);
                        break;
                    case 5:
                        if (!float.TryParse(arguments.At(i), out float health))
                        {
                            response = "Please specify a valid float.";
                            return false;
                        }

                        loadout.Health = (int)health;
                        break;
                    case 6:
                        // TODO - FIX THIS
                        if (!float.TryParse(arguments.At(i), out float artHealth))
                        {
                            response = "Please specify a valid float.";
                            return false;
                        }

                        loadout.ArtificialHealth = new ArtificialHealth(artHealth);
                        break;
                    case 7:
                        
                        // todo - fix this
                        if (!float.TryParse(arguments.At(i), out float stamina))
                        {
                            response = "Please specify a valid float.";
                            return false;
                        }

                        loadout.Stamina = (int)stamina;
                        break;
                    case 8:
                        try
                        {
                            // todo fix this 
                            if (arguments.At(0).ToLower() is "none" or "")
                            {
                                continue;
                            }
                            loadout.Items = new List<ItemType>();
                            List<string> roles = new List<string>();
                            foreach (string roleName in Enum.GetNames(typeof(ItemType)))
                            {
                                roles.Add(roleName);
                            }
                            var item = (ItemType)Enum.Parse(typeof(ItemType), roles.First<string>(x => x.ToLower().Contains(arguments.At(i).ToLower())), true);
                            loadout.Items.Add(item);
                        }
                        catch (Exception e)
                        {
                            response = "Please specify a valid item.";
                            return false;
                        }
                        break;
                    case 9:
                        try
                        {
                            if(!int.TryParse(arguments.At(i), out int flagInt))
                            {
                                response = "Please specify a valid LoadoutFlags.";
                                return false;
                            }

                            flags = (LoadoutFlags)flagInt;
                        }
                        catch (Exception e)
                        {
                            response = "Please specify a valid LoadoutFlags.";
                            return false;
                        }

                        break;
                }
            }
            ply.GiveLoadout(loadout, flags);
            response = $"Role has been set.";
            return true;
        }

        response = "Please specify a player.";
        return false;
    }

    public string Command { get; } = "SetRole";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "Gives a user an impact grenade.";
    public string[] Usage { get; } = new[] { "%player%", "[frag / flash / ball]" };
}
