using Exiled.API.Features;

namespace AutoEvent.Events.EventArgs
{
    public class UsingStaminaArgs
    {
        public UsingStaminaArgs(ReferenceHub ply, float amount, bool isAllowed = true)
        {
            Player = Player.Get(ply);
            Amount = amount;
            IsAllowed = isAllowed;
        }
        public Player Player { get; }
        public float Amount { get; }
        public bool IsAllowed { get; set; }
    }
}
