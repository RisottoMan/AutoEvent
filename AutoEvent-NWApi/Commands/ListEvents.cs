using AutoEvent.Interfaces;
using CommandSystem;
using PluginAPI.Core;
using System;
using System.Text;
#if EXILED
using Exiled.Permissions.Extensions;
#endif

namespace AutoEvent.Commands
{
    internal class ListEvents : ICommand
    {
        public string Command => "list";
        public string Description => "Shows a list of all the events that can be started.";
        public string[] Aliases => null;
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var config = AutoEvent.Singleton.Config;
            var player = Player.Get(sender);
#if EXILED
            if (!((CommandSender)sender).CheckPermission("ev.list"))
            {
                response = "You do not have permission to use this command";
                return false;
            }
#else
            if (sender is ServerConsoleSender || sender is CommandSender cmdSender && cmdSender.FullPermissions)
            {
                goto skipPermissionCheck;
            }
            if (!config.PermissionList.Contains(ServerStatic.PermissionsHandler._members[player.UserId]))
            {
                response = "<color=red>You do not have permission to use this command!</color>";
                return false;
            }
            skipPermissionCheck:
#endif            
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("\"<color=yellow><b>List of events (when running an event, you are responsible for it)</color></b>:");

            foreach (Event ev in Event.Events)
                builder.AppendLine($"<color=yellow>[{ev.CommandName}]</color>: {ev.Description}");

            response = builder.ToString();
            return true;
        }
    }
}
