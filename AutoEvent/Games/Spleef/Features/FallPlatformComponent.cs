using UnityEngine;

namespace AutoEvent.Games.Spleef;
public class FallPlatformComponent : MonoBehaviour
{
    private BoxCollider collider;

    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}
