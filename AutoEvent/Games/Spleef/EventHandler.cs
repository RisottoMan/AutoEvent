using Exiled.Events.EventArgs.Player;
using UnityEngine;

namespace AutoEvent.Games.Spleef;

public class EventHandler
{
    private Plugin _plugin;
    public EventHandler(Plugin plugin)
    {
        _plugin = plugin;
    }
    
    public void OnShot(ShotEventArgs ev)
    {
        if (!Physics.Raycast(ev.Player.CameraTransform.position, ev.Player.CameraTransform.forward, out RaycastHit raycastHit, 10f, 1 << 0))
        {
            return;
        }
        
        if (_plugin.Config.PlatformHealth < 0)
        {
            return;
        }

        if (!ev.Player.CurrentItem.IsFirearm)
        {
            return;
        }

        raycastHit.collider.transform.GetComponentsInParent<FallPlatformComponent>().ForEach(GameObject.Destroy);
    }
}