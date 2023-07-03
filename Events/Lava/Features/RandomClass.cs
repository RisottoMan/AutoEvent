using MapEditorReborn.API.Features.Objects;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events.Lava.Features
{
    internal class RandomClass
    {
        public static Vector3 GetSpawnPosition(SchematicObject GameMap)
        {
            return GameMap.AttachedBlocks.Where(x => x.name == "Spawnpoint").ToList().RandomItem().transform.position;
        }
        public static Vector3 GetRandomGun(SchematicObject GameMap)
        {
            return GameMap.AttachedBlocks.Where(x => x.name == "Spawngun").ToList().RandomItem().transform.position;
        }
        /*
        // Code for random guns from area
        for (int i = 0; i < 20; i++)
        {
            var item = ItemType.GunCOM15;
            var rand = Random.Range(0, 100);
            if (rand < 40) item = ItemType.GunCOM15;
            else if (rand >= 40 && rand < 80) item = ItemType.GunCOM18;
            else if (rand >= 80 && rand < 90) item = ItemType.GunRevolver;
            else if (rand >= 90 && rand < 100) item = ItemType.GunFSP9;
            Pickup pickup = new Item(item).Spawn(GameMap.GameObject.transform.position + new Vector3(Random.Range(-30, 31), 30, Random.Range(-30, 31)));
        }
        */
    }
}
