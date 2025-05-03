using CommandSystem;
using System;
using PlayerRoles;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace AutoEvent.Commands;
internal class Stop : ICommand
{
    public string Command => nameof(Stop);
    public string Description => "Kills the running mini-game (just kills all the players)";
    public string[] Aliases => [];
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission("ev.stop"))
        {
            response = "<color=red>You do not have permission to use this command!</color>";
            return false;
        }

        if (AutoEvent.EventManager.CurrentEvent == null)
        {
            response = "The mini-game is not running!";
            return false;
        }

        AutoEvent.EventManager.CurrentEvent.StopEvent();

        foreach (Player pl in Player.List)
        {
            pl.Role.Set(RoleTypeId.Spectator);
        }

        response = "Killed all the players and the mini-game itself will end soon.";
        return true;
    }
}
