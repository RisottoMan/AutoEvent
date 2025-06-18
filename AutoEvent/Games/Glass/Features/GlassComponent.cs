using MEC;
using Exiled.API.Features;
using UnityEngine;

namespace AutoEvent.Games.Glass.Features;
public class GlassComponent : MonoBehaviour
{
    private BoxCollider collider;
    public float RegenerationDelay { get; set; } = 5;
    
    public void Init(float regenerationDelay)
    {
        RegenerationDelay = regenerationDelay;
    }
    
    private void Start()
    {
        collider = gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = new Vector3(1, 10, 1);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (Player.Get(other.gameObject) != null)
        {
            gameObject.transform.position += Vector3.down * 5;
            
            if (this.RegenerationDelay > 0)
            {
                Timing.CallDelayed(this.RegenerationDelay, () =>
                {
                    gameObject.transform.position -= Vector3.down * 5;
                });
            }
        }
    }
}
