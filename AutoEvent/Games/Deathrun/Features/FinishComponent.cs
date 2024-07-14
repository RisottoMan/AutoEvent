using PluginAPI.Core;
using UnityEngine;

namespace AutoEvent.Games.Deathrun.Features;
public class FinishComponent : MonoBehaviour
{
    private BoxCollider _collider;
    private Plugin _plugin;

    public void StartComponent(Plugin plugin)
    {
        _plugin = plugin;
    }

    private void Start()
    {
        _collider = gameObject.AddComponent<BoxCollider>();
        _collider.isTrigger = true;
    }

    void OnTriggerStay(Collider other)
    {
        if (Player.Get(other.gameObject) is Player)
        {
            var pl = Player.Get(other.gameObject);
            pl.Heal(100);
            pl.AddItem(ItemType.GunE11SR);
        }
    }
}
