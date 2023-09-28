using Exiled.API.Interfaces;

namespace AutoEvent.Games.Infection
{
#if EXILED
    public class LavaTranslate : ITranslation 
#else
    public class LavaTranslate 
#endifL
    {
        public string LavaCommandName { get; set; } = "lava";
        public string LavaName { get; set; } = "The floor is LAVA";
        public string LavaDescription { get; set; } = "Survival, in which you need to avoid lava and shoot at others.";
        public string LavaBeforeStart { get; set; } = "<size=100><color=red>%time%</color></size>\nTake weapons and climb up.";
        public string LavaCycle { get; set; } = "<size=20><color=red><b>Alive: %count% players</b></color></size>";
        public string LavaWin { get; set; } = "<color=red><b>Winner\nPlayer - %winner%</b></color>";
        public string LavaAllDead { get; set; } = "<color=red><b>No one survived to the end.</b></color>";
    }
}