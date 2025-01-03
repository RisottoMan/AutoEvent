using AutoEvent.Interfaces;

namespace AutoEvent.Games.Survival;
public class Translation : EventTranslation
{
    public string SurvivalBeforeInfection { get; set; } = "<b>{name}</b>\n<color=yellow>There are </color> {time} <color=yellow>second before infection begins</color>";
    public string SurvivalAfterInfection { get; set; } = "<b>{name}</b>\n<color=#14AAF5>Humans:</color> {humanCount}\n<color=#299438>Time remaining:</color> {time}";
    public string SurvivalZombieWin { get; set; } = "<color=red>Zombie infected all humans and wins!</color>";
    public string SurvivalHumanWin { get; set; } = "<color=#14AAF5>Humans killed all zombies and stopped infection</color>";
    public string SurvivalHumanWinTime { get; set; } = "<color=yellow>Humans survived, but didn't stop the infection</color>";
}