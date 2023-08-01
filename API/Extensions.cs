using System;
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
        public static ReferenceHub Dummy;
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
            if (Dummy == null)
            {
                var newPlayer = Object.Instantiate(NetworkManager.singleton.playerPrefab);
                var fakeConnection = new FakeConnection(0); // ?
                var hubPlayer = newPlayer.GetComponent<ReferenceHub>();
                //hubPlayer.Network_playerId = new RecyclablePlayerId(0); // ?
                NetworkServer.AddPlayerForConnection(fakeConnection, newPlayer);
                Dummy = hubPlayer;

                hubPlayer.characterClassManager.InstanceMode = ClientInstanceMode.Unverified;

                try
                {
                    hubPlayer.nicknameSync.SetNick(eventName);
                }
                catch (Exception) { }

                var path = Path.Combine(Path.Combine(Paths.Configs, "Music"), audioFile);

                var audioPlayer = AudioPlayerBase.Get(hubPlayer);
                audioPlayer.Enqueue(path, -1);
                audioPlayer.LogDebug = false;
                audioPlayer.BroadcastChannel = VoiceChatChannel.Intercom;
                audioPlayer.Volume = volume;
                audioPlayer.Loop = loop;
                audioPlayer.Play(0);
            }
        }

        public static void StopAudio()
        {
            if (Dummy != null)
            {
                var audioPlayer = AudioPlayerBase.Get(Dummy);

                if (audioPlayer.CurrentPlay != null)
                {
                    audioPlayer.Stoptrack(true);
                    audioPlayer.OnDestroy();
                }

                Dummy.OnDestroy();
                CustomNetworkManager.TypedSingleton.OnServerDisconnect(Dummy.connectionToClient);
                Object.Destroy(Dummy.gameObject);
            }
        }

        public static bool IsExistsMap(string schematicName)
        {
            var data = MapUtils.GetSchematicDataByName(schematicName);
            if (data == null)
            {
                return false;
            }
            else
            {
                return true;
            }
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
