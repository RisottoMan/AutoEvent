using AutoEvent.Interfaces;

namespace AutoEvent.Games.Dodgeball;
public class Translation : EventTranslation
{
    public string Start { get; set; } = "<color=#ffff00>Get ready to take a ball</color>\nThe game start in: <color=red>{time}</color>";
    public string Cycle { get; set; } = "<b><color=#ffff00>{name}</color></b>\n<color=#aad9ff>Throw balls at the players</color>\n<color=red>{time}</color>";
    public string AllDied { get; set; } = "<b><color=#ffff00>{name}</color></b>\n<color=red>All players died</color>\nTime: <color=red>{time}</color>";
    public string ClassDWin { get; set; } = "<b><color=#ffff00>{name}</color></b>\n<color=#ffa500>ClassD's Win</color>\nTime: <color=red>{time}</color>";
    public string ScientistWin { get; set; } = "<b><color=#ffff00>{name}</color></b>\n<color=#ffff00>Scientists Win</color>\nTime: <color=red>{time}</color>";
    public string Draw { get; set; } = "<b><color=#ffff00>{name}</color></b>\n<color=#808080>Draw</color>\nTime: <color=red>{time}</color>";
    public string Redline { get; set; } = "<color=red>You can't go through the red line</color>";
    public string Knocked { get; set; } = "<color=red>You were knocked out by {killer}</color>";
}