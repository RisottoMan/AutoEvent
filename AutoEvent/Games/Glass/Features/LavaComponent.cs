using Exiled.API.Features;
using UnityEngine;

namespace AutoEvent.Games.Glass.Features;
public class LavaComponent : MonoBehaviour
{
    private BoxCollider collider;
    private Plugin _plugin;
    public void StartComponent(Plugin plugin)
    {
        _plugin = plugin;
    }

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
            pl.Hurt(500f, _plugin.Translation.Died);
        }
    }
}
