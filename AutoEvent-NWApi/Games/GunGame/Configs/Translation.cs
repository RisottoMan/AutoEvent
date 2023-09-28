using Exiled.API.Interfaces;

namespace AutoEvent.Games.Infection
{
#if EXILED
    public class GunGameTranslate : ITranslation 
#else
    public class GunGameTranslate
#endif
    {
        public string GunGameCommandName { get; set; } = "gungame";
        public string GunGameName { get; set; } = "Gun Game";
        public string GunGameDescription { get; set; } = "Cool GunGame on the Shipment map from MW19.";
        public string GunGameCycle { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<b><color=yellow>Gun <color=#D71868>{gun}</color> <color=#D71868>||</color> Need <color=#D71868>{kills}</color> kills</color>\n<color=#D71868>Leader: <color=yellow>{leadnick}</color> LVL <color=yellow>{leadgun}</color></color></b>";
        public string GunGameWinner { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>The Winner of the game: <color=green>{winner}</color></color>";
    }
}