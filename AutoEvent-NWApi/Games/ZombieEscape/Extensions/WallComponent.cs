using UnityEngine;
public class WallComponent : MonoBehaviour
{
    private void Start()
    {
        gameObject.transform.position += Vector3.down * 3;
        Destroy(gameObject, 15);
    }
}