using Mirror;
using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Wrappers;
using ProjectMER.Features;
using ProjectMER.Features.Serializable;
using UnityEngine;

namespace AutoEvent.Games.MusicalChairs;
public class Functions
{
    public static List<GameObject> GeneratePlatforms(int count, GameObject parent, Vector3 position)
    {
        float radius = 0.35f * count;
        float angleCount = 360f / count;
        List<GameObject> platformes = new List<GameObject>();

        for (int i = 0; i < count; i++)
        {
            float angle = i * angleCount;
            float radians = angle * Mathf.Deg2Rad;

            float x = position.x + radius * Mathf.Cos(radians);
            float z = position.z + radius * Mathf.Sin(radians);
            Vector3 pos = new Vector3(x, parent.transform.position.y, z);
            /* <<< 03.05.2025 Move from MER to ProjectMER
            PrimitiveObject obj = ObjectSpawner.SpawnPrimitive(new SerializablePrimitive()
            {
                PrimitiveType = PrimitiveType.Cylinder,
                Position = parent.transform.position,
                Color = "yellow",
                Static = false
            },
            pos, 
            parent.transform.rotation, 
            parent.transform.localScale);
            */
            var obj = ObjectSpawner.SpawnPrimitive(new SerializablePrimitive()
            {
                PrimitiveType = PrimitiveType.Cylinder,
                Position = parent.transform.position,
                Scale =parent.transform.localScale,
                Color = "yellow",
            });
            // >>>
            NetworkServer.Spawn(obj.gameObject);
            platformes.Add(obj.gameObject);
        }

        return platformes;
    }

    public static List<GameObject> RearrangePlatforms(int playerCount, List<GameObject> platforms, Vector3 position)
    {
        if (platforms.Count == 0)
            return new List<GameObject>();

        for (int i = playerCount; i <= platforms.Count;)
        {
            GameObject lastPlatform = platforms.Last();
            Object.Destroy(lastPlatform);
            platforms.Remove(lastPlatform);
        }

        int count = platforms.Count;
        float radius = 0.35f * count;
        float angleCount = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = i * angleCount;
            float radians = angle * Mathf.Deg2Rad;

            float x = position.x + radius * Mathf.Cos(radians);
            float z = position.z + radius * Mathf.Sin(radians);
            Vector3 pos = new Vector3(x, platforms[i].transform.position.y, z);

            if (platforms[i].TryGetComponent(out PrimitiveObjectToy primitiveObject))
            {
                primitiveObject.Position = pos;
            }
        }

        return platforms;
    }
}
