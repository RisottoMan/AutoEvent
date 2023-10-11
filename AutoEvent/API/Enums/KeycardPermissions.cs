// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         KeycardLevel.cs
//    Author:           Redforce04#4091
//    Revision Date:    09/24/2023 2:04 PM
//    Created Date:     09/24/2023 2:04 PM
// -----------------------------------------

using System;

namespace AutoEvent.API.Enums;

[Flags]
public enum KeycardPermissions
{
        /// <summary>
        /// No permissions.
        /// </summary>
        None = 0,

        /// <summary>
        /// Every Permission. (including <see cref="BypassMode"/> and <see cref="ScpOverride"/>.)
        /// </summary>
        All = KeycardO5 | Scp079Override | ScpOverride | BypassMode,
        
        ///<summary>
        /// Janitor Keycard. Containment 1.
        /// </summary>
        KeycardJanitor = ContainmentLevelOne,
        
        /// <summary>
        /// Scientist Keycard. Containment 1 and 2.
        /// </summary>
        KeycardScientist = ContainmentLevelOne | ContainmentLevelTwo,
        
        /// <summary>
        /// Zone Manager Keycard. Containment 1, and Checkpoint Access.
        /// </summary>
        KeycardZoneManager = ContainmentLevelOne | Checkpoints,
        
        /// <summary>
        /// Research Supervisor Keycard. Containment 1, 2 and Checkpoint Access.
        /// </summary>
        KeycardResearchSupervisor = KeycardZoneManager | ContainmentLevelTwo,
        
        /// <summary>
        /// Containment Engineer Keycard. Containment 1 - 3, Warhead, Intercom, and Checkpoint Access.
        /// </summary>
        KeycardContainmentEngineer = KeycardResearchSupervisor | ContainmentLevelThree | AlphaWarhead | Intercom | Checkpoints,
        
        /// <summary>
        /// Facility Manager Keycard. Containment 1 - 3, Warhead, Gate, Intercom, and Checkpoint Access.
        /// </summary>
        KeycardFacilityManager = KeycardContainmentEngineer | ExitGates,
        
        /// <summary>
        /// Guard Keycard. Containment 1, Armory 1, and Checkpoint Access.
        /// </summary>
        KeycardGuard = ContainmentLevelOne | Checkpoints | ArmoryLevelOne,
        
        /// <summary>
        /// MTF Private Keycard. Containment 1, 2, Armory 1, 2, and Checkpoint Access.
        /// </summary>
        KeycardMTFPrivate = KeycardGuard | ContainmentLevelTwo | ArmoryLevelTwo,
        
        /// <summary>
        /// MTF Sergeant Keycard. Containment 1, 2, Armory 1, 2, Gate, and Checkpoint Access.
        /// </summary>
        KeycardMTFSergeant = KeycardMTFPrivate | ExitGates,

        /// <summary>
        /// MTF Captain Keycard. Containment 1, 2, Armory 1 - 3, Gate, Intercom, and Checkpoint Access.
        /// </summary>
        KeycardMTFCaptain = KeycardMTFSergeant | ContainmentLevelThree | ArmoryLevelThree | Intercom,

        /// <summary>
        /// Chaos Keycard. Containment 1, 2, Armory 1 - 3, Gate, Intercom, and Checkpoint Access.
        /// </summary>
        KeycardChaos = KeycardMTFCaptain,
        
        /// <summary>
        /// O5 Keycard. Containment 1 - 3, Armory 1 - 3, Warhead, Gate Intercom, and Checkpoint Access.
        /// </summary>
        KeycardO5 = KeycardChaos | ContainmentLevelThree | AlphaWarhead,
        /// <summary>

        /// <summary>
        /// Opens checkpoints.
        /// </summary>
        Checkpoints = 1,

        /// <summary>
        /// Opens <see cref="DoorType.GateA">Gate A</see> and <see cref="DoorType.GateB">Gate B</see>.
        /// </summary>
        ExitGates = 2,

        /// <summary>
        /// Opens <see cref="DoorType.Intercom">the Intercom door</see>.
        /// </summary>
        Intercom = 4,

        /// <summary>
        /// Opens the Alpha Warhead detonation room on surface.
        /// </summary>
        AlphaWarhead = 8,

        /// <summary>
        /// Opens <see cref="DoorType.Scp914Gate"/>.
        /// </summary>
        ContainmentLevelOne = 16, // 0x0010

        /// <summary>
        /// <see cref="ContainmentLevelOne"/>, <see cref="Checkpoints"/>.
        /// </summary>
        ContainmentLevelTwo = 32, // 0x0020

        /// <summary>
        /// <see cref="ContainmentLevelTwo"/>, <see cref="Intercom"/>, <see cref="AlphaWarhead"/>.
        /// </summary>
        ContainmentLevelThree = 64, // 0x0040

        /// <summary>
        /// <see cref="Checkpoints"/>, Opens Light Containment armory.
        /// </summary>
        ArmoryLevelOne = 128, // 0x0080

        /// <summary>
        /// <see cref="ArmoryLevelOne"/>, <see cref="ExitGates"/>, Opens Heavy Containment armories.
        /// </summary>
        ArmoryLevelTwo = 256, // 0x0100

        /// <summary>
        /// <see cref="ArmoryLevelTwo"/>, <see cref="Intercom"/>, Opens MicroHID room.
        /// </summary>
        ArmoryLevelThree = 512, // 0x0200

        /// <summary>
        /// <see cref="Checkpoints"/>.
        /// </summary>
        ScpOverride = 1024, // 0x0400
        
        /// <summary>
        /// Scp 079's door ability.
        /// </summary>
        Scp079Override = 2048,

        /// <summary>
        /// User has bypass mode enabled.
        /// </summary>
        BypassMode = 4096,
        
}
