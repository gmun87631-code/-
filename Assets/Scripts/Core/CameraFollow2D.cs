using UnityEngine;

namespace SideScrollerPrototype.Core
{
    public class CameraFollow2D : MonoBehaviour
    {
        [SerializeField] private Vector3 offset = new Vector3(0f, 1.25f, -10f);
        [SerializeField] private float smoothTime = 0.15f;
        [SerializeField] private float leadDistance = 2.25f;
        [SerializeField] private float leadSmoothing = 4f;

        private Transform target;
        private Vector3 velocity;
        private float currentLead;
        private Player.PlayerController playerController;

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            playerController = newTarget == null ? null : newTarget.GetComponent<Player.PlayerController>();
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            float desiredLead = 0f;
            if (playerController != null)
            {
                desiredLead = playerController.HorizontalInput * leadDistance;
            }

            currentLead = Mathf.Lerp(currentLead, desiredLead, Time.deltaTime * leadSmoothing);
            Vector3 desiredPosition = new Vector3(target.position.x + offset.x + currentLead, transform.position.y, offset.z);
            desiredPosition.y = offset.y;
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
        }
    }
}
