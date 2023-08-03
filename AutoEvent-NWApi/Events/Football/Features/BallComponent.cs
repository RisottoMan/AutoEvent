using Mirror;
using UnityEngine;

namespace AutoEvent.Events.Football.Features
{
    public class BallComponent : MonoBehaviour
    {
        private SphereCollider sphere;
        private Rigidbody rigid;
        private void Start()
        {
            sphere = gameObject.AddComponent<SphereCollider>();
            sphere.isTrigger = true;
            sphere.radius = 1.1f;

            rigid = gameObject.AddComponent<Rigidbody>();
            rigid.isKinematic = false;
            rigid.useGravity = true;
            rigid.mass = 0.1f;
            rigid.drag = 0.1f;
        }

        private void FixedUpdate()
        {
            transform.position += rigid.velocity * Time.fixedDeltaTime;
        }
    }
}
