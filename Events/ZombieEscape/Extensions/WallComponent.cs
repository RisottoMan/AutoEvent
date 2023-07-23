using Exiled.API.Features;
using UnityEngine;

namespace AutoEvent.Events.ZombieEscape.Features
{
    public class WallComponent : MonoBehaviour
    {
        private void Start()
        {
            gameObject.transform.position += Vector3.down * 3;
            Destroy(gameObject, 15);
        }
    }
}
