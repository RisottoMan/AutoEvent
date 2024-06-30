using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoEvent.API.Enums;
using UnityEngine;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Events;
using AutoEvent.Events.Handlers;
using AutoEvent.Interfaces;
using Event = AutoEvent.Interfaces.Event;
using Random = UnityEngine.Random;

namespace AutoEvent.Games.Airstrike
{
    public class Plugin : Event, IEventMap, IEventSound, IInternalEvent
    {
        public override string Name { get; set; } = "Airstrike Party";
        public override string Description { get; set; } = "Survive as aistrikes rain down from above.";
        public override string Author { get; set; } = "KoT0XleB";
        public override string CommandName { get; set; } = "airstrike";
        public override Version Version { get; set; } = new Version(1, 0, 3);
        public MapInfo MapInfo { get; set; } = new MapInfo()
        { 
            MapName = "DeathParty", 
            Position = new Vector3(10f, 1012f, -40f)
        };
        public SoundInfo SoundInfo { get; set; } = new SoundInfo()
        { 
            SoundName = "DeathParty.ogg", 
            Volume = 5
        };
        [EventConfig]
        public Config Config { get; set; }
        [EventTranslation]
        public Translation Translation { get; set; }
        protected override float PostRoundDelay { get; set; } = 5f;
        protected override FriendlyFireSettings ForceEnableFriendlyFire { get; set; } = FriendlyFireSettings.Enable;
        protected override FriendlyFireSettings ForceEnableFriendlyFireAutoban { get; set; } = FriendlyFireSettings.Disable;
        private EventHandler _eventHandler { get; set; }
        private bool RespawnWithGrenades => Config.RespawnPlayersWithGrenades;
        public int Stage { get; private set; }
        private CoroutineHandle _grenadeCoroutineHandle;

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
            Players.PlayerDamage += _eventHandler.OnPlayerDamage;
            Players.PlayerDying += _eventHandler.OnPlayerDying;
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
            Players.PlayerDamage -= _eventHandler.OnPlayerDamage;
            Players.PlayerDying -= _eventHandler.OnPlayerDying;
            _eventHandler = null;
        }

        protected override void OnStart()
        {
            Server.FriendlyFire = true;

            foreach (Player player in Player.GetPlayers())
            {
                player.SetRole(RoleTypeId.ClassD, RoleChangeReason.None);
                player.Position = RandomClass.GetSpawnPosition(MapInfo.Map);
            }
        }

        protected override void OnStop()
        {
            Timing.CallDelayed(1.2f, () => {
                if (_grenadeCoroutineHandle.IsRunning)
                {
                    Timing.KillCoroutines(new CoroutineHandle[] { _grenadeCoroutineHandle });
                }
            });
        }

        protected override IEnumerator<float> BroadcastStartCountdown()
        {
            for (int _time = 10; _time > 0; _time--)
            {
                Extensions.Broadcast($"<size=100><color=red>{_time}</color></size>", 1);
                yield return Timing.WaitForSeconds(1f);
            }
        }

        protected override void CountdownFinished()
        { 
            _grenadeCoroutineHandle = Timing.RunCoroutine(GrenadeCoroutine(), "death_grenade");
        }

        protected override void ProcessFrame()
        {
            var count = Player.GetPlayers().Count(r => r.IsAlive && r.Role != RoleTypeId.ChaosConscript).ToString();
            var cycleTime = $"{EventTime.Minutes:00}:{EventTime.Seconds:00}";
            Extensions.Broadcast(Translation.Cycle.Replace("{count}", count).Replace("{time}", cycleTime), 1);
        }

        protected override bool IsRoundDone()
        {
            int playerCount = Player.GetPlayers().Count(r => r.IsAlive && r.Role != RoleTypeId.ChaosConscript);
            return !(playerCount > (Config.LastPlayerAliveWins ? 1 : 0) 
                && Stage <= Config.Rounds);
        }

