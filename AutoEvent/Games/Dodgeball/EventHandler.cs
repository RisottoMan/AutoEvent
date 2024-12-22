using AutoEvent.Events.EventArgs;
using PluginAPI.Core;
using UnityEngine;

namespace AutoEvent.Games.Dodgeball;
public class EventHandler
{
    private Plugin _plugin;
    private Translation _translation;
    public EventHandler(Plugin plugin)
    {
        _plugin = plugin;
        _translation = plugin.Translation;
    }

    // If the ball hits the player, the player will receive damage, and the ball will be destroy
    public void OnScp018Update(Scp018UpdateArgs ev)
    {
        Collider[] _colliders = Physics.OverlapSphere(ev.Projectile.transform.position, ev.Projectile._radius);

        foreach (var collider in _colliders)
        {
            Player player = Player.Get(collider.gameObject);
            if (player != null && ev.Player != player)
            {
                player.Damage(50, _translation.Knocked.Replace("{killer}", ev.Player.Nickname));
                ev.Projectile.DestroySelf();
                break;
            }
        }
    }
    
    // If the ball collided with a wall, we destroy it
    public void OnScp018Collision(Scp018CollisionArgs ev)
    {
        ev.Projectile.DestroySelf();
    }
}