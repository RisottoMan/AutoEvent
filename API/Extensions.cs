using System;
using System.Collections.Generic;
using System.IO;
using Mirror;
using UnityEngine;
using PlayerRoles;
using Exiled.API.Features;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using SCPSLAudioApi.AudioCore;
using VoiceChat;
using Object = UnityEngine.Object;
using AutoEvent.Interfaces;
using System.Reflection;

namespace AutoEvent
{
    internal class Extensions
    {
        public static List<ReferenceHub> Dummies;
        /// <summary>Тп игроков после конца игры</summary>
        public static void TeleportEnd()
        {
            foreach (Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.Tutorial, Exiled.API.Enums.SpawnReason.None, RoleSpawnFlags.None);
                player.Position = new Vector3(39.332f, 1014.766f, -31.922f);
                player.ClearInventory();
            }
        }
        /// <summary>Тп игроков после конца игры</summary>
        public static void PlayAudio(string audioFile, byte volume, bool loop, string eventName)
        {
            try
            {
                Dummies = new List<ReferenceHub>();
                var newPlayer = Object.Instantiate(NetworkManager.singleton.playerPrefab);
                int id = Dummies.Count;
                var fakeConnection = new FakeConnection(id++);
                var hubPlayer = newPlayer.GetComponent<ReferenceHub>();
                Dummies.Add(hubPlayer);
                NetworkServer.AddPlayerForConnection(fakeConnection, newPlayer);

                hubPlayer.characterClassManager.InstanceMode = ClientInstanceMode.Unverified;

                try
                {
                    hubPlayer.nicknameSync.SetNick(eventName);
                }
                catch (Exception) { }

                var audioPlayer = AudioPlayerBase.Get(hubPlayer);

                var path = Path.Combine(Path.Combine(Paths.Configs, "Music"), audioFile);

                audioPlayer.Enqueue(path, -1);
                audioPlayer.LogDebug = false;
                audioPlayer.BroadcastChannel = VoiceChatChannel.Intercom;
                audioPlayer.Volume = volume;
                audioPlayer.Loop = loop;
                audioPlayer.Play(0);
                Log.Debug($"Playing sound {path}");
            }
            catch (Exception e)
            {
                Log.Error($"Error on: {e.Data} -- {e.StackTrace}");
            }
        }
        /// <summary>Остановить прогирывание</summary>
        public static void StopAudio()
        {
            foreach (var dummies in Dummies)
            {
                var audioPlayer = AudioPlayerBase.Get(dummies);

                if(audioPlayer.CurrentPlay != null)
                {
                    audioPlayer.Stoptrack(true);
                    audioPlayer.OnDestroy();
                }

                NetworkConnectionToClient conn = dummies.connectionToClient;
                dummies.OnDestroy();
                CustomNetworkManager.TypedSingleton.OnServerDisconnect(conn);
                Object.Destroy(dummies.gameObject);
            }
            Dummies.Clear();
        }
        /// <summary>Загрузить карту</summary>
        public static SchematicObject LoadMap(string nameSchematic , Vector3 pos, Quaternion rot, Vector3 scale)
        {
            return ObjectSpawner.SpawnSchematic(nameSchematic, pos, rot, scale);
        }
        /// <summary>Удалить карту</summary>
        public static void UnLoadMap(SchematicObject scheme)
        {
            scheme.Destroy();
        }
        /// <summary>Очистка мусора после ивента.</summary>
        public static void CleanUpAll()
        {
            Map.CleanAllItems();
            Map.CleanAllRagdolls();
        }
        /// <summary>Небольшой бродкаст с очисткой других бродкастов</summary>
        public static void Broadcast(string text, ushort time)
        {
            Map.ClearBroadcasts();
            Map.Broadcast(new Exiled.API.Features.Broadcast(text, time));
        }
        public static string GetEvent(string EventName, CommandSender sender)
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.Namespace.Contains("AutoEvent") && !type.IsInterface && typeof(IEvent).IsAssignableFrom(type) && type.GetProperty("CommandName") != null)
                {
                    var ev = Activator.CreateInstance(type);
                    try
                    {
                        if ((string)type.GetProperty("CommandName").GetValue(ev) == EventName)
                        {
                            var eng = type.GetMethod("OnStart");
                            if (eng != null)
                            {
                                sender.Respond("Trying to run an event, OnStart is not null...");
                                eng.Invoke(Activator.CreateInstance(type), null);
                                Round.IsLocked = true;
                                AutoEvent.ActiveEvent = (IEvent)ev;
                                return "The event is found, run it.";
                            }
                            return "Somehow, the class that was selected does not have OnStart() in it";
                        }
                    }
                    catch (Exception ex)
                    {
                        return $"An error occurred when running the event. Error: {ex.Message}";
                    }
                }
            }

            return "The event was not found, nothing happened.";
        }
    }
}
