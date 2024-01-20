using MER.Lite;
using MER.Lite.Objects;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Reflection;
using System.Linq;

namespace AutoEvent.Games.Boss.Features
{
    public class Functions
    {
        private static IBossState _previousRandomState = new WaitingState();
        public static IBossState GetRandomState(int stage)
        {
            var newList = new List<IBossState>();
            var types = Assembly.GetExecutingAssembly().GetTypes();
            var stateTypes = types.Where(t => t.GetInterfaces().Contains(typeof(IBossState)) && !t.IsInterface);

            foreach(Type type in stateTypes)
            {
                object activeType = Activator.CreateInstance(type);
                if (activeType is IBossState state)
                {
                    if (state.Name != _previousRandomState.Name &&
                        state.Name != "WaitingState" &&
                        state.Stage <= stage)
                        newList.Add(state);
                }
            }

            var newItem = newList.RandomItem();
            DebugLogger.LogDebug($"Last state = {_previousRandomState.Name}; New state = {newItem.Name}");
            _previousRandomState = newItem;

            return newItem;
        }

        public static SchematicObject CreateSchematicBoss()
        {
            return ObjectSpawner.SpawnSchematic("SantaClaus",  new Vector3(0, -2.244f, 0), Quaternion.identity, new Vector3(1f, 1f, 1f));
        }
    }
}
