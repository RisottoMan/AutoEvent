using Exiled.API.Features;
using UnityEngine;

namespace AutoEvent.Events.Glass.Features
{
    public class Component : MonoBehaviour
    {
        private BoxCollider collider;
        private void Start()
        {
            collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
        }
        void OnTriggerEnter(Collider other)
        {
            var pl = Player.Get(other.gameObject);
            Destroy(gameObject);
        }
    }
}
