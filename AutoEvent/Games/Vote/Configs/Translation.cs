using AutoEvent.Interfaces;

namespace AutoEvent.Games.Vote;
public class Translation : EventTranslation
{
    public override string Name { get; set; } = "Vote";
    public override string Description { get; set; } = "Start voting for the mini-game";
    public override string CommandName { get; set; } = "vote";
    public string Cycle { get; set; } = "<b><color=yellow>Voting for <color=#ffa500>{newName}</color></b>\n" +
                                        "Press <color=#90ee90>[Alt] Pros</color> or <color=#c93c20>[Alt] x2 Cons</color>\n" +
                                        "Count: <b>{trueCount}/{allCount}</b> | Time: <b><color=red>{time}</color></b>";
    public string EndOfVoting { get; set; } = "<b><color=yellow>The voting is over</b>\n" +
                                              "<b>{trueCount}</b> of <b>{allCount}</b> players voted</color>\n" +
                                              "{result}";
    public string StartResult { get; set; } = "<color=yellow>Mini-Game <color=#ffa500>{newName}</color> is starting!</color>";
    public string NotStartResult { get; set; } = "<color=yellow>The players voted against the mini-game <color=red>{newName}</color></color>";
}