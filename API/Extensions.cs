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
using Exiled.API.Enums;
using Object = UnityEngine.Object;

namespace AutoEvent
{
    internal class Extensions
    {
        public static List<ReferenceHub> Dummies = new List<ReferenceHub>();
        public static void TeleportEnd()
        {
            foreach (Player player in Player.List)
            {
                player.Role.Set(RoleTypeId.Tutorial, SpawnReason.None, RoleSpawnFlags.AssignInventory);
                player.Position = new Vector3(39.332f, 1014.766f, -31.922f);
            }
        }
        public static void PlayAudio(string audioFile, byte volume, bool loop, string eventName)
        {
            try
            {
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
        public static SchematicObject LoadMap(string nameSchematic , Vector3 pos, Quaternion rot, Vector3 scale)
        {
            return ObjectSpawner.SpawnSchematic(nameSchematic, pos, rot, scale);
        }
        public static void UnLoadMap(SchematicObject scheme)
        {
            scheme.Destroy();
        }
        public static void CleanUpAll()
        {
            Map.CleanAllItems();
            Map.CleanAllRagdolls();
        }
        public static void Broadcast(string text, ushort time)
        {
            Map.ClearBroadcasts();
            Map.Broadcast(new Exiled.API.Features.Broadcast(text, time));
        }
    }
}
