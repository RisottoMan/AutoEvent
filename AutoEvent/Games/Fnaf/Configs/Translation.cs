using Exiled.API.Interfaces;

namespace AutoEvent.Games.Fnaf
{
#if EXILED
    public class FnafTranslation : ITranslation 
#else
    public class FnafTranslation
#endif
    {
        public string ChairsCommandName { get; set; } = "chair";
        public string ChairsName { get; set; } = "Musical Chairs";
        public string ChairsDescription { get; set; } = "Competition with other players for free chairs to funny music";
        public string ChairsStart { get; set; } = "<color=#42aaff>Get ready to run</color>\nThe game start in: <color=red>{time}</color>";
        public string ChairsCycle { get; set; } = "<b><color=#42aaff>{name}</color></b>\n<color=#ffff00>{state}</color>\n<color=#008000>{count}</color> players remaining</color>";
        public string ChairsRunDontTouch { get; set; } = "Run and dont touch the platforms";
        public string ChairsStandFree { get; set; } = "Stand on a free platform";
        public string ChairsStopRunning { get; set; } = "<color=red>You stopped running</color>";
        public string ChairsTouchAhead { get; set; } = "<color=red>You touched the platforms ahead of time</color>";
        public string ChairsNoTime { get; set; } = "<color=red>You didnt stand on a free platform in time</color>";
        public string ChairsMorePlayers { get; set; } = "<b><color=#42aaff>{name}</color></b>\n<color=red>The admin canceled the game</color>";
        public string ChairsWinner { get; set; } = "<b><color=#42aaff>{name}</color></b>\n<color=red>Winner: {winner}</color>";
        public string ChairsAllDied { get; set; } = "<b><color=#42aaff>{name}</color></b>\n<color=red>All players died</color>";
    }
}