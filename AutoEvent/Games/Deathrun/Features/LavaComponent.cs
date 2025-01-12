using Exiled.API.Enums;
using Exiled.API.Features;
using UnityEngine;

namespace AutoEvent.Games.Deathrun;
public class KillComponent : MonoBehaviour
{
    private BoxCollider _collider;
    private void Start()
    {
        _collider = gameObject.AddComponent<BoxCollider>();
        _collider.isTrigger = true;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (Player.Get(collider.gameObject) is Player player)
        {
            if (player.IsAlive)
            {
                player.Kill(DamageType.Explosion);
            }
        }
    }
}