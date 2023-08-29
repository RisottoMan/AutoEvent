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
            // Only one audio bot is used for mini games. If it doesn't exist, then we create it.
            if (Dummy == null) Dummy = AddDummy();

            // If someone started the music, then we will turn it off.
            StopAudio();

            // Looking for a way to the music we want to launch
            var path = Path.Combine(Path.Combine(Paths.Configs, "Music"), audioFile);

            // The bot is already in the game, it remains to start the music.
            var audioPlayer = AudioPlayerBase.Get(Dummy);
            audioPlayer.Enqueue(path, -1);
            audioPlayer.LogDebug = false;
            audioPlayer.BroadcastChannel = VoiceChatChannel.Intercom;
            audioPlayer.Volume = volume * (AutoEvent.Singleton.Config.Volume/100f);
            audioPlayer.Loop = loop;
            audioPlayer.Play(0);
        }

        public static void StopAudio()
        {
            var audioPlayer = AudioPlayerBase.Get(Dummy);

            if (audioPlayer.CurrentPlay != null)
            {
                audioPlayer.Stoptrack(true);
                audioPlayer.OnDestroy();
            }
        }

        public static ReferenceHub AddDummy()
        {
            var newPlayer = Object.Instantiate(NetworkManager.singleton.playerPrefab);
            var fakeConnection = new FakeConnection(0);
            var hubPlayer = newPlayer.GetComponent<ReferenceHub>();
            NetworkServer.AddPlayerForConnection(fakeConnection, newPlayer);
            hubPlayer.characterClassManager.InstanceMode = ClientInstanceMode.Unverified;

            // I can't change the name after creating dummy
            try
            {
                hubPlayer.nicknameSync.SetNick("MiniGames");
            }
            catch (Exception) { }

            return hubPlayer;
        }

        public static void RemoveDummy()
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
