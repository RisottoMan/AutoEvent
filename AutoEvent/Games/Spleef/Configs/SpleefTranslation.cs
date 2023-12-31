namespace AutoEvent.Games.Spleef.Configs;

public class SpleefTranslation
{
    public string SpleefCommandName { get; set; } = "spleef";
    public string SpleefName { get; set; } = "Spleef";
    public string SpleefDescription { get; set; } = "Shoot at the platforms and don't fall into the void!";
    public string SpleefStart { get; set; } = "<color=red>Starts in: </color>{time}";
    public string SpleefRunning { get; set; } = "Players Alive: {players}\nTime remaining: {remaining}";
    public string SpleefAllDied { get; set; } = "<color=red>All players died</color>\nMini-game ended";
    public string SpleefSeveralSurvivors { get; set; } = "<color=red>Several people survived</color>\nMini-game ended";
    public string SpleefWinner { get; set; } = "<color=red>Winner: {winner}</color>\nMini-game ended";
    public string SpleefDied { get; set; } = "<color=red>Burned in Lava</color>";
}