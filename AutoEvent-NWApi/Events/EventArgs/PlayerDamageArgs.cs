using PlayerStatsSystem;
using PluginAPI.Core;

namespace AutoEvent.Events.EventArgs
{
    public class PlayerDamageArgs
    {
        public PlayerDamageArgs(Player target, DamageHandlerBase damageHandler)
        {
            DamageHandler = damageHandler as AttackerDamageHandler;

            Attacker = Player.Get(DamageHandler?.Attacker.Hub);

            Target = target;
        }

        public Player Target { get; }

        public Player Attacker { get; }

        public float Amount
        {
            get => DamageHandler.Damage;
            set => DamageHandler.Damage = value;
        }

        public AttackerDamageHandler DamageHandler { get; set; }

        public bool IsAllowed { get; set; } = true;
    }
}
