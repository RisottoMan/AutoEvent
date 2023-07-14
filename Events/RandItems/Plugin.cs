using AutoEvent.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events.RandItems
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.RandName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.RandDescription;
        public override string CommandName { get; set; } = "randitems";
        public override string Color { get; set; } = "39B310";
        public TimeSpan EventTime = new TimeSpan(0, 0, 0);

        private List<ItemType> WarningItems = new List<ItemType>() {ItemType.SCP244b, ItemType.SCP244a}; // Предметы, выдача которых может привести к высокой нагрузке на хостинг.
        private List<ItemType> RandomItems = Enum.GetValues(typeof(ItemType)).Cast<ItemType>().ToList();

        public override void OnStart() => OnEventStarted();

        public override void OnStop() => EventEnd();

        private void OnEventStarted()
        {
            Round.IsLocked = false;

            RandomItems.RemoveAll(item => WarningItems.Contains(item));
            Timing.RunCoroutine(OnEventRunning(), "randitems_run");
        }

        public IEnumerator<float> OnEventRunning()
        {
            var trans = AutoEvent.Singleton.Translation;

            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast($"{time}", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            while (!Round.IsEnded)
            {
                if (EventTime.TotalMinutes % 3 == 0)
                {
                    ItemType item = RandomItems.RandomItem();
                    Player.List.ToList().ForEach(p => 
                    {
                        p.AddItem(item);
                    });
                }
                yield return Timing.WaitForSeconds(1f);
            }

            OnStop();
            yield break;
        }

        public void EventEnd()
        {
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.StopAudio();
            AutoEvent.ActiveEvent = null;
        }
    }
}
