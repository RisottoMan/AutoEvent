// -----------------------------------------------------------------------
// <copyright file="MirrorExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using CustomPlayerEffects;
using NorthwoodLib.Pools;
using PluginAPI.Core;

namespace AutoEvent.API;

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;
    
    using InventorySystem.Items.Firearms;

    using Mirror;

    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.PlayableScps.Scp049.Zombies;
    using RelativePositioning;

    using Respawning;

    using UnityEngine;

    /// <summary>
    /// A set of extensions for <see cref="Mirror"/> Networking.
    /// </summary>
    public static class MirrorExtensions
    {
        private static readonly Dictionary<Type, MethodInfo> WriterExtensionsValue = new();
        private static readonly Dictionary<string, ulong> SyncVarDirtyBitsValue = new();
        private static readonly Dictionary<string, string> RpcFullNamesValue = new();

        private static readonly ReadOnlyDictionary<Type, MethodInfo> ReadOnlyWriterExtensionsValue =
            new(WriterExtensionsValue);

        private static readonly ReadOnlyDictionary<string, ulong> ReadOnlySyncVarDirtyBitsValue =
            new(SyncVarDirtyBitsValue);

        private static readonly ReadOnlyDictionary<string, string> ReadOnlyRpcFullNamesValue = new(RpcFullNamesValue);
        private static MethodInfo setDirtyBitsMethodInfoValue;
        private static MethodInfo sendSpawnMessageMethodInfoValue;
        private static MethodInfo bufferRpcMethodInfoValue;

        /// <summary>
        /// Gets <see cref="MethodInfo"/> corresponding to <see cref="Type"/>.
        /// </summary>
        public static ReadOnlyDictionary<Type, MethodInfo> WriterExtensions
        {
            get
            {
                if (WriterExtensionsValue.Count == 0)
                {
                    foreach (MethodInfo method in typeof(NetworkWriterExtensions).GetMethods()
                                 .Where(x => !x.IsGenericMethod && (x.GetParameters()?.Length == 2)))
                        WriterExtensionsValue.Add(
                            method.GetParameters().First(x => x.ParameterType != typeof(NetworkWriter)).ParameterType,
                            method);

                    Type fuckNorthwood =
                        Assembly.GetAssembly(typeof(RoleTypeId)).GetType("Mirror.GeneratedNetworkCode");
                    foreach (MethodInfo method in fuckNorthwood.GetMethods().Where(x =>
                                 !x.IsGenericMethod && (x.GetParameters()?.Length == 2) &&
                                 (x.ReturnType == typeof(void))))
                        WriterExtensionsValue.Add(
                            method.GetParameters().First(x => x.ParameterType != typeof(NetworkWriter)).ParameterType,
                            method);

                    foreach (Type serializer in typeof(ServerConsole).Assembly.GetTypes()
                                 .Where(x => x.Name.EndsWith("Serializer")))
                    {
                        foreach (MethodInfo method in serializer.GetMethods()
                                     .Where(x => (x.ReturnType == typeof(void)) && x.Name.StartsWith("Write")))
                            WriterExtensionsValue.Add(
                                method.GetParameters().First(x => x.ParameterType != typeof(NetworkWriter))
                                    .ParameterType, method);
                    }
                }

                return ReadOnlyWriterExtensionsValue;
            }
        }

        /// <summary>
        /// Gets a all DirtyBit <see cref="ulong"/> from <see cref="StringExtensions"/>(format:classname.methodname).
        /// </summary>
        public static ReadOnlyDictionary<string, ulong> SyncVarDirtyBits
        {
            get
            {
                if (SyncVarDirtyBitsValue.Count == 0)
                {
                    foreach (PropertyInfo property in typeof(ServerConsole).Assembly.GetTypes()
                                 .SelectMany(x => x.GetProperties())
                                 .Where(m => m.Name.StartsWith("Network")))
                    {
                        MethodInfo setMethod = property.GetSetMethod();

                        if (setMethod is null)
                            continue;

                        MethodBody methodBody = setMethod.GetMethodBody();

                        if (methodBody is null)
                            continue;

                        byte[] bytecodes = methodBody.GetILAsByteArray();

                        if (!SyncVarDirtyBitsValue.ContainsKey($"{property.ReflectedType.Name}.{property.Name}"))
                            SyncVarDirtyBitsValue.Add($"{property.ReflectedType.Name}.{property.Name}",
                                bytecodes[bytecodes.LastIndexOf((byte)OpCodes.Ldc_I8.Value) + 1]);
                    }
                }

                return ReadOnlySyncVarDirtyBitsValue;
            }
        }

        /// <summary>
        /// Gets Rpc's FullName <see cref="string"/> corresponding to <see cref="StringExtensions"/>(format:classname.methodname).
        /// </summary>
        public static ReadOnlyDictionary<string, string> RpcFullNames
        {
            get
            {
                if (RpcFullNamesValue.Count == 0)
                {
                    foreach (MethodInfo method in typeof(ServerConsole).Assembly.GetTypes()
                                 .SelectMany(x =>
                                     x.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                                 .Where(m => m.GetCustomAttributes(typeof(ClientRpcAttribute), false).Length > 0 ||
                                             m.GetCustomAttributes(typeof(TargetRpcAttribute), false).Length > 0))
                    {
                        MethodBody methodBody = method.GetMethodBody();

                        if (methodBody is null)
                            continue;

                        byte[] bytecodes = methodBody.GetILAsByteArray();

                        if (!RpcFullNamesValue.ContainsKey($"{method.ReflectedType.Name}.{method.Name}"))
                            RpcFullNamesValue.Add($"{method.ReflectedType.Name}.{method.Name}",
                                method.Module.ResolveString(BitConverter.ToInt32(bytecodes,
                                    bytecodes.IndexOf((byte)OpCodes.Ldstr.Value) + 1)));
                    }
                }

                return ReadOnlyRpcFullNamesValue;
            }
        }

        /// <summary>
        /// Gets a <see cref="NetworkBehaviour.SetSyncVarDirtyBit(ulong)"/>'s <see cref="MethodInfo"/>.
        /// </summary>
        public static MethodInfo SetDirtyBitsMethodInfo => setDirtyBitsMethodInfoValue ??=
            typeof(NetworkBehaviour).GetMethod(nameof(NetworkBehaviour.SetSyncVarDirtyBit));

        /// <summary>
        /// Gets a NetworkServer.SendSpawnMessage's <see cref="MethodInfo"/>.
        /// </summary>
        public static MethodInfo SendSpawnMessageMethodInfo => sendSpawnMessageMethodInfoValue ??=
            typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.NonPublic | BindingFlags.Static);

        /// <summary>
        /// Gets a NetworkConnectionToClient.BufferRpc's <see cref="MethodInfo"/>.
        /// </summary>
        public static MethodInfo BufferRpcMethodInfo => bufferRpcMethodInfoValue ??=
            typeof(NetworkConnectionToClient).GetMethod("BufferRpc", BindingFlags.NonPublic | BindingFlags.Instance,
                null, CallingConventions.HasThis, new Type[] { typeof(RpcMessage), typeof(int) }, null);

        /// <summary>
        /// Force resync to client's <see cref="SyncVarAttribute"/>.
        /// </summary>
        /// <param name="behaviorOwner"><see cref="NetworkIdentity"/> of object that owns <see cref="NetworkBehaviour"/>.</param>
        /// <param name="targetType"><see cref="NetworkBehaviour"/>'s type.</param>
        /// <param name="propertyName">Property name starting with Network.</param>
        public static void ResyncSyncVar(NetworkIdentity behaviorOwner, Type targetType, string propertyName) =>
            SetDirtyBitsMethodInfo.Invoke(behaviorOwner.gameObject.GetComponent(targetType),
                new object[] { SyncVarDirtyBits[$"{targetType.Name}.{propertyName}"] });

        /// <summary>
        /// Send fake values to client's <see cref="ClientRpcAttribute"/>.
        /// </summary>
        /// <param name="target">Target to send.</param>
        /// <param name="behaviorOwner"><see cref="NetworkIdentity"/> of object that owns <see cref="NetworkBehaviour"/>.</param>
        /// <param name="targetType"><see cref="NetworkBehaviour"/>'s type.</param>
        /// <param name="rpcName">Property name starting with Rpc.</param>
        /// <param name="values">Values of send to target.</param>
        public static void SendFakeTargetRpc(Player target, NetworkIdentity behaviorOwner, Type targetType,
            string rpcName, params object[] values)
        {
            NetworkWriterPooled writer = NetworkWriterPool.Get();

            foreach (object value in values)
                WriterExtensions[value.GetType()].Invoke(null, new[] { writer, value });

            RpcMessage msg = new()
            {
                netId = behaviorOwner.netId,
                componentIndex = (byte)GetComponentIndex(behaviorOwner, targetType),
                functionHash = (ushort)RpcFullNames[$"{targetType.Name}.{rpcName}"].GetStableHashCode(),
                payload = writer.ToArraySegment(),
            };

            BufferRpcMethodInfo.Invoke(target.Connection, new object[] { msg, 0 });

            NetworkWriterPool.Return(writer);
        }

        /// <summary>
        /// Send fake values to client's <see cref="SyncObject"/>.
        /// </summary>
        /// <param name="target">Target to send.</param>
        /// <param name="behaviorOwner"><see cref="NetworkIdentity"/> of object that owns <see cref="NetworkBehaviour"/>.</param>
        /// <param name="targetType"><see cref="NetworkBehaviour"/>'s type.</param>
        /// <param name="customAction">Custom writing action.</param>
        /// <example>
        /// EffectOnlySCP207.
        /// <code>
        ///  MirrorExtensions.SendCustomSync(player, player.ReferenceHub.networkIdentity, typeof(PlayerEffectsController), (writer) => {
        ///   writer.WriteUInt64(1ul);                                           // DirtyObjectsBit
        ///   writer.WriteUInt32(1);                                             // DirtyIndexCount
        ///   writer.WriteByte((byte)SyncList&lt;byte&gt;.Operation.OP_SET);     // Operations
        ///   writer.WriteUInt32(17);                                            // EditIndex
        ///   writer.WriteByte(1);                                               // Value
        ///  });
        /// </code>
        /// </example>
        public static void SendFakeSyncObject(Player target, NetworkIdentity behaviorOwner, Type targetType,
            Action<NetworkWriter> customAction)
        {
            NetworkWriterPooled writer = NetworkWriterPool.Get();
            NetworkWriterPooled writer2 = NetworkWriterPool.Get();
            MakeCustomSyncWriter(behaviorOwner, targetType, customAction, null, writer, writer2);
            target.ReferenceHub.networkIdentity.connectionToClient.Send(new EntityStateMessage()
                { netId = behaviorOwner.netId, payload = writer.ToArraySegment() });
            NetworkWriterPool.Return(writer);
            NetworkWriterPool.Return(writer2);
        }

        /// <summary>
        /// Edit <see cref="NetworkIdentity"/>'s parameter and sync.
        /// </summary>
        /// <param name="identity">Target object.</param>
        /// <param name="customAction">Edit function.</param>
        public static void EditNetworkObject(NetworkIdentity identity, Action<NetworkIdentity> customAction)
        {
            customAction.Invoke(identity);

            ObjectDestroyMessage objectDestroyMessage = new()
            {
                netId = identity.netId,
            };

            foreach (Player player in Player.GetPlayers())
            {
                player.Connection.Send(objectDestroyMessage, 0);
                SendSpawnMessageMethodInfo.Invoke(null, new object[] { identity, player.Connection });
            }
        }

        // Get components index in identity.(private)
        private static int GetComponentIndex(NetworkIdentity identity, Type type)
        {
            return Array.FindIndex(identity.NetworkBehaviours, (x) => x.GetType() == type);
        }

        // Make custom writer(private)
        private static void MakeCustomSyncWriter(NetworkIdentity behaviorOwner, Type targetType,
            Action<NetworkWriter> customSyncObject, Action<NetworkWriter> customSyncVar, NetworkWriter owner,
            NetworkWriter observer)
        {
            ulong value = 0;
            NetworkBehaviour behaviour = null;

            // Get NetworkBehaviors index (behaviorDirty use index)
            for (int i = 0; i < behaviorOwner.NetworkBehaviours.Length; i++)
            {
                if (behaviorOwner.NetworkBehaviours[i].GetType() == targetType)
                {
                    behaviour = behaviorOwner.NetworkBehaviours[i];
                    value = 1UL << (i & 31);
                    break;
                }
            }

            // Write target NetworkBehavior's dirty
            Compression.CompressVarUInt(owner, value);

            // Write init position
            int position = owner.Position;
            owner.WriteByte(0);
            int position2 = owner.Position;

            // Write custom sync data
            if (customSyncObject is not null)
                customSyncObject(owner);
            else
                behaviour.SerializeObjectsDelta(owner);

            // Write custom syncvar
            customSyncVar?.Invoke(owner);

            // Write syncdata position data
            int position3 = owner.Position;
            owner.Position = position;
            owner.WriteByte((byte)(position3 - position2 & 255));
            owner.Position = position3;

            // Copy owner to observer
            if (behaviour.syncMode != SyncMode.Observers)
                observer.WriteBytes(owner.ToArraySegment().Array, position, owner.Position - position);
        }
    }