using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoEvent
{
    public class Translation : ITranslation
    {
        [Description("Zombie Infection Game Mode")]
        public string ZombieName { get; set; } = "Zombie Infection";
        public string ZombieDescription { get; set; } = "Zombie mode, the purpose of which is to infect all players.";
        public string ZombieBeforeStart { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=#ABF000>There are <color=red>{time}</color> seconds left before the game starts.</color>";
        public string ZombieCycle { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>Humans left: <color=green>{count}</color></color>\n<color=yellow>Event time <color=red>{time}</color></color>";
        public string ZombieExtraTime { get; set; } = "Extra time: {extratime}\n<color=yellow>The <b><i>Last</i></b> person left!</color>\n<color=yellow>Event time <color=red>{time}</color></color>";
        public string ZombieWin { get; set; } = "<color=red>Zombie Win!</color>\n<color=yellow>Event time <color=red>{time}</color></color>";
        public string ZombieLose { get; set; } = "<color=yellow><color=#D71868><b><i>Humans</i></b></color> Win!</color>\n<color=yellow>Event time <color=red>{time}</color></color>";
        [Description("Atomic Escape Game Mode")]
        public string EscapeName { get; set; } = "Atomic Escape";
        public string EscapeDescription { get; set; } = "Escape from the facility behind SCP-173 at supersonic speed!";
        public string EscapeBeforeStart { get; set; } = "{name}\nHave time to escape from the facility before it explodes!\n<color=red>Before the escape: {time} seconds</color>";
        public string EscapeCycle { get; set; } = "{name}\nBefore the explosion: <color=red>{time}</color> seconds";
        public string EscapeEnd { get; set; } = "{name}\n<color=red> SCP Win </color>";
        [Description("Simon's Prison Game Mode")]
        public string JailName { get; set; } = "Simon's Prison";
        public string JailDescription { get; set; } = "Jail mode from CS 1.6, in which you need to hold events [VERY HARD].";
        public string JailBeforeStart { get; set; } = "<color=yellow><color=red><b><i>{name}</i></b></color>\n<i>Open the doors to the players by shooting the button</i>\nBefore the start: <color=red>{time}</color> seconds</color>";
        public string JailCycle { get; set; } = "<size=20><color=red>{name}</color>\n<color=yellow>Prisoners: {dclasscount}</color> || <color=blue>Jailers: {mtfcount}</color>\n<color=red>{time}</color></size>";
        public string JailPrisonersWin { get; set; } = "<color=red><b><i>Prisoners Win</i></b></color>\n<color=red>{time}</color>";
        public string JailJailersWin { get; set; } = "<color=blue><b><i>Jailers Win</i></b></color>\n<color=red>{time}</color>";
        [Description("Cock Fights Game Mode")]
        public string VersusName { get; set; } = "Cock Fights";
        public string VersusDescription { get; set; } = "Duel of players on the 35hp map from cs 1.6";
        public string VersusPlayersNull { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\nGo inside the arena to fight each other!";
        public string VersusClassDNull { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\nThe player left alive <color=yellow>{scientist}</color>";
        public string VersusScientistNull { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\nThe player left alive <color=orange>{classd}</color>";
        public string VersusPlayersDuel { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow><color=yellow>{scientist}</color> <color=red>VS</color> <color=orange>{classd}</color></color>";
        public string VersusClassDWin { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>WINNERS: <color=red>CLASS D</color></color>";
        public string VersusScientistWin { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>WINNERS: <color=red>SCIENTISTS</color></color>";
        [Description("Knives of Death Game Mode")]
        public string KnivesName { get; set; } = "Knives of Death";
        public string KnivesDescription { get; set; } = "Knife players against each other on a 35hp map from cs 1.6";
        public string KnivesCycle { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow><color=blue>{mtfcount} MTF</color> <color=red>VS</color> <color=green>{chaoscount} CHAOS</color></color>";
        public string KnivesChaosWin { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>WINNERS: <color=green>CHAOS</color></color>";
        public string KnivesMtfWin { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>WINNERS: <color=#42AAFF>MTF</color></color>";
        [Description("Deathmatch Game Mode")]
        public string DeathmatchName { get; set; } = "Territory of Death";
        public string DeathmatchDescription { get; set; } = "Cool Deathmatch on the Shipment map from MW19";
        public string DeathmatchCycle { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<b><color=yellow><color=#42AAFF> {mtftext}> </color> <color=red>|</color> <color=green> <{chaostext}</color></color></b>";
        public string DeathmatchChaosWin { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>WINNERS: <color=green>CHAOS</color></color>";
        public string DeathmatchMtfWin { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>WINNERS: <color=#42AAFF>MTF</color></color>";
        [Description("GunGame Game Mode")]
        public string GunGameName { get; set; } = "Quick Hands";
        public string GunGameDescription { get; set; } = "Cool GunGame on the Shipment map from MW19.";
        public string GunGameCycle { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<b><color=yellow><color=#D71868>{level}</color> LVL <color=#D71868>||</color> Need <color=#D71868>{kills}</color> kills</color></b>";
        public string GunGameWinner { get; set; } = "<color=#D71868><b><i>{name}</i></b></color>\n<color=yellow>The Winner of the game: <color=green>{winner}</color></color>";
    }
}
