using UnityEngine;

public class RotateRedComponent : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public float accelerationRate = 2f;
    public float maxSpeed = 200f;

    private float currentRotationSpeed;

    private void Start()
    {
        currentRotationSpeed = rotationSpeed;
    }

    private void Update()
    {
        transform.Rotate(0f, -currentRotationSpeed * Time.deltaTime, 0f, Space.Self);

        currentRotationSpeed += accelerationRate * Time.deltaTime;
        currentRotationSpeed = Mathf.Clamp(currentRotationSpeed, 0f, maxSpeed);
    }
}