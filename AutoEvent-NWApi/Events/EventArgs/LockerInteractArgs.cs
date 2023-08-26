using MapGeneration.Distributors;
using PluginAPI.Core;

namespace AutoEvent.Events.EventArgs
{
    public class LockerInteractArgs
    {
        public LockerInteractArgs(Locker locker, ReferenceHub ply, bool isAllowed = true)
        {
            Player = Player.Get(ply);
            Locker = locker;
            LockerType = locker.StructureType;
            IsAllowed = isAllowed;
        }

        public Player Player { get; }

        public Locker Locker { get; set; }

        public StructureType LockerType { get; set; }

        public bool IsAllowed { get; set; }
    }
}
