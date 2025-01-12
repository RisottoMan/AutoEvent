using PlayerRoles;
using CustomPlayerEffects;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;

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
            
        if (ev.Attacker.IsScp && ev.Player.IsHuman)
        {
            if (ev.Player.ArtificialHealth <= 50)
            {
                SpawnZombie(ev.Player);
            }
            else
            {
                ev.IsAllowed = false;
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
        Timing.CallDelayed(5f, () =>
        {
            // game not ended
            if (Player.List.Count(r => r.IsScp) > 0 && Player.List.Count(r => r.IsHuman) > 0)
            {
                SpawnZombie(ev.Player);
            }
        });
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
        }
    }

    private void SpawnZombie(Player player)
    {
        player.GiveLoadout(_plugin.Config.ZombieLoadouts);
        player.Position = _plugin.SpawnList.RandomItem().transform.position;
        Extensions.PlayPlayerAudio(_plugin.SoundInfo.AudioPlayer, player, _plugin.Config.ZombieScreams.RandomItem(), 15);
    }
}