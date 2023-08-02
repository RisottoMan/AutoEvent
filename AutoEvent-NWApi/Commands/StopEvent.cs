using CommandSystem;
using System;
using PlayerRoles;
using AutoEvent.Interfaces;
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
            /*
            if (!((CommandSender)sender).CheckPermission("ev.stop"))
            {
                response = "You do not have permission to use this command";
                return false;
            }
            */
            if (AutoEvent.ActiveEvent == null)
            {
                response = "The mini-game is not running!";
                return false;
            }

            /*
            Event ev = Event.GetEvent(arguments.At(0));
            if (ev == null)
            {
                response = "event not found.";
                return false;
            }

            ev.OnStop();
            AutoEvent.ActiveEvent = null;
            */

            foreach (Player player in Player.GetPlayers()) player.SetRole(RoleTypeId.Spectator);

            response = "Killed all the players and the mini-game itself will end soon.";
            return true;
        }
    }
}
