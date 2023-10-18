using MEC;
using UnityEngine;
public class WallComponent : MonoBehaviour
{
    public void Init(float gateDelay)
    {
        Duration = gateDelay;
    }
    public float Duration { get; set; } = 15;
    private void Start()
    {
        gameObject.transform.position += Vector3.down * 3;
        Timing.CallDelayed(Duration, () =>
        {
            gameObject.transform.position += Vector3.up * 3;
        });
        // Destroy(gameObject, Duration);
    } 
}