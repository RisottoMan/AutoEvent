using HarmonyLib;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.GetPlayers))]
    public class PlayerList
    {
        public static bool Prefix<Player>(Player __instance , ref List<Player> __result)
        {
            __result = new List<Player>();
            return false;
        }
    }
}
