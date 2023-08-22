using HarmonyLib;
using PluginAPI.Core;
using System.Collections.Generic;
using System.Linq;

namespace AutoEvent.Patches
{
    //[HarmonyPatch(typeof(Player), nameof(Player.GetPlayers))]
    public class PlayerList
    {
        // надо исправить этот патч. Он должен убирать из списка игроков нашего бота, чтобы его не спавнило
        // вся игровая логика проверяется через этот метод, поэтому убрать бота отсюда логично
        public static bool Prefix(Player __instance, ref List<Player> __result) // попробовать переписать???
        {
            List<Player> newList = new List<Player>();

            foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
            {
                if (!allHub.isLocalPlayer || !Extensions.IsAudioBot(allHub))
                {
                    newList.Add(Player.Get(allHub));
                }
            }

            __result = newList;

            return false;
        }
    }
}
