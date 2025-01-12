using Exiled.API.Features;
using UnityEngine;

namespace AutoEvent.Games.Deathrun;
public class WeaponComponent : MonoBehaviour
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

    void OnTriggerEnter(Collider collider)
    {
        if (Player.Get(collider.gameObject) is Player player)
        {
            player.GiveLoadout(_plugin.Config.WeaponLoadouts);
        }
    }
}