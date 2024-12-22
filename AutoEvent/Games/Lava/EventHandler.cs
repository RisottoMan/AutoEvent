using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using PlayerStatsSystem;

namespace AutoEvent.Games.Lava;
public class EventHandler
{
    public EventHandler(Plugin plugin)
    {
        _plugin = plugin;
    }

    private Plugin _plugin;
    public void OnHurting(HurtingEventArgs ev)
    {
        // prevent infinite recursion
        if (ev.DamageHandler.Type is DamageType.Custom)
        {
            ev.IsAllowed = true;
            return;
        }
        
        if (ev.DamageType == DeathTranslations.Falldown.Id)
        {
            ev.IsAllowed = false;
        }
        
        if (ev.Attacker != null && ev.Target != null)
        {
            ev.IsAllowed = false;
            if (_plugin.Config.GunEffects is null || _plugin.Config.GunEffects.IsEmpty())
            {
                goto defaultDamage;
            }
            ev.IsAllowed = true;
            //_plugin.Config.GunEffects.ApplyWeaponEffect(ref ev);
            ev.Attacker.ReceiveHitMarker(1);
            ev.Target?.Damage(new CustomReasonDamageHandler("Shooting", ev.Amount));
            DebugLogger.LogDebug($"Applying Custom Weapon Effect. Damage: {ev.Amount}");
            ev.IsAllowed = false;
            return;
        }
        defaultDamage:
            ev.Attacker?.ReceiveHitMarker();
            ev.Target?.Damage(new CustomReasonDamageHandler("Shooting", 3f));
            ev.IsAllowed = false;
            return;
    }

    public void OnDropItem(DropItemArgs ev) => ev.IsAllowed = _plugin.Config.PlayersCanDropGuns;
}