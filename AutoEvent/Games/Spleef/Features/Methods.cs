using AdminToys;
using System.Collections.Generic;
using System.Linq;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Serializable;
using UnityEngine;

namespace AutoEvent.Games.Spleef;
internal class Methods
{
    public static List<GameObject> GeneratePlatforms(Plugin plugin)
    {
        int amountPerAxis = plugin.Config.PlatformAxisCount;
        float areaSizeX = 20f;
        float areaSizeY = 20f;
        float sizeX = areaSizeX / amountPerAxis;
        float sizeY = areaSizeY / amountPerAxis;
        float startPosX = -(areaSizeX / 2f) + sizeX / 2f;
        float startPosY = -(areaSizeY / 2f) + sizeY / 2f;
        float startPosZ = 6f;
        float breakSize = .2f;
        float sizeZ = 3f;
        float spawnHeight = 6f;
        List<SpleefPlatform> platforms = new List<SpleefPlatform>();

        for (int z = 0; z < plugin.Config.LayerCount; z++)
        {
            for (int x = 0; x < amountPerAxis; x++)
            {
                for (int y = 0; y < amountPerAxis; y++)
                {
                    float posX = startPosX + (sizeX * x);
                    float posY = startPosY + (sizeY * y);
                    float posZ = startPosZ + (sizeZ * z);

                    Color color = Color.green;
                    switch (z)
                    {
                        case 0: color = Color.red; break;
                        case 1: color = Color.magenta; break;
                        case 2: color = Color.cyan; break;
                        case 3: color = Color.yellow; break;
                        case 4: color = Color.blue; break;
                    }

                    var plat = new SpleefPlatform(sizeX - breakSize, sizeY - breakSize, .3f, posX, posY, posZ, color);
                    platforms.Add(plat);
                    if (posZ > spawnHeight + 2)
                    {
                        spawnHeight = posZ + 2;
                    }
                }
            }
        }

        var primary = plugin.MapInfo.Map.AttachedBlocks.FirstOrDefault(x => x.name == "Parent-Platform");
        List<GameObject> platformes = new List<GameObject>();

        foreach (SpleefPlatform platform in platforms)
        {
            Vector3 position = plugin.MapInfo.Map.Position + new Vector3(platform.PositionX, platform.PositionZ, platform.PositionY);
            PrimitiveObjectToy primaryObjectToy = primary.GetComponent<PrimitiveObjectToy>();
            /* <<< 03.05.2025 Move from MER to MapEditorReborn
            PrimitiveObject newPlatform = ObjectSpawner.SpawnPrimitive(new PrimitiveSerializable()
            {
                PrimitiveType = primaryObjectToy.PrimitiveType,
                Position = position,
                Color = platform.Color.ToHex()
            },
            position,
            Quaternion.identity,
            new Vector3(platform.X, platform.Z, platform.Y));
            */
            var newPlatform = ObjectSpawner.SpawnPrimitive(new PrimitiveSerializable()
            {
                PrimitiveType = primaryObjectToy.PrimitiveType,
                Position = position,
                Scale = new Vector3(platform.X, platform.Z, platform.Y),
                Color = platform.Color.ToHex(),
            });
            // >>>

            newPlatform.gameObject.AddComponent<FallPlatformComponent>();
            platformes.Add(newPlatform.gameObject);
        }

        return platformes;
    }
}