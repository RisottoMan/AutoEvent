using Exiled.API.Features;
using UnityEngine;

namespace AutoEvent.Events.Glass.Features
{
    public class GlassComponent : MonoBehaviour
    {
        private BoxCollider collider;
        private void Start()
        {
            collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = new Vector3(1, 10, 1);
        }
        void OnTriggerEnter(Collider other)
        {
            if (Player.Get(other.gameObject) is Player)
            {
                Destroy(gameObject);
            }
        }
    }
}
