using PlayerStatsSystem;
using PluginAPI.Core;

namespace AutoEvent.Events.EventArgs
{
    public class PlayerDyingArgs
    {
        public PlayerDyingArgs(Player target, DamageHandlerBase damageHandler)
        {
            DamageHandler = damageHandler;
            
            var attackerHandler = damageHandler as AttackerDamageHandler;
            Attacker = Player.Get(attackerHandler.Attacker.Hub);

            Target = target;
        }

        public Player Target { get; }

        public DamageHandlerBase DamageHandler { get; set; }

        public bool IsAllowed { get; set; } = true;

        public Player Attacker { get; }
    }
}
