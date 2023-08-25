using System;
using System.IO;
using Mirror;
using UnityEngine;
using PlayerRoles;
using SCPSLAudioApi.AudioCore;
using VoiceChat;
using AutoEvent.API.Schematic.Objects;
using AutoEvent.API.Schematic;
using PluginAPI.Core;
using InventorySystem.Items.Pickups;
using PlayerStatsSystem;
using PluginAPI.Helpers;
using Object = UnityEngine.Object;
using System.Reflection;

namespace AutoEvent
{
    public class Extensions
    {
        public static ReferenceHub Dummy = new ReferenceHub();

        private static MethodInfo sendSpawnMessage;
        public static MethodInfo SendSpawnMessage => sendSpawnMessage ?? (sendSpawnMessage = typeof(NetworkServer).
            GetMethod("SendSpawnMessage", BindingFlags.Static | BindingFlags.NonPublic));

        public static void SetRole(Player player, RoleTypeId newRole, RoleSpawnFlags spawnFlags)
        {
            player.ReferenceHub.roleManager.ServerSetRole(newRole, RoleChangeReason.RemoteAdmin, spawnFlags);
        }

        public static void SetRole(Player player, RoleTypeId newRole, RoleChangeReason reason, RoleSpawnFlags spawnFlags)
        {
            player.ReferenceHub.roleManager.ServerSetRole(newRole, reason, spawnFlags);
        }

        public static void SetPlayerScale(Player target, Vector3 scale)
        {
            if (target.GameObject.transform.localScale == scale) return;

            try
            {
                NetworkIdentity identity = target.ReferenceHub.networkIdentity;
                target.GameObject.transform.localScale = scale;
                foreach (Player player in Player.GetPlayers())
                {
                    SendSpawnMessage?.Invoke(null, new object[2] { identity, player.Connection });
                }
            }
            catch (Exception ex)
            {
                Log.Info($"Scale error: {ex}");
            }
        }

        public static void SetPlayerAhp(Player player, float amount, float limit = 75, float decay = 1.2f, float efficacy = 0.7f, float sustain = 0, bool persistant = false)
        {
            player.ReferenceHub.playerStats.GetModule<AhpStat>().ServerAddProcess(amount, limit, decay, efficacy, sustain, persistant);
        }

        public static void TeleportEnd()
        {
            foreach (Player player in Player.GetPlayers())
            {
                player.SetRole(RoleTypeId.Tutorial, RoleChangeReason.None);
                player.Position = new Vector3(39.332f, 1014.766f, -31.922f);
            }
        }

        public static void PlayAudio(string audioFile, byte volume, bool loop, string eventName)
        {
            return;
            Dummy = AddDummy();

            StopAudio();

            var path = Path.Combine(Path.Combine(Paths.GlobalPlugins.Plugins, "Music"), audioFile);

            var audioPlayer = AudioPlayerBase.Get(Dummy);
            audioPlayer.Enqueue(path, -1);
            audioPlayer.LogDebug = false;
            audioPlayer.BroadcastChannel = VoiceChatChannel.Intercom;
            audioPlayer.Volume = volume;
            audioPlayer.Loop = loop;
            audioPlayer.Play(0);
        }

        public static void StopAudio()
        {
            return;
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

        public static SchematicObject LoadMap(string nameSchematic, Vector3 pos, Quaternion rot, Vector3 scale)
        {
            return ObjectSpawner.SpawnSchematic(nameSchematic, pos, rot, scale);
        }

        public static void UnLoadMap(SchematicObject scheme)
        {
            scheme.Destroy();
        }

        public static void CleanUpAll()
        {
            foreach (var item in Object.FindObjectsOfType<ItemPickupBase>())
            {
                GameObject.Destroy(item.gameObject);
            }

            foreach (var ragdoll in Object.FindObjectsOfType<BasicRagdoll>())
            {
                GameObject.Destroy(ragdoll.gameObject);
            }
        }

        public static void Broadcast(string text, ushort time)
        {
            Map.ClearBroadcasts();
            Map.Broadcast(time, text);
        }

        public static bool IsAudioBot(ReferenceHub hub)
        {
            return Dummy == hub;
        }
    }
}
