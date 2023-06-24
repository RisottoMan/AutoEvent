using CommandSystem;
using System;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace AutoEvent.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class RunEvent : ICommand
    {
        public string Command => "ev_run";
        public string Description => "Run the event, takes on 1 argument - the command name of the event.";
        public string[] Aliases => null;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("ev.run"))
            {
                response = "You do not have permission to use this command";
                return false;
            }
            if (!Round.IsStarted)
            {
                response = "The round has not started!";
                return false;
            }
            if (AutoEvent.ActiveEvent != null)
            {
                response = "The mini-game is already running!";
                return false;
            }
            if (arguments.Count != 1)
            {
                response = "Only 1 argument is needed - the command name of the event!";
                return false;
            }

            string resp = Extensions.GetEvent(arguments.At(0), (CommandSender)sender);

            response = resp;
            return true;
        }
    }
}
