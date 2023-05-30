using UnityEngine;
using Random = UnityEngine.Random;

namespace AutoEvent.Events.Jail
{
    public class JailRandom
    {
        public static Vector3 GetRandomPosition()
        {
            Vector3 position = new Vector3(0, 0, 0);
            switch (Random.Range(0, 15))
            {
                case 0: position = new Vector3(6.99f, -5.396f, 17.18f); break;
                case 1: position = new Vector3(14.36f, -5.396f, 17.18f); break;
                case 2: position = new Vector3(21.49f, -5.396f, 17.18f); break;
                case 3: position = new Vector3(28.82f, -5.396f, 17.18f); break;
                case 4: position = new Vector3(36.47f, -5.396f, 17.18f); break;
                case 5: position = new Vector3(6.99f, -8.686f, 17.18f); break;
                case 6: position = new Vector3(14.36f, -8.686f, 17.18f); break;
                case 7: position = new Vector3(21.49f, -8.686f, 17.18f); break;
                case 8: position = new Vector3(28.82f, -8.686f, 17.18f); break;
                case 9: position = new Vector3(36.47f, -8.686f, 17.18f); break;
                case 10: position = new Vector3(6.99f, -12, 17.18f); break;
                case 11: position = new Vector3(14.36f, -12, 17.18f); break;
                case 12: position = new Vector3(21.49f, -12, 17.18f); break;
                case 13: position = new Vector3(28.82f, -12, 17.18f); break;
                case 14: position = new Vector3(36.47f, -12, 17.18f); break;
            }
            return position;
        }
    }
}
