using System;
using AdminToys;
using AutoEvent.API.Components;
using MEC;
using Mirror;
using PlayerStatsSystem;
using PluginAPI.Core;
using UnityEngine;

namespace AutoEvent.Games.Spleef.Features
{
    public class FallPlatformComponent : MonoBehaviour, IDestructible
    {
        private BoxCollider collider;

        public float RegenerationDelay { get; set; } = -1;
        public float FallDelay { get; set; } = -1;
        private float Scale { get; set; } 
        public void Init(float regenerationDelay = 0, float fallDelay = -1, float health = 1, float fallStartDelay = 15f, float scale = 1f)
        {
            Scale = scale;
            RegenerationDelay = regenerationDelay;
            FallDelay = fallDelay;
            Health = health;
            DefaultHealth = health;
            Timing.CallDelayed(fallStartDelay, () =>
            {
                collider.isTrigger = true;
            });
        }
        private void Start()
        {
            collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = false;
            var val = gameObject.GetComponent<PrimitiveObjectToy>().NetworkScale + new Vector3(0.2f, 4f, 0.2f);
            collider.size = val;
        }
        void OnTriggerEnter(Collider other)
        {
            if (Player.Get(other.gameObject) is Player)
            {

                if (FallDelay < 0)
                    return;
                if (isDestroyed)
                    return;
                this.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.red;

                Timing.CallDelayed(FallDelay, () => { DestroyThis(); });
            }
        }

        private void DestroyThis()
        {
            isDestroyed = true;
            this.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.red;
            gameObject.transform.position += Vector3.down * 25;
            Health = -1000;
            NetworkServer.UnSpawn(this.gameObject);
            NetworkServer.Spawn(this.gameObject);
            if (RegenerationDelay > 0)
            {
                Timing.CallDelayed(RegenerationDelay, () =>
                {
                    RestoreThis();
                });
            }
        }

        private void RestoreThis()
        {
            isDestroyed = false;
            this.GetComponent<PrimitiveObjectToy>().NetworkMaterialColor = Color.green;
            gameObject.transform.position += Vector3.up * 25;
            Health = DefaultHealth;
            NetworkServer.UnSpawn(this.gameObject);
            NetworkServer.Spawn(this.gameObject);
        }
        
        public bool Damage(float damage, DamageHandlerBase handler, Vector3 exactHitPos)
        {
            if (isDestroyed)
            {
                return false;
            }
            Health -= damage;
            if (Health <= 0)
            {
                DestroyThis();
            }

            return true;
        }

        private bool isDestroyed = false;
        public uint NetworkId { get; }
        public Vector3 CenterOfMass { get; }
        public float Health { get; private set; }
        private float DefaultHealth { get; set; }
        public event Action<DamagingPrimitiveArgs> DamagingPrimitive;
    }
}
