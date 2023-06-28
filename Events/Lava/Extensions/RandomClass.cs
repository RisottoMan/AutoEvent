using System.Collections.Generic;
using UnityEngine;

namespace AutoEvent.Events.Lava
{
    internal class RandomClass
    {
        public static readonly List<Vector3> spawnPos = new()
        {
             new Vector3(-13.43136f, 2.6f, 25.6032f),
             new Vector3(2.91f, 2.6f, 16.35f),
             new Vector3(18.59f, 2.6f, 23.39f),
             new Vector3(27.92f, 2.6f, 7.71f),
             new Vector3(15.34f, 2.6f, -0.27f),
             new Vector3(18.02f, 2.6f, -23.75f),
             new Vector3(9.88f, 2.6f, -12.95f),
             new Vector3(0.11f, 2.6f, -16.04f),
             new Vector3(-13.26f, 2.6f, -14.57f),
             new Vector3(-26.03f, 2.6f, -9.07f),
             new Vector3(-14.85f, 2.6f, 4.35f),
             new Vector3(-24.42f, 2.6f, 16.25f),
             new Vector3(-3.49f, 2.6f, 29.02f)
        };
        public static Vector3 GetRandomPosition() => spawnPos.RandomItem();

        public static readonly List<Vector3> gunPos = new()
        {
            new Vector3(-16.077f, 5.86f, 22.593f),
            new Vector3(5.484f, 10.339f, 22.564f),
            new Vector3(22.36f, 4.73f, 8.22f),
            new Vector3(18.904f, 9.852f, 11.129f),
            new Vector3(0.2160301f, 15.04f, 0.1276894f),
            new Vector3(9.688f, 19.16f, -24.429f),
            new Vector3(-16.69451f, 4.02f, -24.33174f),
            new Vector3(-20.75f, 4.36f, -5.99f),
            new Vector3(25.351f, 7.429f, -6.61f),
            new Vector3(24.05f, 13.53f, -11.1f),
            new Vector3(9.58f, 13.53f, -22.4f),
            new Vector3(-24.58f, 6.43f, 4.68f)
        };
        public static Vector3 GetRandomGun() => gunPos.RandomItem();
    }
}
