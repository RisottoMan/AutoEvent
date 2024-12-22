using System;

namespace AutoEvent.API.Enums;

[Flags]
public enum EventFlags
{
    Default = 0,
    
    IgnoreRespawnTeam = 1 << 0,
    
    IgnoreDecontaminating = 2 << 0,
    
    IgnoreBulletHole = 3 << 0,
    
    IgnoreRagdoll = 4 << 0,
    
    IgnoreInfiniteAmmo = 5 << 0,
    
    IgnoreDroppingAmmo = 6 << 0,
    
    IgnoreDroppingItem = 7 << 0,
    
    IgnoreHandcuffing = 8 << 0,
    
    IgnoreAll = (IgnoreRespawnTeam | IgnoreDecontaminating | IgnoreBulletHole | IgnoreRagdoll | IgnoreInfiniteAmmo | IgnoreDroppingAmmo | IgnoreDroppingItem | IgnoreHandcuffing),
}
