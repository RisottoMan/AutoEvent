using AdminToys;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Games.MusicalChairs
{
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

                GameObject child = GameObject.Instantiate(parent, pos, Quaternion.identity);
                
                if (child.TryGetComponent(out PrimitiveObjectToy toy))
                {
                    if (child.TryGetComponent(out Collider collider))
                    {
                        collider.enabled = false;
                        DebugLogger.LogDebug("Collider");
                    }

                    if (child.TryGetComponent(out SphereCollider sphere))
                    {
                        sphere.enabled = false;
                        DebugLogger.LogDebug("Sphere Collider");
                    }

                    if (child.TryGetComponent(out CapsuleCollider capsule))
                    {
                        capsule.enabled = false;
                        DebugLogger.LogDebug("Capsule Collider");
                    }
                }

                NetworkServer.Spawn(child);
                platformes.Add(child);
            }

            return platformes;
        }

        public static List<GameObject> RearrangePlatforms(int playerCount, List<GameObject> platforms, Vector3 position)
        {
            for (int i = playerCount; i <= platforms.Count;)
            {
                GameObject.Destroy(platforms.Last());
                platforms.Remove(platforms.Last());
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

                platforms[i].transform.position = pos;
            }

            return platforms;
        }
    }
}
