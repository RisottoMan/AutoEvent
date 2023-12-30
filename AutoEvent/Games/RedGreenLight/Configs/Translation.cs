using Exiled.API.Interfaces;

namespace AutoEvent.Games.Light
{
#if EXILED
    public class LightTranslation : ITranslation 
#else
    public class LightTranslation
#endif
    {
        public string LightCommandName { get; set; } = "light";
        public string LightName { get; set; } = "Red Light Green Light";
        public string LightDescription { get; set; } = "Defeat the enemy with snowballs.";
        public string LightStart { get; set; } = "<color=#42aaff>Get ready to take a snowball</color>\nThe game start in: <color=red>{time}</color>";
        public string LightCycle { get; set; } = "<b><color=#42aaff>{name}</color></b>\n<color=#aad9ff>Throw snowballs at the players</color>\n<color=red>{time}</color>";
        public string LightAllDied { get; set; } = "<b><color=#42aaff>{name}</color></b>\n<color=red>All players died</color>\nTime: <color=red>{time}</color>";
        public string LightClassDWin { get; set; } = "<b><color=#42aaff>{name}</color></b>\n<color=#ffa500>ClassD's Win</color>\nTime: <color=red>{time}</color>";
        public string LightScientistWin { get; set; } = "<b><color=#42aaff>{name}</color></b>\n<color=#ffff00>Scientists Win</color>\nTime: <color=red>{time}</color>";
        public string LightDraw { get; set; } = "<b><color=#42aaff>{name}</color></b>\n<color=#808080>Draw</color>\nTime: <color=red>{time}</color>";
        public string LightRedline { get; set; } = "<color=red>You can't go through the red line</color>";
    }
}