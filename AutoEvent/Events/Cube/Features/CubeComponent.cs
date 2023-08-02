using UnityEngine;

public class CubeComponent : MonoBehaviour
{
    public float Speed = 1.0f;

    private void Start()
    {
        
    }

    private void Update()
    {
        transform.TransformVector(transform.position += new Vector3(0f, Speed * Time.deltaTime, 0f));
    }
}