using System.Linq;
using AutoEvent.API.Enums;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;

namespace AutoEvent.Games.HideAndSeek;
public class EventHandler
{
    private Plugin _plugin { get; set; }
    public EventHandler(Plugin ev)
    {
        _plugin = ev;
    }

    public void OnHurting(HurtingEventArgs ev)
    {
        if (ev.DamageHandler.Type == DamageType.Falldown)
        {
            ev.IsAllowed = false;
        }

        if (ev.Player.GetEffect<SpawnProtected>().IsEnabled)
        {
            ev.IsAllowed = false;
            return;
        }

        if (ev.Attacker != null)
        {
            ev.IsAllowed = true;
            bool isAttackerTagger = ev.Attacker.Items.Any(r => r.Type == _plugin.Config.TaggerWeapon);
            bool isTargetTagger = ev.Player.Items.Any(r => r.Type == _plugin.Config.TaggerWeapon);
            if (!isAttackerTagger || isTargetTagger)
            {
                ev.IsAllowed = false;
                return;
            }

            MakePlayerNormal(ev.Attacker);
            MakePlayerCatchUp(ev.Player);
        }
    }

    public void MakePlayerNormal(Player player)
    {
        player.EnableEffect<SpawnProtected>(_plugin.Config.NoTagBackDuration, false);
        player.GiveLoadout(_plugin.Config.PlayerLoadouts, LoadoutFlags.IgnoreItems | LoadoutFlags.IgnoreWeapons | LoadoutFlags.IgnoreGodMode);
        player.ClearInventory();
    }

    public void MakePlayerCatchUp(Player player)
    {
        bool isLast = Player.List.Count(ply => ply.HasLoadout(_plugin.Config.PlayerLoadouts)) <= _plugin.Config.PlayersRequiredForBreachScannerEffect;
        if (isLast)
        {
            player.EnableEffect(EffectType.Scanned, 255);
        }

        player.GiveLoadout(_plugin.Config.TaggerLoadouts, LoadoutFlags.IgnoreItems | LoadoutFlags.IgnoreWeapons | LoadoutFlags.IgnoreGodMode);
        player.ClearInventory();

        if (isLast)
        {
            player.EnableEffect(EffectType.Scanned, 0, 1f);
        }
        
        if (player.CurrentItem == null)
        {
            player.CurrentItem = player.AddItem(_plugin.Config.TaggerWeapon);
        }
    }
    
    public void OnJailbirdCharge(ChargingJailbirdEventArgs ev) => ev.IsAllowed = _plugin.Config.JailbirdCanCharge;
}
