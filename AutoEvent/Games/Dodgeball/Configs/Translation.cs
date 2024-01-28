using Exiled.API.Interfaces;

namespace AutoEvent.Games.Dodgeball
{
#if EXILED
    public class DodgeballTranslation : ITranslation 
#else
    public class DodgeballTranslation
#endif
    {
        public string DodgeballCommandName { get; set; } = "dodge";
        public string DodgeballName { get; set; } = "Dodgeball";
        public string DodgeballDescription { get; set; } = "Defeat the enemy with balls.";
        public string DodgeballStart { get; set; } = "<color=#ffff00>Get ready to take a ball</color>\nThe game start in: <color=red>{time}</color>";
        public string DodgeballCycle { get; set; } = "<b><color=#ffff00>{name}</color></b>\n<color=#aad9ff>Throw balls at the players</color>\n<color=red>{time}</color>";
        public string DodgeballAllDied { get; set; } = "<b><color=#ffff00>{name}</color></b>\n<color=red>All players died</color>\nTime: <color=red>{time}</color>";
        public string DodgeballClassDWin { get; set; } = "<b><color=#ffff00>{name}</color></b>\n<color=#ffa500>ClassD's Win</color>\nTime: <color=red>{time}</color>";
        public string DodgeballScientistWin { get; set; } = "<b><color=#ffff00>{name}</color></b>\n<color=#ffff00>Scientists Win</color>\nTime: <color=red>{time}</color>";
        public string DodgeballDraw { get; set; } = "<b><color=#ffff00>{name}</color></b>\n<color=#808080>Draw</color>\nTime: <color=red>{time}</color>";
        public string DodgeballRedline { get; set; } = "<color=red>You can't go through the red line</color>";
    }
}