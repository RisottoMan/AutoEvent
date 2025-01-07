using UnityEngine;

namespace AutoEvent.Games.Deathrun;
public class ColliderComponent : MonoBehaviour
{
    private BoxCollider _collider;
    void Start()
    {
        _collider = gameObject.AddComponent<BoxCollider>();
        _collider.isTrigger = true;
    }
    
    void OnTriggerEnter(Collider collider)
    {
        Animator animator = gameObject.GetComponentInParent<Animator>();
        if (animator != null)
        {
            animator.Play(animator.name + "action");
        }
    }
}