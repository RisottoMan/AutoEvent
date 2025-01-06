using System;

namespace AutoEvent.API.Enums;

[Flags]
public enum EventFlags
{
    Default = 0,
    
    IgnoreRespawnTeam = 1 << 0,       // 1
    
    IgnoreDecontaminating = 1 << 1,   // 2
    
    IgnoreBulletHole = 1 << 2,        // 4
    
    IgnoreRagdoll = 1 << 3,           // 8
    
    IgnoreInfiniteAmmo = 1 << 4,      // 16
    
    IgnoreDroppingAmmo = 1 << 5,      // 32
    
    IgnoreDroppingItem = 1 << 6,      // 64
    
    IgnoreHandcuffing = 1 << 7,       // 128
    
    IgnoreAll = IgnoreRespawnTeam | IgnoreDecontaminating | IgnoreBulletHole | IgnoreRagdoll | IgnoreInfiniteAmmo | IgnoreDroppingAmmo | IgnoreDroppingItem | IgnoreHandcuffing
}
