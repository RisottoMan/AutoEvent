using UnityEngine;

namespace AutoEvent.Events.Football.Features
{
    public class Component : MonoBehaviour
    {
        private SphereCollider sphere;
        private Rigidbody rigid;
        private void Start()
        {
            sphere = gameObject.AddComponent<SphereCollider>();
            sphere.isTrigger = true;
            sphere.radius = 1;

            rigid = gameObject.AddComponent<Rigidbody>();
            rigid.isKinematic = false;
            rigid.useGravity = true;
            rigid.mass = 0.5f;
            rigid.drag = 1f;
        }
    }
}
