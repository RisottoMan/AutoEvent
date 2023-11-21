using Exiled.API.Interfaces;
using static Exiled.Loader.Features.MultiAdminFeatures;
using System.Reflection;

namespace AutoEvent.Games.AllDeathmatch
{
#if EXILED
    public class AllTranslation  : ITranslation 
#else
    public class AllTranslation
#endif
    {
        public string AllCommandName { get; set; } = "dm";
        public string AllName { get; set; } = "All Deathmatch";
        public string AllDescription { get; set; } = "Fight against each other in all deathmatch.";
        public string AllCycle { get; set; } = "<size=30><i><b>{name}</b>\n<color=red>You - {kills}/{needKills} kills</color>\nRound Time: {time}</i></size>";
        public string AllNoPlayers { get; set; } = "<color=red>The game is over by the admin\nYour kills {count}</color>";
        public string AllTimeEnd { get; set; } = "<color=red>The game is over in time\nYour kills {count}</color>";
        public string AllWinnerEnd { get; set; } = "<b><color=red>Winner - <color=yellow>{winner}</color></color></b>\nYour kills <color=red>{count}</color></color>\nGame Time - <color=#008000>{time}</color></i>";
    }
}