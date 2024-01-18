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
        public static IBossState GetRandomState(IBossState _prevState)
        {
            List<Type> _eventStates = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IBossState)) && !t.IsInterface).ToList();
            List<IBossState> newList = new List<IBossState>();
            foreach(Type type in _eventStates)
            {
                object activeType = Activator.CreateInstance(type);
                if (activeType is IBossState bossState)
                {
                    if (_prevState != bossState && bossState.Name != "Waiting")
                        newList.Add(bossState);
                }
            }

            return newList.RandomItem();
        }

        public static SchematicObject CreateSchematicBoss()
        {
            return ObjectSpawner.SpawnSchematic("SantaClaus",  new Vector3(0, -2.244f, 0), Quaternion.identity, new Vector3(1f, 1f, 1f));
        }
    }
}
