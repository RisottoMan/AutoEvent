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
        public string VoteBeforeStart { get; set; } = "<color=#D71868><b><i>Run Forward</i></b></color>\nInfection starts in: {time}";
        public string VoteCycle { get; set; } = "Vote: Press [Alt] Pros or [Alt]x2 Cons\n{trueCount} of {allCount} players for {newName}\n{time} seconds left!";
        public string VoteMinigameStart { get; set; } = "{newName} will start soon.";
        public string VoteMinigameNotStart { get; set; } = "{newName} will not start.";
        public string VoteEnd { get; set; } = "Vote: End of voting\n{trueCount} of {allCount} players\n{results}";
    }
}