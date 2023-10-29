// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         Loadouts.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/28/2023 7:31 PM
//    Created Date:     10/28/2023 7:31 PM
// -----------------------------------------

using System.Collections.Generic;
using System.Linq;
using AutoEvent.API;
using CustomPlayerEffects;
using InventoryMenu.API;
using InventoryMenu.API.EventArgs;
using InventorySystem.Items;
using MEC;

namespace AutoEvent.Games.GhostBusters.Features;

public class Loadouts
{
    private Plugin _plugin;
    public Loadouts(Plugin plugin)
    {
        _plugin = plugin;
    }
    internal void HuntersSelectLoadout(MenuItemClickedArgs ev)
    {
        GhostBusterClassType type = ev.MenuItemClicked.Item switch
        {
            ItemType.MicroHID => GhostBusterClassType.HunterTank, 
            ItemType.ParticleDisruptor => GhostBusterClassType.HunterSniper, 
            ItemType.Jailbird => GhostBusterClassType.HunterMelee, 
            _ => GhostBusterClassType.HunterUnchosen
        };
            
        if (!_plugin.Classes.ContainsKey(ev.Player))
            _plugin.Classes.Add(ev.Player, new GhostBusterClass(ev.Player, type));
        else
            _plugin.Classes[ev.Player].ClassType = type;
            
        switch (type)
        {
            case GhostBusterClassType.HunterTank:
                ev.Player.GiveLoadout(_plugin.Config.TankLoadout);
                break;
            case GhostBusterClassType.HunterSniper:
                ev.Player.GiveLoadout(_plugin.Config.SniperLoadout);
                break;
            case GhostBusterClassType.HunterMelee:
                ev.Player.GiveLoadout(_plugin.Config.MeleeLoadout);
                if(ev.Player.EffectsManager.GetEffect<MovementBoost>()?.Intensity > 0)
                    ev.Player.ApplyFakeEffect<Scp207>(1);
                break;
        }
        ev.Player.HideMenu();
            
        Timing.CallDelayed(0.1f, () =>
        {
            ev.Player.CurrentItem = ev.Player.Items.First(x => x.ItemTypeId.IsWeapon());
        });
    }
    internal void GhostUseAbility(MenuItemClickedArgs ev)
    {
        if (!_plugin.Classes.ContainsKey(ev.Player))
        {
            DebugLogger.LogDebug("Doesnt contain player");
            return;
        }

        if (!ev.IsLeftClick)
        {
            int uses = _plugin.Classes[ev.Player].Ability!.AllowedUses;
            string cooldown = (_plugin.Classes[ev.Player].AbilityCooldown < -1 ? "No more uses." : $"Ability Cooldown: {_plugin.Classes[ev.Player].AbilityCooldown}");
                
            ev.Player.ReceiveHint($"{_plugin.Classes[ev.Player].Ability!.Description}\n{cooldown}\n" +
                                  $"Ability Uses [{_plugin.Classes[ev.Player].AbilityUses} / {(uses == -1 ? "Unlimited" : uses.ToString())}]");
            return;
        }

        if (!_plugin.Classes[ev.Player].IsAbilityReady(out string reason))
        {
            ev.Player.ReceiveHint($"Cannot use ability - {reason}.", 5f);
            return;
        }
        _plugin.Classes[ev.Player].Ability!.OnAbilityUsed(ev.Player, _plugin.Classes[ev.Player].Ability!);
    }
    internal void GhostSelectLoadout(MenuItemClickedArgs ev)
    {
        GhostBusterClassType type = ev.MenuItemClicked.Item switch

        {
            ItemType.SCP268 => GhostBusterClassType.GhostInvisibility,
            ItemType.SCP207 => GhostBusterClassType.GhostSpeed,
            ItemType.GrenadeHE => GhostBusterClassType.GhostExplosive,
            ItemType.GrenadeFlash => GhostBusterClassType.GhostFlash,
            ItemType.SCP018 => GhostBusterClassType.GhostBall,
            ItemType.SCP2176 => GhostBusterClassType.GhostLockdown,
            _ => GhostBusterClassType.GhostUnchosen
        };
            
        if (!_plugin.Classes.ContainsKey(ev.Player))
            _plugin.Classes.Add(ev.Player, new GhostBusterClass(ev.Player, type));
        else
            _plugin.Classes[ev.Player].ClassType = type;

        ev.Player.HideMenu();
        ev.Player.ShowMenu(_plugin.GhostPowerupMenu);
    }
    public void ProcessGetMenuArgs(GetMenuItemsForPlayerArgs ev)
    {
        if (!_plugin.Classes.ContainsKey(ev.Player))
            return;
        var ability = _plugin.Abilities.First(x => x.ClassType == _plugin.Classes[ev.Player].ClassType);
        var item = ev.Menu.ItemBases.First(x => x.Key == ability.Serial);
        
        ev.Items = new Dictionary<ushort, ItemBase>() { { item.Key, item.Value } };
        
    }
}