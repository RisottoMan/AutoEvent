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
    public string GhostBustersName { get; set; } = "Ghost Busters"; // Soccer in america - football everywhere else 🦅🦅🦅🇺🇸🇺🇸🇺🇸 <- (USA Flag doesnt render in rider...)
    public string GhostBustersDescription { get; set; } = "Ghostbusters vs ghosts. The clock is ticking, will the ghost-busters be able to kill all ghosts before midnight hits?";
    public string GhostBustersStartGhostMessage { get; set; } = "You're a Ghost!\nRun and Hide from the Ghost Busters! They will try and take you out!";
    public string GhostBustersStartHunterMessage { get; set; } = "You're a Ghost-Buster!\nFind all the ghosts before it is too late!";
    public string GhostBustersMidnightGhostMessage { get; set; } = "Midnight Has Hit\nFind and kill the Ghost Busters!";
    public string GhostBustersMidnightHunterMessage { get; set; } = "Midnight Has Hit\nRun and hide for your life! The ghosts are after you!";
    public string GhostBustersGhostsWin { get; set; } = "Ghost Win\nThe ghosts have killed all hunters.";
    public string GhostBustersHuntersWin { get; set; } = "Ghost-Buster Win\nThe Ghost-Busters have exterminated all of the ghosts.";
    }