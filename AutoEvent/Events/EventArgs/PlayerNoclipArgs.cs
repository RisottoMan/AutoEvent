using PluginAPI.Core;

namespace AutoEvent.Events.EventArgs
{
    public class PlayerNoclipArgs
    {
        public PlayerNoclipArgs(ReferenceHub ply, bool isAllowed = true)
        {
            Player = Player.Get(ply);
            IsAllowed = isAllowed;
        }
        public bool IsAllowed { get; set; }
        public Player Player { get; }
    }
}
