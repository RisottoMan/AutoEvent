using AutoEvent.API.Schematic.Objects;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Games.HideAndSeek.Features
{
    internal class RandomClass
    {
        public static Vector3 GetSpawnPosition(SchematicObject GameMap)
        {
            return GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint").ToList().RandomItem().transform.position;
        }

        public static int GetCatchByCount(int playerCount)
        {
            int catchCount = 0;
            switch (playerCount)
            {
                case int n when (n > 0 && n <= 3): catchCount = 1; break;
                case int n when (n > 3 && n <= 5): catchCount = 2; break;
                case int n when (n > 5 && n <= 10): catchCount = 3; break;
                case int n when (n > 10 && n <= 15): catchCount = 5; break;
                case int n when (n > 15 && n <= 20): catchCount = 8; break;
                case int n when (n > 20 && n <= 25): catchCount = 10; break;
                case int n when (n > 25): catchCount = n / 2; break;
            }

            return catchCount;
        }
    }
}
