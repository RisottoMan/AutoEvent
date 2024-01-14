using MER.Lite;
using MER.Lite.Objects;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace AutoEvent.Games.Boss.Features
{
    public class Functions
    {
        private static StateEnum _previousState { get; set; }
        public static void GetRandomState() //StateEnum
        {
            // Не должно входить предыдущее состояние
            // Этим состоянием не должен быть Waiting

            //return new List<StateEnum>(Enum.GetValues(typeof(StateEnum)) as IEnumerable<StateEnum>);
        }

        public static SchematicObject CreateSchematicBoss()
        {
            return ObjectSpawner.SpawnSchematic("SantaClaus",  new Vector3(0, -2.244f, 0), Quaternion.identity, new Vector3(1f, 1f, 1f));
        }
    }
}
