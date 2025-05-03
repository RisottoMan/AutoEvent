using Exiled.API.Features;
using UnityEngine;

namespace AutoEvent.Games.Race;
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
            pl.Position = _plugin.Spawnpoint.transform.position;
        }
    }
}
