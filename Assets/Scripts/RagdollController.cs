using UnityEngine;

namespace FoodFlight
{
    public class RagdollController : MonoBehaviour
    {
        private bool devControl;

        [SerializeField] private Transform hips;
        private Rigidbody[] rigidBodies;
        private Animator animator;

        void Start()
        {
            rigidBodies = GetComponentsInChildren<Rigidbody>();
            animator = GetComponent<Animator>();

            devControl = true;
            DisableRagdoll();
        }

        [ContextMenu("Toggle")]

        void Toggle()
        {
            if (devControl == true)
            {
                devControl = false;
                EnableRagdoll();
            }
            else
            {
                devControl = true;
                DisableRagdoll();
            }
        }

        public void DisableRagdoll()
        {
            foreach (var rigidBody in rigidBodies)
            {
                rigidBody.isKinematic = true;
            }

            Vector3 originalHipsPos = hips.position;
            transform.position = hips.position;
            hips.position = originalHipsPos;

            animator.enabled = true;
            animator.Play("Skydive Loop");
            animator.Update(0);
        }

        public void EnableRagdoll()
        {
            foreach (var rigidBody in rigidBodies)
            {
                rigidBody.isKinematic = false;
                rigidBody.linearVelocity = Vector3.zero;
            }

            animator.enabled = false;
        }
    }
}
