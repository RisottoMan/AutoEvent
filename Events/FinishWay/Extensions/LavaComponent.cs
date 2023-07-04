using Exiled.API.Features;
using UnityEngine;

namespace AutoEvent.Events.FinishWay.Features
{
    public class LavaComponent : MonoBehaviour
    {
        private BoxCollider collider;
        private void Start()
        {
            collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
        }
        void OnTriggerStay(Collider other)
        {
            if (Player.Get(other.gameObject) is Player)
            {
                var pl = Player.Get(other.gameObject);
                pl.Hurt(500f, $"Вы сдохли от лавы.");
            }
        }
    }
}
