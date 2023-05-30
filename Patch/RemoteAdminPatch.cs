using Exiled.API.Features;
using HarmonyLib;
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
                    if (!config.IsDisableDonators) return true;

                    if (AutoEvent.ActiveEvent == null) return true;

                    if (q.Contains("$"))
                    {
                        return true;
                    }

                    if (sender.SenderId == "SERVER CONSOLE" && !q.Contains("REQUEST_DATA"))
                    {
                        return true;
                    }

                    bool Success = true;
                    bool Allowed = true;
                    Player player = Player.Get(sender);

                    if (config.DonatorList.Contains(player.GroupName))
                    {
                        Success = false;
                        Allowed = false;
                        sender.RaReply($"SYSTEM#Сейчас проводятся мини-игры!", Success, true, string.Empty);
                    }
                    return Allowed;
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
