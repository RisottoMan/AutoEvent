using System.Collections.Generic;
using UnityEngine;

namespace AutoEvent.Events.Survival
{
    public class RandomClass
    {
        public static Vector3 GetRandomSpawn()
        {
            return Spawn.RandomItem();
        }
        public static List<Vector3> Spawn = new List<Vector3>()
        {
            new Vector3(-68, 8, 0),
            new Vector3(68, 8, 0),
            new Vector3(0, 8, 68),
            new Vector3(0, 8, -68),
        };
    }
}
