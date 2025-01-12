using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using HarmonyLib;

namespace AutoEvent.Patches;

[HarmonyPatch(typeof(Player), nameof(Player.List), MethodType.Getter)]
public class PlayerList
{
    public static void Postfix(ref IReadOnlyCollection<Player> __result)
    {
        if (AutoEvent.EventManager.CurrentEvent is null)
            return;

        if (AutoEvent.Singleton.Config.IgnoredRoles is null || AutoEvent.Singleton.Config.IgnoredRoles.Count == 0)
            return;
        
        __result = Player.Dictionary.Values.Where(x => !AutoEvent.Singleton.Config.IgnoredRoles.Contains(x.Role)).ToList();
        
        // Display bots in the Exiled.List for testing
        if (!AutoEvent.Singleton.Config.Debug)
        {
            __result = __result.Where(x => !x.IsNPC).ToList();
        }
    }
}