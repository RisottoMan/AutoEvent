using PlayerRoles;
using PluginAPI.Core;
using System.Linq;
using AutoEvent.Interfaces;
using UnityEngine;

namespace AutoEvent.Games.Line;
public class LineComponent : MonoBehaviour
{
    private BoxCollider collider;
    private Plugin _plugin;
    private ObstacleType _type;
    public void Init(Plugin plugin, ObstacleType type)
    {
        _plugin = plugin;
        _type = type;
    }
    private void Start()
    {
        collider = gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
    }
    
    void OnTriggerStay(Collider other)
    {
        if (AutoEvent.ActiveEvent is IEventMap map && map.MapInfo.Map is not null)
        {
            if (Player.Get(other.gameObject) != null)
            {
                var pl = Player.Get(other.gameObject);
                pl.SetRole(AutoEvent.Singleton.Config.LobbyRole, RoleChangeReason.None);
                pl.Position = map.MapInfo.Map.AttachedBlocks.First(x => x.name == "SpawnPoint_spec").transform.position;
            }
        }
    }
}
public enum ObstacleType
{
    Ground,
    Wall,
    Dots,
    MiniWalls
}
