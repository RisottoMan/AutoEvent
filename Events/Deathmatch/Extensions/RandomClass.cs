using UnityEngine;
using Random = UnityEngine.Random;

namespace AutoEvent.Events.Deathmatch
{
    internal class RandomClass
    {
        public static Vector3 GetRandomPosition()
        {
            Vector3 position = new Vector3(0, 0, 0);
            switch (Random.Range(0, 26))
            {
                case 0: position = new Vector3(-17.34f, 1.26f, 5.4f); break;
                case 1: position = new Vector3(-2.748f, 1.26f, -10.362f); break;
                case 2: position = new Vector3(-11.934f, 1.26f, -0.924f); break;
                case 3: position = new Vector3(-6.324f, 1.26f, 10.14f); break;
                case 4: position = new Vector3(0.948f, 1.26f, -4.734f); break;
                case 5: position = new Vector3(0.948f, 1.26f, 4.242f); break;
                case 6: position = new Vector3(-5.97f, 1.26f, 0.204f); break;
                case 7: position = new Vector3(4.752f, 3.792f, 2.64f); break;
                case 8: position = new Vector3(-5.598f, 3.708f, -4.842f); break;
                case 9: position = new Vector3(-18.198f, 1.26f, 1.092f); break;
                case 10: position = new Vector3(-17.61f, 1.26f, -8.604f); break;
                case 11: position = new Vector3(-12.372f, 1.26f, -15.234f); break;
                case 12: position = new Vector3(0.33f, 1.26f, -17.46f); break;
                case 13: position = new Vector3(10.32f, 1.26f, -13.578f); break;
                case 14: position = new Vector3(13.782f, 1.26f, -14.07f); break;
                case 15: position = new Vector3(18.882f, 1.26f, -16.422f); break;
                case 16: position = new Vector3(18.882f, 1.26f, -5.052f); break;
                case 17: position = new Vector3(10.5f, 1.26f, -0.18f); break;
                case 18: position = new Vector3(16.182f, 1.26f, - 0.18f); break;
                case 19: position = new Vector3(18.642f, 1.26f, 6.402f); break;
                case 20: position = new Vector3(17.202f, 1.26f, 13.044f); break;
                case 21: position = new Vector3(17.202f, 1.26f, 16.326f); break;
                case 22: position = new Vector3(5.808f, 1.26f, 13.416f); break;
                case 23: position = new Vector3(1.9266f, 1.26f, 15.564f); break;
                case 24: position = new Vector3(-15.066f, 1.26f, 15.336f); break;
                case 25: position = new Vector3(-18.21f, 1.26f, 16.704f); break;
            }
            return position;
        }
    }
}
