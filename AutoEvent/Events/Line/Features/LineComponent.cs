using Exiled.API.Enums;
using Exiled.API.Features;
using PluginAPI.Roles;
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
                pl.Role.Set(PlayerRoles.RoleTypeId.Tutorial, SpawnReason.None, PlayerRoles.RoleSpawnFlags.None);
                pl.Position = Plugin.GameMap.AttachedBlocks.First(x => x.name == "SpawnPoint_spec").transform.position;
            }
        }
    }
}
