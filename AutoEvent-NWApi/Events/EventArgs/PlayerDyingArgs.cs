using PlayerStatsSystem;
using PluginAPI.Core;

namespace AutoEvent.Events.EventArgs
{
    public class PlayerDyingArgs
    {
        public PlayerDyingArgs(Player target, DamageHandlerBase damageHandler)
        {
            DamageHandler = damageHandler;

            Target = target;

            AttackerHandler = damageHandler as AttackerDamageHandler;

            Attacker = Player.Get(AttackerHandler?.Attacker.Hub);

            if (DamageHandler is UniversalDamageHandler damage)
            {
                DamageType = damage.TranslationId;
            }
        }

        public Player Target { get; }

        public Player Attacker { get; }

        public AttackerDamageHandler AttackerHandler { get; set; }

        public DamageHandlerBase DamageHandler { get; set; }

        public byte DamageType { get; set; }

        public bool IsAllowed { get; set; } = true;
    }
}
