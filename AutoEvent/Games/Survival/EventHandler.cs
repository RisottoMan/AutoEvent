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
                SpawnZombie(ev.Player);
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

    public void OnDying(DyingEventArgs ev)
    {
        ev.IsAllowed = false;
        SpawnZombie(ev.Player);
    }

    public void OnJoined(JoinedEventArgs ev)
    {
        if (Player.List.Count(r => r.Role == RoleTypeId.Scp0492) > 0)
        {
            SpawnZombie(ev.Player);
        }
        else
        {
            ev.Player.GiveLoadout(_plugin.Config.PlayerLoadouts);
            ev.Player.Position = _plugin.SpawnList.RandomItem().transform.position;
            ev.Player.CurrentItem = ev.Player.Items.ElementAt(1);
            Extensions.SetPlayerAhp(ev.Player, 100, 100, 0);
        }
    }

    private void SpawnZombie(Player player)
    {
        player.GiveLoadout(_plugin.Config.ZombieLoadouts);
        player.Position = _plugin.SpawnList.RandomItem().transform.position;
        Extensions.PlayPlayerAudio(_plugin.SoundInfo.AudioPlayer, player, _plugin.Config.ZombieScreams.RandomItem(), 15);
    }
}