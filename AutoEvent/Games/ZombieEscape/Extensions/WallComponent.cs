using UnityEngine;
public class WallComponent : MonoBehaviour
{
    public float Duration { get; set; } = 15;
    private void Start()
    {
        gameObject.transform.position += Vector3.down * 3;
        Destroy(gameObject, Duration);
    } 
}