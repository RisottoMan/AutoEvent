using HarmonyLib;
using PluginAPI.Core;
using RemoteAdmin;
using System;

namespace AutoEvent
{
    internal class RemoteAdminPatch
    {
        public static Config config = AutoEvent.Singleton.Config;

        [HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery))]
        public static class RemoteAdmin
        {
            static bool Prefix(string q, CommandSender sender)
            {
                try
                {
                    if (AutoEvent.ActiveEvent == null) return true;
                    if (!config.IsDisableDonators) return true;

                    Player player = Player.Get(sender);

                    if (q.StartsWith("$") || player == null) return true; // || !config.DonatorList.Contains(player.GroupName)

                    sender.RaReply($"AutoEvent#A mini-game is currently underway, acess denied!", false, true, string.Empty);
                    
                    return false;
                }
                catch (Exception e)
                {
                    Log.Error($"Patch Error - <Server> [RemoteAdmin]:{e}\n{e.StackTrace}");
                    return true;
                }
            }
        }
    }
}
