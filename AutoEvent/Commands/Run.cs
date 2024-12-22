using AutoEvent.Interfaces;
using CommandSystem;
using System;
using Exiled.API.Features;
using MEC;
using Exiled.Permissions.Extensions;

namespace AutoEvent.Commands;
internal class Run : ICommand, IUsageProvider
{
    public string Command => nameof(Run);
    public string Description => "Run the event, takes on 1 argument - the command name of the event";
    public string[] Aliases => ["start", "play", "begin"];
    public string[] Usage => ["Event Name"];
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission("ev.run"))
        {
            response = "<color=red>You do not have permission to use this command!</color>";
            return false;
        }

        if (AutoEvent.ActiveEvent != null)
        {
            response = $"The mini-game {AutoEvent.ActiveEvent.Name} is already running!";
            return false;
        }

        if (arguments.Count < 1)
        {
            response = "Only 1 argument is needed - the command name of the event!";
            return false;
        }

        Event ev = Event.GetEvent(arguments.At(0));
        if (!(ev is IEventMap map && !string.IsNullOrEmpty(map.MapInfo.MapName) && map.MapInfo.MapName.ToLower() != "none"))
        {
            Log.Warn("No map has been specified for this event!");
        }
        else if (!Extensions.IsExistsMap(map.MapInfo.MapName))
        {
            response = $"You need to download the map {map.MapInfo.MapName} to run this mini game.\n" +
                       $"Download and install Schematics.tar.gz from the github.";
            return false;
        }

        Round.IsLocked = true;

        if (!Round.IsStarted)
        {
            Round.Start();

            Timing.CallDelayed(2f, () => 
            {
                foreach (Player player in Player.List)
                {
                    player.ClearInventory();
                }

                ev.StartEvent();
                AutoEvent.ActiveEvent = ev;
            });
        }
        else
        {
            ev.StartEvent();
            AutoEvent.ActiveEvent = ev;
        }

        response = $"The mini-game {ev.Name} has started!";
        return true;
    }
}