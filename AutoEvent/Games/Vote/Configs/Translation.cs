using AutoEvent.Interfaces;

namespace AutoEvent.Games.Vote;

public class Translation : EventTranslation
{
    public override string Name { get; set; } = "Vote";
    public override string Description { get; set; } = "Start voting for the mini-game";
    public override string CommandName { get; set; } = "vote";
    public string VoteBeforeStart { get; set; } = "<color=#D71868><b><i>Run Forward</i></b></color>\nInfection starts in: {time}";
    public string VoteCycle { get; set; } = "Vote: Press [Alt] Pros or [Alt]x2 Cons\n{trueCount} of {allCount} players for {newName}\n{time} seconds left!";
    public string VoteMinigameStart { get; set; } = "{newName} will start soon.";
    public string VoteMinigameNotStart { get; set; } = "{newName} will not start.";
    public string VoteEnd { get; set; } = "Vote: End of voting\n{trueCount} of {allCount} players\n{results}";
}