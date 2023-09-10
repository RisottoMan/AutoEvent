using AutoEvent.Events.EventArgs;
using AutoEvent.Events.Handlers;
using HarmonyLib;
using PluginAPI.Core;
using RemoteAdmin;
using System;
using System.Linq;

namespace AutoEvent.Patches
{
    [HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery))]
    internal class RemoteAdmin
    {
        public static Config config = AutoEvent.Singleton.Config;
        static bool Prefix(string q, CommandSender sender)
        {
            try
            {
                if (q.Contains("$"))
                {
                    return true;
                }

                if (sender.SenderId == "SERVER CONSOLE" && !q.Contains("REQUEST_DATA"))
                {
                    return true;
                }

                string[] arr = q.Split(' ');
                string name = arr[0].ToLower();
                string[] args = arr.Skip(1).ToArray();

                RemoteAdminArgs remoteAdminPatch = new RemoteAdminArgs(sender, Player.Get(sender), q, name, args, true, true);
                Servers.OnRemoteAdmin(remoteAdminPatch);

                if (!string.IsNullOrEmpty(remoteAdminPatch.Reply))
                {
                    sender.RaReply($"SYSTEM#{remoteAdminPatch.Reply}", remoteAdminPatch.IsSuccess, true, string.Empty);
                }

                return remoteAdminPatch.IsAllowed;
            }
            catch (Exception e)
            {
                DebugLogger.LogDebug("Patching Error has occured at remote admin patch.", LogLevel.Error, true);
                DebugLogger.LogDebug($"Patch Error - <Server> [RemoteAdmin]:{e}\n{e.StackTrace}", LogLevel.Error);
                return true;
            }
        }
    }
}
