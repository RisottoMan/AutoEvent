using Exiled.API.Interfaces;

namespace AutoEvent.Games.Infection
{
#if EXILED
    public class DeathmatchTranslate  : DeITranslation 
#else
    public class DeathmatchTranslate 
#endif
    {
        public string DeathmatchCommandName { get; set; } = "tdm";
        public string DeathmatchName { get; set; } = "Team Death-Match";
        public string DeathmatchDescription { get; set; } = "Team Death-Match on the Shipment map from MW19";
        public string DeathmatchCycle { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<b><color=yellow><color=#42AAFF> {mtftext}> </color> <color=red>|</color> <color=green> <{chaostext}</color></color></b>";
        public string DeathmatchChaosWin { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>WINNERS: <color=green>CHAOS</color></color>";
        public string DeathmatchMtfWin { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>WINNERS: <color=#42AAFF>MTF</color></color>";
    }
}