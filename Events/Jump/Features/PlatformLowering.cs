using UnityEngine;

namespace AutoEvent.Events.Jump.Features
{
    public class PlatformLowering : MonoBehaviour
    {
        public float loweringSpeed = 0.3f;
        public float lowerDistance = 2f;

        private Vector3 originalPosition;
        private float targetPositionY;

        private void Start()
        {
            originalPosition = transform.position;
            targetPositionY = originalPosition.y - lowerDistance;
        }

        private void Update()
        {
            transform.position = new Vector3(transform.position.x, Mathf.MoveTowards(transform.position.y, targetPositionY, loweringSpeed * Time.deltaTime), transform.position.z);
        }
    }
}
