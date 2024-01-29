using AutoEvent.Events.EventArgs;
using InventorySystem.Items.ThrowableProjectiles;
using LightContainmentZoneDecontamination;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
#if EXILED
using Exiled.Permissions.Extensions;
#endif

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
    }
}
