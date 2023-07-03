using Exiled.API.Features;
using UnityEngine;

namespace AutoEvent.Events.Lava.Features
{
    public class LavaComponent : MonoBehaviour
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
                pl.Hurt(0.1f, "<color=red>Сгорел в Лаве!</color>");
                /* пока что нужно подумать
                if (pl.Health < 1)
                {
                    GrenadeFrag grenade = new GrenadeFrag(ItemType.GrenadeHE);
                    grenade.FuseTime = 0.5f;
                    grenade.Base.transform.localScale = new Vector3(0, 0, 0);
                    grenade.MaxRadius = 0.5f;
                    grenade.Spawn(pl.Position);
                    pl.Kill("<color=red>Сгорел в Лаве!</color>");
                    pl.GameObject.GetComponent<BoxCollider>().enabled = false;
                }
                pl.Hp -= 0.5f;
                */
            }
        }
    }
}
