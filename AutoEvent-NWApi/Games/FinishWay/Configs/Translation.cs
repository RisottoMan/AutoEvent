namespace AutoEvent.Games.Infection
{
    public class FinishWayTranslate
    {
        public string FinishWayName { get; set; } = "Race";
        public string FinishWayDescription { get; set; } = "Get to the end of the map to win.";
        public string FinishWayCycle { get; set; } = "%name%\n<color=yellow>Pass the finishing point to win!</color>\nTime Remaining: %time%";
        public string FinishWayDied { get; set; } = "You didnt finish the race.";
        public string FinishWaySeveralSurvivors { get; set; } = "<color=red>Human wins!</color>\nSurvived %count%";
        public string FinishWayOneSurvived { get; set; } = "<color=red>Human wins!</color>\nWinner: %player%";
        public string FinishWayNoSurvivors { get; set; } = "<color=red>Nobody survived!</color>";
    }
}