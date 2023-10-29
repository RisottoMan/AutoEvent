// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         Translation.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/28/2023 1:52 AM
//    Created Date:     10/28/2023 1:52 AM
// -----------------------------------------

using Exiled.API.Interfaces;

namespace AutoEvent.Games.GhostBusters.Configs;


#if EXILED
public class GhostBustersTranslate : ITranslation
#else
    public class GhostBustersTranslate
#endif
{
    public string GhostBustersCommandName { get; set; } = "ghosts"; // Soccer in america - football everywhere else 🦅🦅🦅🇺🇸🇺🇸🇺🇸 <- (USA Flag doesnt render in rider...)
    public string GhostBustersName { get; set; } = "Ghost Busters <color=purple>[Halloween]</color>"; // Soccer in america - football everywhere else 🦅🦅🦅🇺🇸🇺🇸🇺🇸 <- (USA Flag doesnt render in rider...)
    public string GhostBustersDescription { get; set; } = "Ghostbusters vs ghosts. The clock is ticking, will the ghost-busters be able to kill all ghosts before midnight hits?";
    public string GhostBustersStartGhostMessage { get; set; } = "<color=#D71868>You're a <b><color=#a9a9a9>Ghost!</color></b>\n<color=#ff4500><b>Run</b></color> and <color=#6495ed><b>Hide</b></color> from the <color=#cc0000><b>Ghost Busters!</color></b> They will try and <color=red>take you out!</color>\n<color=#ff8c00>Select a role via your <b>inventory menu.</b></color> \n<color=#ff8c00>You will be given a <b>powerup</b> that can be accessed in you <b>inventory menu.</b></color>";
    public string GhostBustersStartHunterMessage { get; set; } = "You're a <color=#cc0000><b>Ghost-Buster!</color></b>\nFind all the <b><color=#a9a9a9>ghosts</color></b> before it is too late!\nYou can select a role in your <b>inventory menu.</b> ";
    public string GhostBustersRunning { get; set; } = "Time Until Midnight: <b>{time}</b>\nGhosts Alive: <b><color=#a9a9a9>{ghosts}</color>,</b> <color=#cc0000>Ghost-Busters</color> Alive:<b> {hunters}</b>";
    public string GhostBustersMidnightGhostMessage { get; set; } = "<b><color=#800000>Midnight Has Hit</color></b>\nFind and <b><color=#cd0000>kill</color></b> the <color=#cc0000><b>Ghost Busters!</color></b>\n{time}";
    public string GhostBustersMidnightHunterMessage { get; set; } = "<b><color=#800000>Midnight Has Hit</color></b>\n<b>Run</b> and <b>hide</b> for your life! The ghosts are after you!\n{time}";
    public string GhostBustersGhostsWin { get; set; } = "<b><color=#a9a9a9>Ghost Win</b></color>\nThe <b><color=#a9a9a9>ghosts</b></color> have killed all <color=#cc0000><b>hunters.</color></b>";

    public string GhostBustersHuntersWin { get; set; } = "<color=#cc0000><b>Ghost-Buster Win</color></b>\nThe <color=#cc0000><b>Ghost-Busters</color></b> have managed to exterminate all of the <b><color=#a9a9a9>ghosts.</color></b>";
    public string GhostBustersTie { get; set; } = "<color=#cc0000><b>Ghost-Buster Win</color></b>\nThe <color=#cc0000><b>Ghost-Busters</color></b> have managed to <color=#008000>survive the night.</color>";
    }