using CommandSystem;
using System;
using PlayerRoles;
using PluginAPI.Core;

namespace AutoEvent.Commands
{
    internal class StopEvent : ICommand
    {
        public string Command => "stop";
        public string Description => "Kills the running mini-game (just kills all the players)";
        public string[] Aliases => null;
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var config = AutoEvent.Singleton.Config;
            var player = Player.Get(sender);

            if (!config.PermissionList.Contains(ServerStatic.PermissionsHandler._members[player.UserId]))
            {
                response = "<color=red>You do not have permission to use this command!</color>";
                return false;
            }

            if (AutoEvent.ActiveEvent == null)
            {
                response = "The mini-game is not running!";
                return false;
            }

            AutoEvent.ActiveEvent.StopEvent();

            foreach (Player pl in Player.GetPlayers())
            {
                pl.SetRole(RoleTypeId.Spectator);
            }

            response = "Killed all the players and the mini-game itself will end soon.";
            return true;
        }
    }
}
