using AutoEvent.Interfaces;

namespace AutoEvent.Games.Deathrun;
public class Translation : EventTranslation
{
    public string BeforeStartBroadcast { get; set; } = "<color=#FFC618>{name}</color>\n" +
                                                       "<size=35><color=#ff4c5b>Warning! There are deadly traps ahead</color>\n" +
                                                       "<color=#20B2AA>Get ready to run in <b>{time}</b> seconds</color></size>";
    public string CycleBroadcast { get; set; } = "<color=#FFC618>{name}</color>\n" +
                                                 "<size=35><color=yellow>😃 Runners [{runnerCount}]</color> | <color=#ff4c5b>[{deathCount}] Death 😈</color>\n" +
                                                 "<color=#20B2AA>Time Left: <b>{time}</b></color></size>";
    public string DeathWinBroadcast { get; set; } = "<color=#FFC618>{name}</color>\n<b><color=#ff4c5b>Death win</color></b>";
    public string RunnerWinBroadcast { get; set; } = "<color=#FFC618>{name}</color>\n<b><color=yellow>Runners win</color></b>";
    public string Died { get; set; } = "<color=red>You didn't have time to kill the death</color>";
    public string OverTimeBroadcast { get; set; } = "<b><color=red>Time to kill <i>Death</i> 😸</color></b>";
    public string SecondLifeHint { get; set; } = "<b><color=green>You've got a second life.</color></b>";
}