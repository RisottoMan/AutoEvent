namespace AutoEvent.Games.Infection
{
    public class DeathTranslate
    {
        public string DeathName { get; set; } = "Death Party";
        public string DeathDescription { get; set; } = "Survive in grenade rain.";
        public string DeathCycle { get; set; } = "<color=yellow>Dodge the grenades!</color>\n<color=green>%time% seconds passed</color>\n<color=red>%count% players left</color>";
        public string DeathMorePlayer { get; set; } = "<color=red>Death Party</color>\n<color=yellow><color=red>%count%</color> players survived.</color>\n<color=#ffc0cb>%time%</color>";
        public string DeathOnePlayer { get; set; } = "<color=red>Death Party</color>\n<color=yellow>Winner - <color=red>%winner%</color></color>\n<color=#ffc0cb>%time%</color>";
        public string DeathAllDie { get; set; } = "<color=red>Death Party</color>\n<color=yellow>No one survived.(((</color>\n<color=#ffc0cb>%time%</color>";
    }
}