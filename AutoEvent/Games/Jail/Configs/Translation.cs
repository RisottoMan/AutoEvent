using AutoEvent.Interfaces;

namespace AutoEvent.Games.Jail;
public class Translation : EventTranslation
{
    public string Start { get; set; } = "<color=yellow><color=red><b><i>{name}</i></b></color>\n<i>Trigger or release a lockdown by shooting the big button</i>\nBefore the start: <color=red>{time}</color> seconds</color>";
    public string StartPrisoners { get; set; } = "<color=yellow><color=red><b><i>{name}</i></b></color>\n<i>Escape the prison at any cost or take it over. Be careful. Items are scattered around the map to help you survive.</i>\nBefore the start: <color=red>{time}</color> seconds</color>";
    public string Cycle { get; set; } = "<size=20><color=red>{name}</color>\n<color=yellow>Prisoners: {dclasscount}</color> || <color=#14AAF5>Jailers: {mtfcount}</color>\n<color=red>{time}</color></size>";
    public string PrisonersWin { get; set; } = "<color=red><b><i>Prisoners Win</i></b></color>\n<color=red>{time}</color>";
    public string JailersWin { get; set; } = "<color=#14AAF5><b><i>Jailers Win</i></b></color>\n<color=red>{time}</color>";
    public string LockdownOnCooldown { get; set; } = "You cannot trigger a lockdown, while it is on cooldown. Cooldown Remaining: {cooldown}";
    public string LivesRemaining { get; set; } = "You have {lives} lives remaining.";
    public string NoLivesRemaining { get; set; } = "You have no more lives remaining.";
}