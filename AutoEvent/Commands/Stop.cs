using CommandSystem;
using System;
using AutoEvent.Interfaces;
using PlayerRoles;
using PluginAPI.Core;
#if EXILED
using Exiled.Permissions.Extensions;
#endif
namespace AutoEvent.Commands
{
    internal class Stop : ICommand, IPermission
    {
        public string Command => nameof(Stop);
        public string Description => "Kills the running mini-game (just kills all the players)";
        public string[] Aliases => new string[] { };
        public string Permission { get; set; } = "ev.stop";
        public bool SanitizeResponse => false;
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(((IPermission)this).Permission, out bool IsConsoleCommandSender))
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
