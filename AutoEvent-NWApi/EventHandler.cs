using AutoEvent.Events.EventArgs;
using Exiled.Permissions.Extensions;
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

            if (AutoEvent.ActiveEvent == null || !config.IsDisableDonators)
                return;

#if !EXILED
            Player player = Player.Get(ev.CommandSender);
            if (player == null || !config.DonatorList.Contains(ServerStatic.PermissionsHandler._members[player.UserId])) 
                return;
#else
            if (!Exiled.API.Features.Player.Get(ev.CommandSender).CheckPermission("ev.donator"))
                return;
#endif
            ev.CommandSender.RaReply($"AutoEvent#A mini-game is currently underway, access denied!", false, true, string.Empty);
            ev.IsAllowed = false;
            ev.IsSuccess = false;
        }
    }
}
