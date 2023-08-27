namespace AutoEvent.Games.Infection
{
    public class PuzzleTranslate
    {
        public string PuzzleName { get; set; } = "Puzzle";
        public string PuzzleDescription { get; set; } = "Get up the fastest on the right color.";
        public string PuzzleStart { get; set; } = "<color=red>Starts in: </color>%time%";
        public string PuzzleStage { get; set; } = "<color=red>Stage: </color>%stageNum%<color=red> / </color>%stageFinal%\n<color=yellow>Remaining players:</color> %plyCount%";
        public string PuzzleAllDied { get; set; } = "<color=red>All players died</color>\nMini-game ended";
        public string PuzzleSeveralSurvivors { get; set; } = "<color=red>Several people survived</color>\nMini-game ended";
        public string PuzzleWinner { get; set; } = "<color=red>Winner: %plyWinner%</color>\nMini-game ended";
        public string PuzzleDied { get; set; } = "<color=red>Burned in Lava</color>";
    }
}