using System.Collections;
using UnityEngine;

namespace FoodFlight
{
    public class RagdollController : MonoBehaviour
    {
        private bool devControl;

        [SerializeField] private Rigidbody hips;
        [SerializeField] private Rigidbody playerRigidbody;
        private Rigidbody[] rigidBodies;
        private Animator animator;

        private Coroutine ragdollRoutine;

        void Awake()
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
            playerRigidbody.transform.position = hips.position;

            foreach (var rigidBody in rigidBodies)
            {
                rigidBody.isKinematic = true;
            }

            //Vector3 originalHipsPos = hips.position;
            //transform.parent.position = hips.position;
            //hips.position = originalHipsPos;
            //transform.position = hips.position;
            //hips.position = originalHipsPos;


            animator.enabled = true;
            animator.Rebind();
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

        /// <summary>
        /// Causes the player to ragdoll for a certain number of seconds.
        /// </summary>
        /// <param name="seconds"></param>
        public void RagdollForSeconds(float seconds)
        {
            if (ragdollRoutine != null)
            {
                StopCoroutine(ragdollRoutine);
                ragdollRoutine = null;
            }
            ragdollRoutine = StartCoroutine(RagdollRoutine(seconds));
        }
        private IEnumerator RagdollRoutine(float seconds)
        {
            EnableRagdoll();
            yield return new WaitForSeconds(seconds);
            DisableRagdoll();
            ragdollRoutine = null;
        }
    }
}
