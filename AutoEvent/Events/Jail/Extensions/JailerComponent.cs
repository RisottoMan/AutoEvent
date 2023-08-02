using UnityEngine;

public class JailerComponent : MonoBehaviour
{
    private Transform doorTransform;
    private bool isOpen = false;
    private Vector3 originalPosition;

    private void Start()
    {
        doorTransform = transform;
        originalPosition = doorTransform.position;
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            doorTransform.position += new Vector3(-2.2f, 0, 0);
        }
        else
        {
            doorTransform.position = originalPosition;
        }
    }
}