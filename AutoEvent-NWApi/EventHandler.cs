using AutoEvent.Events.EventArgs;
using LightContainmentZoneDecontamination;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace AutoEvent
{
    internal class EventHandler
    {
        [PluginEvent(ServerEventType.RoundRestart)]
        public void OnRestarting()
        {
            if (AutoEvent.ActiveEvent == null) return;

            ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
        }

        [PluginEvent(ServerEventType.LczDecontaminationStart)]
        public void OnDecontamination(LczDecontaminationStartEvent ev)
        {
            if (AutoEvent.ActiveEvent == null) return;

            DecontaminationController.Singleton.NetworkDecontaminationOverride = DecontaminationController.DecontaminationStatus.Disabled;
        }

        public void OnRemoteAdmin(RemoteAdminArgs ev)
        {
            var config = AutoEvent.Singleton.Config;

            if (AutoEvent.ActiveEvent == null) return;
            if (!config.IsDisableDonators) return;

            Player player = Player.Get(ev.CommandSender);

            if (ev.Command.StartsWith("$") || player == null) return;
            if (!config.DonatorList.Contains(ServerStatic.PermissionsHandler._members[player.UserId])) return;

            ev.CommandSender.RaReply($"AutoEvent#A mini-game is currently underway, acess denied!", false, true, string.Empty);
            ev.IsAllowed = false;
            ev.IsSuccess = false;
        }
    }
}
