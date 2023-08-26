using HarmonyLib;
using PluginAPI.Core;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(Player))]
    public class PlayerList
    {
        public static MethodInfo TargetMethod()
        {
            return typeof(Player).GetMethods().First(r => r.Name == "GetPlayers");
        }

        public static void Postfix(ref List<Player> __result)
        {
            if (Extensions.AudioBot == null) return;

            Player dummy = Player.Get(Extensions.AudioBot);
            __result.Remove(dummy);
        }
    }
}
