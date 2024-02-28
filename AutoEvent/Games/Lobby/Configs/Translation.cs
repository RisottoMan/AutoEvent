using AutoEvent.Interfaces;

namespace AutoEvent.Games.Lobby;
public class Translation : EventTranslation
{
    public override string Name { get; set; } = "Lobby";
    public override string Description { get; set; } = "A lobby in which one quick player chooses a mini-game.";
    public override string CommandName { get; set; } = "lobby";
    public string Cycle { get; set; } = "Vote: Press [Alt] Pros or [Alt]x2 Cons\n{trueCount} of {allCount} players for {newName}\n{time} seconds left!";
    public string GlobalMessage { get; set; } = "Lobby\n{message}\n{count} players in the lobby";
    public string GetReady { get; set; } = "Get ready to run to the center and choose a mini game";
    public string Run { get; set; } = "RUN";
    public string Choosing { get; set; } = "Waiting for the {nickName} to choose a mini-game";
    public string FinishMessage { get; set; } = "The lobby is finished.\nThe player {nickName} chose the {newName} mini-game.\nTotal {count} players in the lobby";
}