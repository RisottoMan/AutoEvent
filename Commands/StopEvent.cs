using CommandSystem;
using System;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using PlayerRoles;

namespace AutoEvent.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class StopEvent : ICommand
    {
        public string Command => "ev_stop";
        public string Description => "Kills the running mini-game (just kills all the players)";
        public string[] Aliases => null;
        // I realized that there is no point in killing endless cycles, because it's enough to simply enter a check for the number of live players in the cycle.
        // Therefore, the easiest thing to do for those who will write their own mini-game is to come up with good logic.
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("ev.stop"))
            {
                response = "You do not have permission to use this command";
                return false;
            }
            if (AutoEvent.ActiveEvent == null)
            {
                response = "The mini-game is not running!";
                return false;
            }
            foreach (Player player in Player.List) player.Role.Set(RoleTypeId.Spectator);
            response = "Killed all the players and the mini-game itself will end soon.";
            return true;
        }
    }
}
