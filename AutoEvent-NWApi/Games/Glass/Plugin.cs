using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using PlayerRoles;
using UnityEngine;
using AutoEvent.Games.Glass.Features;
using Mirror;
using CustomPlayerEffects;
using PluginAPI.Core;
using PluginAPI.Events;
using AutoEvent.API.Schematic.Objects;
using AutoEvent.Events.Handlers;
using Object = UnityEngine.Object;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Glass
{
    public class Plugin : Event
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.GlassName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.GlassDescription;
        public override string Author { get; set; } = "KoT0XleB";
        public override string MapName { get; set; } = "Glass";
        public override string CommandName { get; set; } = "glass";
        public SchematicObject GameMap { get; set; }
        public List<GameObject> Platformes { get; set; }
        public GameObject Lava { get; set; }
        public GameObject Finish { get; set; }
        public TimeSpan EventTime { get; set; }

        EventHandler _eventHandler;

        public override void OnStart()
        {
            _eventHandler = new EventHandler();
            EventManager.RegisterEvents(_eventHandler);
            Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
            Players.DropItem += _eventHandler.OnDropItem;

            OnEventStarted();
        }
        public override void OnStop()
        {
            EventManager.UnregisterEvents(_eventHandler);
            Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= _eventHandler.OnSpawnRagdoll;
            Players.DropItem -= _eventHandler.OnDropItem;

            _eventHandler = null;
            Timing.CallDelayed(10f, () => EventEnd());
        }

        public void OnEventStarted()
        {
            GameMap = Extensions.LoadMap(MapName, new Vector3(76f, 1026.5f, -43.68f), Quaternion.Euler(Vector3.zero), Vector3.one);
            Extensions.PlayAudio("CrabGame.ogg", 15, true, Name);

            Lava = GameMap.AttachedBlocks.First(x => x.name == "Lava");
            Lava.AddComponent<LavaComponent>();
 
            int platformCount;
            int playerCount = Player.GetPlayers().Count(r => r.IsAlive);
            if (playerCount <= 5)
            {
                platformCount = 3;
                EventTime = new TimeSpan(0, 0, 30);
            }
            else if (playerCount > 5 && playerCount <= 15)
            {
                platformCount = 6;
                EventTime = new TimeSpan(0, 1, 0);
            }
            else if (playerCount > 15 && playerCount <= 25)
            {
                platformCount = 9;
                EventTime = new TimeSpan(0, 1, 30);
            }
            else if (playerCount > 25 && playerCount <= 30)
            {
                platformCount = 12;
                EventTime = new TimeSpan(0, 2, 0);
            }
            else
            {
                platformCount = 15;
                EventTime = new TimeSpan(0, 2, 30);
            }

            var platform = GameMap.AttachedBlocks.First(x => x.name == "Platform");
            var platform1 = GameMap.AttachedBlocks.First(x => x.name == "Platform1");

            Platformes = new List<GameObject>();
            var delta = new Vector3(3.69f, 0, 0);
            for (int i = 0; i < platformCount; i++)
            {
                var newPlatform = Object.Instantiate(platform, platform.transform.position + delta * (i + 1), Quaternion.identity);
                NetworkServer.Spawn(newPlatform);
                Platformes.Add(newPlatform);

                var newPlatform1 = Object.Instantiate(platform1, platform1.transform.position + delta * (i + 1), Quaternion.identity);
                NetworkServer.Spawn(newPlatform1);
                Platformes.Add(newPlatform1);

                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    newPlatform.AddComponent<GlassComponent>();
                }
                else
                {
                    newPlatform1.AddComponent<GlassComponent>();
                }
            }

            Finish = GameMap.AttachedBlocks.First(x => x.name == "Finish");
            Finish.transform.position = (platform.transform.position + platform1.transform.position) / 2f + delta * (platformCount + 2);

            foreach (Player player in Player.GetPlayers())
            {
                Extensions.SetRole(player, RoleTypeId.ClassD, RoleSpawnFlags.None);
                player.Position = RandomClass.GetSpawnPosition(GameMap);
                player.EffectsManager.EnableEffect<Disabled>();
            }
            Timing.RunCoroutine(OnEventRunning(), "glass_time");
        }
        public IEnumerator<float> OnEventRunning()
        {
            var translation = AutoEvent.Singleton.Translation;

            for (int time = 15; time > 0; time--)
            {
                Extensions.Broadcast($"<size=100><color=red>{time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }

            GameObject.Destroy(GameMap.AttachedBlocks.First(x => x.name == "Wall"));

            while (EventTime.TotalSeconds > 0 && Player.GetPlayers().Count(r => r.IsAlive) > 0)
            {
                var text = translation.GlassStart;
                text = text.Replace("{plyAlive}", Player.GetPlayers().Count(r => r.IsAlive).ToString());
                text = text.Replace("{eventTime}", $"{EventTime.Minutes}:{EventTime.Seconds}");

                Extensions.Broadcast(text, 1);

                yield return Timing.WaitForSeconds(1f);
                EventTime -= TimeSpan.FromSeconds(1f);
            }

            foreach (Player player in Player.GetPlayers())
            {
                if (Vector3.Distance(player.Position, Finish.transform.position) >= 10)
                {
                    player.Damage(500, translation.GlassDied);
                }
            }
            
            if (Player.GetPlayers().Count(r => r.IsAlive) > 1)
            {
                Extensions.Broadcast(translation.GlassWinSurvived.Replace("{countAlive}", Player.GetPlayers().Count(r => r.IsAlive).ToString()), 3);
            }
            else if (Player.GetPlayers().Count(r => r.IsAlive) == 1)
            {
                Extensions.Broadcast(translation.GlassWinner.Replace("{winner}", Player.GetPlayers().First(r =>r.IsAlive).Nickname), 10);
            }
            else if (Player.GetPlayers().Count(r => r.IsAlive) < 1)
            {
                Extensions.Broadcast(translation.GlassFail, 10);
            }

            OnStop();
            yield break;
        }
        public void EventEnd()
        {
            Platformes.ForEach(Object.Destroy);
            Extensions.CleanUpAll();
            Extensions.TeleportEnd();
            Extensions.UnLoadMap(GameMap);
            Extensions.StopAudio();
            AutoEvent.ActiveEvent = null;
        }
    }
}
