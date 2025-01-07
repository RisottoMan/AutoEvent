using MEC;
using PlayerRoles;
using CustomPlayerEffects;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;

namespace AutoEvent.Games.Survival;
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
        {
            ev.IsAllowed = false;
        }

        if (ev.Attacker == null || ev.Player == null)
            return;
            
        if (ev.Attacker.IsScp && !ev.Player.IsScp)
        {
            if (ev.Player.Health <= 50)
            {
                ev.Player.GiveLoadout(_plugin.Config.ZombieLoadouts);
            }
            else
            {
                ev.Amount = 0;
                ev.Player.Health -= 50;
                ev.Player.ArtificialHealth = 0;
            }

            ev.Attacker.ShowHitMarker();
        }
        else if (ev.Attacker.IsHuman && ev.Player.IsScp)
        {
            ev.Player.EnableEffect<Disabled>(1, 1);
            ev.Player.EnableEffect<Scp1853>(1, 1);
        }
            
        if (ev.Player == _plugin.FirstZombie)
        {
            ev.Amount = 1;
        }
    }

    public void OnDying(DyingEventArgs ev) // Timing.CallDelayed 2sec
    {
        ev.IsAllowed = false;
        SpawnZombie(ev.Player, 3000);
    }

    public void OnJoined(JoinedEventArgs ev)
    {
        if (Player.List.Count(r => r.Role == RoleTypeId.Scp0492) > 0)
        {
            SpawnZombie(ev.Player, 10000);
        }
        else
        {
            ev.Player.Role.Set(RoleTypeId.NtfSergeant, RoleSpawnFlags.AssignInventory);
            ev.Player.Position = _plugin.SpawnList.RandomItem().transform.position;
            Extensions.SetPlayerAhp(ev.Player, 100, 100, 0);

            Timing.CallDelayed(0.1f, () =>
            {
                ev.Player.CurrentItem = ev.Player.Items.ElementAt(1);
            });
        }
    }

    public void SpawnZombie(Player player, float hp)
    {
        player.Role.Set(RoleTypeId.Scp0492, RoleSpawnFlags.AssignInventory);
        player.Position = _plugin.SpawnList.RandomItem().transform.position;
        player.EnableEffect<Disabled>(1, 1);
        player.EnableEffect<Scp1853>(1, 1);
        player.Health = hp;
    }
}