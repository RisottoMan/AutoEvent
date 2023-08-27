namespace AutoEvent.Games.Infection
{
    public class GunGameTranslate
    {
        public string GunGameName { get; set; } = "Quick Hands";
        public string GunGameDescription { get; set; } = "Cool GunGame on the Shipment map from MW19.";
        public string GunGameCycle { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<b><color=yellow><color=#D71868>{level}</color> LVL <color=#D71868>||</color> Need <color=#D71868>{kills}</color> kills</color>\n<color=#D71868>Leader: <color=yellow>{leadnick}</color> LVL <color=yellow>{leadlevel}</color></color></b>";
        public string GunGameWinner { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>The Winner of the game: <color=green>{winner}</color></color>";
    }
}