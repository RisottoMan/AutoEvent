using Exiled.API.Interfaces;

namespace AutoEvent.Games.Snowball
{
#if EXILED
    public class SnowballTranslation : ITranslation 
#else
    public class SnowballTranslation
#endif
    {
        public string SnowballCommandName { get; set; } = "snow";
        public string SnowballName { get; set; } = "Snowball";
        public string SnowballDescription { get; set; } = "Defeat the enemy with snowballs.";
        public string SnowballStart { get; set; } = "<color=#42aaff>Get ready to take a snowball</color>\nThe game start in: <color=red>{time}</color>";
        public string SnowballCycle { get; set; } = "<b><color=#42aaff>{name}</color></b>\n<color=#aad9ff>Throw snowballs at the players</color>\n<color=red>{time}</color>";
        public string SnowballAllDied { get; set; } = "<b><color=#42aaff>{name}</color></b>\n<color=red>All players died</color>\nTime: <color=red>{time}</color>";
        public string SnowballClassDWin { get; set; } = "<b><color=#42aaff>{name}</color></b>\n<color=#ffa500>ClassD's Win</color>\nTime: <color=red>{time}</color>";
        public string SnowballScientistWin { get; set; } = "<b><color=#42aaff>{name}</color></b>\n<color=#ffff00>Scientists Win</color>\nTime: <color=red>{time}</color>";
        public string SnowballDraw { get; set; } = "<b><color=#42aaff>{name}</color></b>\n<color=#808080>Draw</color>\nTime: <color=red>{time}</color>";
        public string SnowballRedline { get; set; } = "<color=red>You can't go through the red line</color>";
    }
}