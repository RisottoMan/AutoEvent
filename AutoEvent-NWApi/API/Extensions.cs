using System;
using System.IO;
using Mirror;
using UnityEngine;
using PlayerRoles;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using SCPSLAudioApi.AudioCore;
using VoiceChat;
using Object = UnityEngine.Object;
using PluginAPI.Core;
using PluginAPI.Helpers;

namespace AutoEvent
{
    internal class Extensions
    {
        public static ReferenceHub Dummy;
        public static void TeleportEnd()
        {
            foreach (Player player in Player.GetPlayers())
            {
                player.SetRole(RoleTypeId.Tutorial, RoleChangeReason.None);
                //player.IsWithoutItems = true;
                player.Position = new Vector3(39.332f, 1014.766f, -31.922f);
            }
        }

        public static void PlayAudio(string audioFile, byte volume, bool loop, string eventName)
        {
            if (Dummy == null)
            {
                var newPlayer = Object.Instantiate(NetworkManager.singleton.playerPrefab);
                var fakeConnection = new FakeConnection(0);
                var hubPlayer = newPlayer.GetComponent<ReferenceHub>();
                NetworkServer.AddPlayerForConnection(fakeConnection, newPlayer);
                Dummy = hubPlayer;

                hubPlayer.characterClassManager.InstanceMode = ClientInstanceMode.Unverified;

                try
                {
                    hubPlayer.nicknameSync.SetNick(eventName);
                }
                catch (Exception) { }

                var path = Path.Combine(Path.Combine(Paths.GlobalPlugins.Plugins, "Music"), audioFile);

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
            //Map.CleanAllItems();
            //Map.CleanAllRagdolls();
        }

        public static void Broadcast(string text, ushort time)
        {
            Map.ClearBroadcasts();
            Map.Broadcast(time, text);
        }
    }
}