        public IEnumerator<float> GrenadeCoroutine()
        { 
            Stage = 1;
            float fuse = 10f;
            float height = 20f;
            float count = 20;
            float timing = 1f;
            float scale = 4;
            float radius = MapInfo.Map.AttachedBlocks.First(x => x.name == "Arena").transform.localScale.x / 2 - 6f;

            while (Player.GetPlayers().Count(r => r.IsAlive) > (Config.LastPlayerAliveWins ? 1 : 0) && Stage <= Config.Rounds)
            {
                if (KillLoop)
                {
                    yield break;
                }

                DebugLogger.LogDebug($"Stage: {Stage}/{Config.Rounds}. Radius: {radius}, Scale: {scale}, Count: {count}, Timing: {timing}, Height: {height}, Fuse: {fuse}, Target: {Config.TargetPlayers}");
                
                // Not the last round.
                if (Stage != Config.Rounds)
                {
                    int playerIndex = 0;
                    for (int i = 0; i < count; i++)
                    {

                        Vector3 pos = MapInfo.Map.Position + new Vector3(Random.Range(-radius, radius), height, Random.Range(-radius, radius));
                        // has to be re-iterated every run because a player could have been killed from the last one.
                        if (Config.TargetPlayers)
                        {
                            try
                            {
                                Player randomPlayer = Player.GetPlayers().Where(x => x.Role == RoleTypeId.ClassD)
                                    .ToList().RandomItem();
                                pos = randomPlayer.Position;
                                pos.y = height + MapInfo.Map.Position.y;
                                // Log.Debug($"Target Player Position: ({pos.x}, {pos.y}, {pos.z})");
                            }
                            catch (Exception e)
                            {
                                DebugLogger.LogDebug("Caught an error while targeting a player.", LogLevel.Warn, true);
                                DebugLogger.LogDebug($"{e}");
                            }
                        }
                        // Log.Debug($"Target Position: ({pos.x}, {pos.y}, {pos.z})");
                        Extensions.GrenadeSpawn(fuse, pos, scale);
                        yield return Timing.WaitForSeconds(timing);
                        playerIndex++;
                    }
                }
                else // last round.
                {
                    Vector3 pos = MapInfo.Map.Position + new Vector3(Random.Range(-10, 10), 20, Random.Range(-10, 10));
                    Extensions.GrenadeSpawn(10, pos, 75);
                }

                yield return Timing.WaitForSeconds(15f);
                Stage++;

                // Defaults: 
                count += 30;     //20,  50,  80,  110, [ignored last round] 1
                timing -= 0.3f;  //1.0, 0.7, 0.4, 0.1, [ignored last round] 10
                height -= 5f;    //20,  15,  10,  5,   [ignored last round] 20
                fuse -= 2f;      //10,  8,   6,   4,   [ignored last round] 10
                scale -= 1;      //4,   3,   2,   1,   [ignored last round] 75
                radius += 7f;    //4,   11,  18,  25   [ignored last round] 10
            }

            Log.Debug("Finished Grenade Coroutine.");
            yield break;
        }

        protected override void OnFinished()
        {
            if (_grenadeCoroutineHandle.IsRunning)
            {
                KillLoop = true;
                Timing.CallDelayed(1.2f, () => {
                    if (_grenadeCoroutineHandle.IsRunning)
                    {
                        Timing.KillCoroutines(new CoroutineHandle[] { _grenadeCoroutineHandle });
                    }
                });
            }
            var time = $"{EventTime.Minutes:00}:{EventTime.Seconds:00}";
            int count = Player.GetPlayers().Count(r => r.IsAlive && r.Role != RoleTypeId.ChaosConscript);
            if (count > 1)
            {
                Extensions.Broadcast(Translation.MorePlayer.Replace("{count}", $"{Player.GetPlayers().Count(r => r.Role != RoleTypeId.ChaosConscript)}").Replace("{time}", time), 10);
            }
            else if (count == 1)
            {
                var player = Player.GetPlayers().First(r => r.IsAlive && r.Role != RoleTypeId.ChaosConscript);
                player.Health = 1000;
                Extensions.Broadcast(Translation.OnePlayer.Replace("{winner}", player.Nickname).Replace("{time}", time), 10);
            }
            else
            {
                Extensions.Broadcast(Translation.AllDie.Replace("{time}", time), 10);
            }
        }
    }
}
