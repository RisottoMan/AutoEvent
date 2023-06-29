using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using PlayerRoles;
using AutoEvent.Interfaces;
using Exiled.API.Features;
using MapEditorReborn.API.Features.Objects;
using UnityEngine;

namespace AutoEvent.Events.Pazzle
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = "Puzzle";
        public override string Description { get; set; } = "alpha testing";
        public override string Color { get; set; } = "FFFF00";
        public override string CommandName { get; set; } = "puzzle";
        public SchematicObject GameMap { get; set; }
        public TimeSpan EventTime { get; set; }

        public int Stage { get; set; }

        EventHandler _eventHandler;

        public override void OnStart()
        {
            _eventHandler = new EventHandler();

            Exiled.Events.Handlers.Player.Verified += _eventHandler.OnJoin;
            Exiled.Events.Handlers.Player.DroppingItem += _eventHandler.OnDroppingItem;
            Exiled.Events.Handlers.Server.RespawningTeam += _eventHandler.OnTeamRespawn;
            OnEventStarted();
        }
        public override void OnStop()
        {
            Exiled.Events.Handlers.Player.Verified -= _eventHandler.OnJoin;
            Exiled.Events.Handlers.Player.DroppingItem -= _eventHandler.OnDroppingItem;
            Exiled.Events.Handlers.Server.RespawningTeam -= _eventHandler.OnTeamRespawn;
            Timing.CallDelayed(10f, () => EventEnd());
            AutoEvent.ActiveEvent = null;
            _eventHandler = null;
        }
        public void OnEventStarted()
        {
            EventTime = new TimeSpan(0, 0, 0);
            Stage = 0;
            GameMap = Extensions.LoadMap("Pazzle", new Vector3(76f, 1026.5f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Extensions.PlayAudio("ClassicMusic.ogg", 5, true, Name);

            foreach (Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);
                player.Position = GameMap.Position + new Vector3(0, 8.2f, 0);
            }
            Timing.RunCoroutine(OnEventRunning(), "pazzle_run");
        }
        public IEnumerator<float> OnEventRunning()
        {
            for (int time = 5; time > 0; time--)
            {
                Extensions.Broadcast($"Ивент Пазл\n" +
                    $"До начала <color=red>{time}</color> секунд.", 1);
                yield return Timing.WaitForSeconds(1f);
            }
            /*
            // Change color
            foreach (var obj in GameMap.AttachedBlocks.Where(x => x.name == "Platform").ToList())
            {
                obj.GetComponent<PrimitiveObject>().Primitive.Color = UnityEngine.Color.red;
            }
            */
            /*
            while (Stage != 5)
            {


                yield return Timing.WaitForSeconds(0.3f);
            }
            */
            OnStop();
            yield break;
        }
        public void ChangeStage()
        {
            Stage++;

        }
        public void EventEnd()
        {
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.UnLoadMap(GameMap);
            Extensions.StopAudio();
        }
    }
}
