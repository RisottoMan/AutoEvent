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
    internal class List : ICommand
    {
        public List()
        {
            // Log.Debug("Skipping Registering List Command");
        }
        public string Command => nameof(List);
        public string Description => "Shows a list of all the events that can be started.";
        public string[] Aliases => new string[] { };
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
#if EXILED
            if (!sender.CheckPermission("ev.list"))
            {
                response = "You do not have permission to use this command";
                return false;
            }
#else
            var config = AutoEvent.Singleton.Config;
            var player = Player.Get(sender);
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
            builder.AppendLine("\"<color=yellow><b>List of events</b></color>:");

            foreach (Event ev in Event.Events)
            {
                string color = "white";
                switch (ev)
                {
                    case IInternalEvent:
                        color = "yellow";
                        break;
                    case IExiledEvent:
                        color = "blue";
                        break;
                    default:
                        color = "orange";
                        break;
                }
                builder.AppendLine($"<color={color}>{ev.Name} [<color=white>{ev.CommandName}<color={color}>]</color>: {ev.Description}");
            }
            builder.AppendLine("\n<color=yellow>To run an event, use the [<i><color=white>Command Name<color=yellow></i>] like following:</color>");
            builder.AppendLine("<color=yellow>Ev Run <i>CommandName</i></color>");

            response = builder.ToString();
            return true;
        }

    }
}
