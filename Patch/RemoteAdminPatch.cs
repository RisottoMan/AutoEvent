using Exiled.API.Features;
using HarmonyLib;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoEvent
{
    internal class RemoteAdminPatch
    {
        public static Config config = AutoEvent.Singleton.Config;

        [HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery))]
        static class RemoteAdmin
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

                    bool Success = true;
                    bool Allowed = true;
                    Player player = Player.Get(sender);

                    if (!player.IsHost && !(player.Sender.SenderId == null) && !(player.Id == 1))
                    {
                        if (config.DonatorList.Contains(player.GroupName))
                        {
                            Success = false;
                            Allowed = false;
                            sender.RaReply($"SYSTEM#Сейчас проводятся мини-игры!", Success, true, string.Empty);
                        }
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
