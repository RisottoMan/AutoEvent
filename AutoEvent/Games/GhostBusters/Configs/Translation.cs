using AutoEvent.Interfaces;

namespace AutoEvent.Games.GhostBusters;

public class Translation : EventTranslation
{
    public override string Name { get; set; } = "Ghost Busters";
    public override string Description { get; set; } = "Ghostbusters vs ghosts. The clock is ticking, will the ghost-busters be able to kill all ghosts before midnight hits?";
    public override string CommandName { get; set; } = "ghosts";
    public string StartGhostMessage { get; set; } = "<color=#D71868>You're a <b><color=#a9a9a9>Ghost!</color></b>\n<color=#ff4500><b>Run</b></color> and <color=#6495ed><b>Hide</b></color> from the <color=#cc0000><b>Ghost Busters!</color></b> They will try and <color=red>take you out!</color>\n<color=#ff8c00>Select a role via your <b>inventory menu.</b></color> \n<color=#ff8c00>You will be given a <b>powerup</b> that can be accessed in you <b>inventory menu.</b></color>";
    public string StartHunterMessage { get; set; } = "You're a <color=#cc0000><b>Ghost-Buster!</color></b>\nFind all the <b><color=#a9a9a9>ghosts</color></b> before it is too late!\nYou can select a role in your <b>inventory menu.</b> ";
    public string Running { get; set; } = "Time Until Midnight: <b>{time}</b>\nGhosts Alive: <b><color=#a9a9a9>{ghosts}</color>,</b> <color=#cc0000>Ghost-Busters</color> Alive:<b> {hunters}</b>";
    public string MidnightGhostMessage { get; set; } = "<b><color=#800000>Midnight Has Hit</color></b>\nFind and <b><color=#cd0000>kill</color></b> the <color=#cc0000><b>Ghost Busters!</color></b>\n{time}";
    public string MidnightHunterMessage { get; set; } = "<b><color=#800000>Midnight Has Hit</color></b>\n<b>Run</b> and <b>hide</b> for your life! The ghosts are after you!\n{time}";
    public string GhostsWin { get; set; } = "<b><color=#a9a9a9>Ghost Win</b></color>\nThe <b><color=#a9a9a9>ghosts</b></color> have killed all <color=#cc0000><b>hunters.</color></b>";
    public string HuntersWin { get; set; } = "<color=#cc0000><b>Ghost-Buster Win</color></b>\nThe <color=#cc0000><b>Ghost-Busters</color></b> have managed to exterminate all of the <b><color=#a9a9a9>ghosts.</color></b>";
    public string Tie { get; set; } = "<color=#cc0000><b>Ghost-Buster Win</color></b>\nThe <color=#cc0000><b>Ghost-Busters</color></b> have managed to <color=#008000>survive the night.</color>";
}