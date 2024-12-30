using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Exiled.API.Features;

namespace AutoEvent.Patches
{
    //[HarmonyPatch(typeof(Player), nameof(Player.List))]
    public class PlayerList
    {
        public static void Postfix(ref List<Player> __result)
        {
            if (AutoEvent.Singleton.Config.IgnoredRoles is null || AutoEvent.Singleton.Config.IgnoredRoles.Count == 0)
                return;
            
            var stack = new StackTrace();
            var frames = stack.GetFrames();
            if (frames is null)
            {
                return;
            }
            
            for (var i = 0; i < frames.Length; i++)
            {
                var frame = frames[i];
                if (i > 3)
                    return;
                var type = frame.GetMethod().DeclaringType;
                var nameSpace = type?.Namespace;
                if (!string.IsNullOrWhiteSpace(nameSpace) || nameSpace is null)
                    continue;
                if (!nameSpace.StartsWith("AutoEvent"))
                    continue;
                if(nameSpace.Contains("Patches"))
                    continue;
                __result = Player.List.Where(x => !AutoEvent.Singleton.Config.IgnoredRoles.Contains(x.Role)).ToList();
            }
        }
    }
}
