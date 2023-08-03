using UnityEngine;

public class DoorComponent : MonoBehaviour
{
    private Transform doorTransform;
    private bool isOpen;
    private float openTime = 2f;

    private void Start()
    {
        doorTransform = transform;
        isOpen = false;
    }

    private void Update()
    {
        if (isOpen)
        {
            if (openTime <= 0)
            {
                doorTransform.position += new Vector3(0f, -4f, 0f);
                isOpen = false;
            }
            else
            {
                openTime -= Time.deltaTime;
            }
        }
    }

    public void Open()
    {
        doorTransform.position += new Vector3(0f, 4f, 0f);
        isOpen = true;
        openTime = 2f;
    }
}