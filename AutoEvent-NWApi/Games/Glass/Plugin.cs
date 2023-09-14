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
using AutoEvent.Games.Infection;
using AutoEvent.Interfaces;
using Object = UnityEngine.Object;
using Event = AutoEvent.Interfaces.Event;

namespace AutoEvent.Games.Glass
{
    public class Plugin : Event, IEventSound, IEventMap, IInternalEvent
    {
        public override string Name { get; set; } = AutoEvent.Singleton.Translation.GlassTranslate.GlassName;
        public override string Description { get; set; } = AutoEvent.Singleton.Translation.GlassTranslate.GlassDescription;
        public override string Author { get; set; } = "KoT0XleB";

        public override string CommandName { get; set; } = "glass";
        public MapInfo MapInfo { get; set; } = new MapInfo()
            {MapName = "Glass", Position = new Vector3(76f, 1026.5f, -43.68f) };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
            { SoundName = "CrabGame.ogg", Volume = 15, Loop = true };
        protected override float PostRoundDelay { get; set; } = 10f;
        private EventHandler EventHandler { get; set; }
        private GlassTranslate Translation { get; set; }
        private List<GameObject> _platforms;
        private GameObject _lava;
        private GameObject _finish;
        private int _matchTimeInSeconds;

        protected override void RegisterEvents()
        {
            Translation = new GlassTranslate();
            EventHandler = new EventHandler();
            EventManager.RegisterEvents(EventHandler);
            Servers.TeamRespawn += EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += EventHandler.OnSpawnRagdoll;
            Players.DropItem += EventHandler.OnDropItem;
        }

        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(EventHandler);
            Servers.TeamRespawn -= EventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= EventHandler.OnSpawnRagdoll;
            Players.DropItem -= EventHandler.OnDropItem;

            EventHandler = null;
        }

        protected override void OnStart()
        {
            _lava = MapInfo.Map.AttachedBlocks.First(x => x.name == "Lava");
            _lava.AddComponent<LavaComponent>();
 
            int platformCount;
            int playerCount = Player.GetPlayers().Count(r => r.IsAlive);
            if (playerCount <= 5)
            {
                platformCount = 3;
                _matchTimeInSeconds = 30;
            }
            else if (playerCount > 5 && playerCount <= 15)
            {
                platformCount = 6;
                _matchTimeInSeconds = 60;
            }
            else if (playerCount > 15 && playerCount <= 25)
            {
                platformCount = 9;
                _matchTimeInSeconds = 90;
            }
            else if (playerCount > 25 && playerCount <= 30)
            {
                platformCount = 12;
                _matchTimeInSeconds = 120;
            }
            else
            {
                platformCount = 15;
                _matchTimeInSeconds = 150;
            }

            var platform = MapInfo.Map.AttachedBlocks.First(x => x.name == "Platform");
            var platform1 = MapInfo.Map.AttachedBlocks.First(x => x.name == "Platform1");

            _platforms = new List<GameObject>();
            var delta = new Vector3(3.69f, 0, 0);
            for (int i = 0; i < platformCount; i++)
            {
                var newPlatform = Object.Instantiate(platform, platform.transform.position + delta * (i + 1), Quaternion.identity);
                NetworkServer.Spawn(newPlatform);
                _platforms.Add(newPlatform);

                var newPlatform1 = Object.Instantiate(platform1, platform1.transform.position + delta * (i + 1), Quaternion.identity);
                NetworkServer.Spawn(newPlatform1);
                _platforms.Add(newPlatform1);

                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    newPlatform.AddComponent<GlassComponent>();
                }
                else
                {
                    newPlatform1.AddComponent<GlassComponent>();
                }
            }

            _finish = MapInfo.Map.AttachedBlocks.First(x => x.name == "Finish");
            _finish.transform.position = (platform.transform.position + platform1.transform.position) / 2f + delta * (platformCount + 2);

            foreach (Player player in Player.GetPlayers())
            {
                Extensions.SetRole(player, RoleTypeId.ClassD, RoleSpawnFlags.None);
                player.Position = RandomClass.GetSpawnPosition(MapInfo.Map);
                player.EffectsManager.EnableEffect<Disabled>();
            }
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int time = 15; time > 0; time--)
            {
                Extensions.Broadcast($"<size=100><color=red>{time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override void CountdownFinished()
        {
            GameObject.Destroy(MapInfo.Map.AttachedBlocks.First(x => x.name == "Wall"));
        }

        protected override bool IsRoundDone()
        {
            // Elapsed time is smaller then the match time (+ countdown) &&
            // At least one player is alive.
            return !(EventTime.TotalSeconds < _matchTimeInSeconds + 15 && Player.GetPlayers().Count(r => r.IsAlive) > 0);
        }

        protected override void ProcessFrame()
        {
            var text = Translation.GlassStart;
            text = text.Replace("{plyAlive}", Player.GetPlayers().Count(r => r.IsAlive).ToString());
            text = text.Replace("{eventTime}", $"{EventTime.Minutes:00}:{EventTime.Seconds:00}");

            Extensions.Broadcast(text, 1);
        }

        protected override void OnFinished()
        {
            foreach (Player player in Player.GetPlayers())
            {
                if (Vector3.Distance(player.Position, _finish.transform.position) >= 10)
                {
                    player.Damage(500, Translation.GlassDied);
                }
            }
            
            if (Player.GetPlayers().Count(r => r.IsAlive) > 1)
            {
                Extensions.Broadcast(Translation.GlassWinSurvived.Replace("{countAlive}", Player.GetPlayers().Count(r => r.IsAlive).ToString()), 3);
            }
            else if (Player.GetPlayers().Count(r => r.IsAlive) == 1)
            {
                Extensions.Broadcast(Translation.GlassWinner.Replace("{winner}", Player.GetPlayers().First(r =>r.IsAlive).Nickname), 10);
            }
            else if (Player.GetPlayers().Count(r => r.IsAlive) < 1)
            {
                Extensions.Broadcast(Translation.GlassFail, 10);
            }
        }

        protected override void OnCleanup()
        {
            _platforms.ForEach(Object.Destroy);
        }
    }
}
