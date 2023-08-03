using PlayerRoles;
using PluginAPI.Core;
using System.Linq;
using UnityEngine;

namespace AutoEvent.Events.Line.Features
{
    public class LineComponent : MonoBehaviour
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
                pl.SetRole(RoleTypeId.Tutorial, PlayerRoles.RoleChangeReason.None);
                pl.Position = Plugin.GameMap.AttachedBlocks.First(x => x.name == "SpawnPoint_spec").transform.position;
            }
        }
    }
}
