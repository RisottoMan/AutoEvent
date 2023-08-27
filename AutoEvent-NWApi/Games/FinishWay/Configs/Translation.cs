namespace AutoEvent.Games.Infection
{
    public class FinishWayTranslate
    {
        public string FinishWayName { get; set; } = "Finish Way";
        public string FinishWayDescription { get; set; } = "Go to the end of the finish to win.";
        public string FinishWayCycle { get; set; } = "%name%\n<color=yellow>Pass the finish!</color>\nTime left: %time%";
        public string FinishWayDied { get; set; } = "You didnt pass the finish";
        public string FinishWaySeveralSurvivors { get; set; } = "<color=red>Human wins!</color>\nSurvived %count%";
        public string FinishWayOneSurvived { get; set; } = "<color=red>Human wins!</color>\nWinner: %player%";
        public string FinishWayNoSurvivors { get; set; } = "<color=red>No one human survived</color>";
    }
}