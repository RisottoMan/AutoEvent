using System;
using MEC;
using PlayerRoles;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.MarshmallowMan;

namespace AutoEvent.Games.Infection;
public class EventHandler
{
    Plugin _plugin;
    public EventHandler(Plugin plugin)
    {
        _plugin = plugin;
    }

    public void OnHurting(HurtingEventArgs ev)
    {
        if (ev.DamageHandler.Type == DamageType.Falldown)
            ev.IsAllowed = false;
        
        if (_plugin.IsHalloweenUpdate)
        {
            ev.Player.Role.Set(RoleTypeId.Scientist, RoleSpawnFlags.None);
            ev.Player.IsGodModeEnabled = true;
            Timing.CallDelayed(0.1f, () =>
            {
                ev.Player.EnableEffect<MarshmallowEffect>();
            });
        }
        else if (_plugin.IsChristmasUpdate && Enum.TryParse("ZombieFlamingo", out RoleTypeId roleTypeId))
        {
            if (ev.Player.Role.Type == roleTypeId)
            {
                ev.IsAllowed = false;
                return;
            }
            
            ev.Player.Role.Set(roleTypeId, RoleSpawnFlags.None);
            ev.Attacker.ShowHitMarker();
            Extensions.PlayPlayerAudio(_plugin.SoundInfo.AudioPlayer, ev.Player, _plugin.Config.ZombieScreams.RandomItem(), 15);
        }
        else if (ev.Attacker != null && ev.Attacker.Role == RoleTypeId.Scp0492)
        {
            ev.Player.GiveLoadout(_plugin.Config.ZombieLoadouts);
            ev.Attacker.ShowHitMarker();
            Extensions.PlayPlayerAudio(_plugin.SoundInfo.AudioPlayer, ev.Player, _plugin.Config.ZombieScreams.RandomItem(), 15);
        }
    }

    public void OnJoined(JoinedEventArgs ev)
    {
        if (_plugin.IsHalloweenUpdate || _plugin.IsChristmasUpdate)
            return;
        
        if (Player.List.Count(r => r.Role == RoleTypeId.Scp0492) > 0)
        {
            ev.Player.GiveLoadout(_plugin.Config.ZombieLoadouts);
            ev.Player.Position = _plugin.SpawnList.RandomItem().transform.position;
            Extensions.PlayPlayerAudio(_plugin.SoundInfo.AudioPlayer, ev.Player, _plugin.Config.ZombieScreams.RandomItem(), 15);
        }
        else
        {
            ev.Player.GiveLoadout(_plugin.Config.PlayerLoadouts);
            ev.Player.Position = _plugin.SpawnList.RandomItem().transform.position;
        }
    }

    public void OnDied(DiedEventArgs ev)
    {
        Timing.CallDelayed(2f, () =>
        {
            if (_plugin.IsHalloweenUpdate)
            {
                ev.Player.Role.Set(RoleTypeId.Scientist, RoleSpawnFlags.None);
                ev.Player.IsGodModeEnabled = true;
                Timing.CallDelayed(0.1f, () =>
                {
                    ev.Player.EnableEffect<MarshmallowEffect>();
                });
            }
            else if (_plugin.IsChristmasUpdate && Enum.TryParse("ZombieFlamingo", out RoleTypeId roleTypeId))
            {
                ev.Player.Role.Set(roleTypeId, RoleSpawnFlags.None);
            }
            else
            {
                ev.Player.GiveLoadout(_plugin.Config.ZombieLoadouts);
                Extensions.PlayPlayerAudio(_plugin.SoundInfo.AudioPlayer, ev.Player, _plugin.Config.ZombieScreams.RandomItem(), 15);
            }
            
            ev.Player.Position = _plugin.SpawnList.RandomItem().transform.position;
        });
    }
}