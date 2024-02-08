using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using UnityEngine;
using PluginAPI.Events;
using AutoEvent.Events.Handlers;
using AutoEvent.Interfaces;
using AutoEvent.Games.Fnaf.Features;
using Event = AutoEvent.Interfaces.Event;
using Player = PluginAPI.Core.Player;

namespace AutoEvent.Games.Fnaf
{
    public class Plugin : Event, IEventMap, IInternalEvent, IHidden
    {
        public override string Name { get; set; } = "Five Nights at Freddy's";
        public override string Description { get; set; } = "Survive one night with animatronics";
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "fnaf";
        public override Version Version { get; set; } = new Version(1, 0, 1);
        [EventConfig]
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        public MapInfo MapInfo { get; set; } = new MapInfo()
        {
            MapName = "Fnaf",
            Position = new Vector3(0, 30, 30)
        };
        private EventHandler _eventHandler;
        protected override void RegisterEvents()
        {
            _eventHandler = new EventHandler(this);
            EventManager.RegisterEvents(_eventHandler);
            Servers.TeamRespawn += _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll += _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet += _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood += _eventHandler.OnPlaceBlood;
            Players.DropItem += _eventHandler.OnDropItem;
            Players.DropAmmo += _eventHandler.OnDropAmmo;
            Players.PlayerDamage += _eventHandler.OnDamage;
            Players.UsingStamina += _eventHandler.OnUsingStamina;
        }

        protected override void UnregisterEvents()
        {
            EventManager.UnregisterEvents(_eventHandler);
            Servers.TeamRespawn -= _eventHandler.OnTeamRespawn;
            Servers.SpawnRagdoll -= _eventHandler.OnSpawnRagdoll;
            Servers.PlaceBullet -= _eventHandler.OnPlaceBullet;
            Servers.PlaceBlood -= _eventHandler.OnPlaceBlood;
            Players.DropItem -= _eventHandler.OnDropItem;
            Players.DropAmmo -= _eventHandler.OnDropAmmo;
            Players.PlayerDamage -= _eventHandler.OnDamage;
            Players.UsingStamina -= _eventHandler.OnUsingStamina;

            _eventHandler = null;
        }

        protected override void OnStart()
        {
            //_animDict = new Dictionary<string, AnimatronicClass>();
            AddAnimatronicFromPreset(Config.FreddyPreset);

            foreach (Player player in Player.GetPlayers())
            {
                player.GiveLoadout(Config.PlayerLoadout);
                player.Position = MapInfo.Map.AttachedBlocks.Where(r => r.name == "Spawnpoint").
                    ToList().FirstOrDefault().transform.position;
            }
        }

        protected void AddAnimatronicFromPreset(AnimatronicPreset preset)
        {
            GameObject animObject = new GameObject();
            List<Vector3> animPositions = new List<Vector3>();

            foreach (GameObject gameObject in MapInfo.Map.AttachedBlocks)
            {
                if (gameObject.name == preset.Name)
                {
                    animObject = gameObject;
                }
                
                if (gameObject.name == preset.PositionName)
                {
                    animPositions.Add(gameObject.transform.position);
                }
            }

            /*
            _animDict.Add(preset.Name, new AnimatronicClass()
            {
                Level = preset.Level,
                State = FreddyState.Playing,
                GameObject = animObject,
                Positions = animPositions,
                IndexPosition = 0,
                IsDoorClosed = false,
                Timer = preset.Timer,
                Counter = preset.Timer
            });
            */
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int time = 10; time > 0; time--)
            {
                Extensions.Broadcast("waiting", 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override bool IsRoundDone()
        {
            //return !(Player.GetPlayers().Count(r => r.IsAlive) > 1);
            return false;
        }
        protected override float FrameDelayInSeconds { get; set; } = 0.1f;
        protected override void ProcessFrame()
        {
            /*
            foreach(var item in _animDict)
            {
                UpdateAnimatronicLogic(item.Value);
            }
            */
            Extensions.Broadcast("Cycle", 1);
        }
        /*
        protected void UpdateAnimatronicLogic(AnimatronicClass anim)
        {
            // Время ожидания аниматроника
            if (anim.Counter > 0)
            {
                anim.Counter -= FrameDelayInSeconds;
                return;
            }

            // Возвращаем в счетчик время ожидания аниматроника
            anim.Counter = anim.Timer;

            // Если случайное значение меньше, чем левел аниматроника, то он ходит, иначе скипаем
            if (RNGGenerator.GetRandomNumber(1, 21) > anim.Level)
                return;

            // В зависимости от состояния аниматроник ходит
            switch (anim.State)
            {
                case FreddyState.Playing:
                    {
                        // Подразумевается, что цель аниматроников дойти до комнаты охраны
                        int index = anim.IndexPosition++;

                        // Если аниматроник дойдет до последнего состояния к двери охраны, то меняем состояние на ожидание
                        if (index == anim.Positions.Count)
                        {
                            anim.State++;
                        }

                        // Телепорт до следующей позиции
                        anim.GameObject.transform.position = anim.Positions[anim.IndexPosition++];
                    }
                    break;
            }
        }
        */
        protected override void OnFinished()
        {
            Extensions.Broadcast("End", 10);
        }
    }
}