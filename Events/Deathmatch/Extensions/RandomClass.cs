using System.Collections.Generic;
using UnityEngine;

namespace AutoEvent.Events.Deathmatch
{
    internal class RandomClass // нужно исправить под динамические спавны
    {
        private static readonly List<Vector3> spawnPos = new()
        {
            new Vector3(-17.34f, 1.26f, 5.4f),
            new Vector3(-2.748f, 1.26f, -10.362f),
            new Vector3(-11.934f, 1.26f, -0.924f),
            new Vector3(-6.324f, 1.26f, 10.14f),
            new Vector3(0.948f, 1.26f, -4.734f),
            new Vector3(0.948f, 1.26f, 4.242f),
            new Vector3(-5.97f, 1.26f, 0.204f),
            new Vector3(4.752f, 3.792f, 2.64f),
            new Vector3(-5.598f, 3.708f, -4.842f),
            new Vector3(-18.198f, 1.26f, 1.092f),
            new Vector3(-17.61f, 1.26f, -8.604f),
            new Vector3(-12.372f, 1.26f, -15.234f),
            new Vector3(0.33f, 1.26f, -17.46f),
            new Vector3(10.32f, 1.26f, -13.578f),
            new Vector3(13.782f, 1.26f, -14.07f),
            new Vector3(18.882f, 1.26f, -16.422f),
            new Vector3(18.882f, 1.26f, -5.052f),
            new Vector3(10.5f, 1.26f, -0.18f),
            new Vector3(16.182f, 1.26f, -0.18f),
            new Vector3(18.642f, 1.26f, 6.402f),
            new Vector3(17.202f, 1.26f, 13.044f),
            new Vector3(17.202f, 1.26f, 16.326f),
            new Vector3(5.808f, 1.26f, 13.416f),
            new Vector3(1.9266f, 1.26f, 15.564f),
            new Vector3(-15.066f, 1.26f, 15.336f),
            new Vector3(-18.21f, 1.26f, 16.704f)
        };
        public static Vector3 GetRandomPosition() => spawnPos.RandomItem();
    }
}
