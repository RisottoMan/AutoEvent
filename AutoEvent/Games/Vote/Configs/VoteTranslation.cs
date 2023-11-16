using Exiled.API.Interfaces;

namespace AutoEvent.Games.Vote
{
#if EXILED
    public class VoteTranslation : ITranslation 
#else
    public class VoteTranslation
#endif
    {
        public string VoteCommandName { get; set; } = "vote";
        public string VoteName { get; set; } = "Vote";
        public string VoteDescription { get; set; } = "Start voting for the mini-game.";
        public string VoteBeforeStart { get; set; } = "<color=#D71868><b><i>Run Forward</i></b></color>\nInfection starts in: <color=yellow>{time}</color>";
        public string VoteCycle { get; set; } = "<color=#1378f2>Vote: Press [Alt] Pros or [Alt]x2 Cons</color>\n<color=yellow>{trueCount}</color> <color=#1378f2>of</color> <color=yellow>{allCount}</color> <color=#1378f2>players for {newName}</color>\n<color=yellow>{time}</color> <color=#1378f2>seconds left!</color>";
        public string VoteMinigameStart { get; set; } = "<b><color=yellow>{newName} will start soon.</color></b>";
        public string VoteMinigameNotStart { get; set; } = "<b><color=red>{newName} will not start.</color></b>";
        public string VoteEnd { get; set; } = "<color=#1378f2>Vote: End of voting</color>\n<color=yellow>{trueCount}</color> <color=#1378f2>of</color> <color=yellow>{allCount}</color> <color=#1378f2>players</color>\n<color=yellow>{results}</color>";
    }
}