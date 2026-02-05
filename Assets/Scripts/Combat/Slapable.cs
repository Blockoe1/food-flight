/*****************************************************************************
// File Name : Attackable.cs
// Author : Brandon Koederitz
// Creation Date : 2/2/2026
// Last Modified : 2/2/2026
//
// Brief Description :  Allows an object to be slapped.
*****************************************************************************/
using CustomAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace FoodFlight
{
    public class Slapable : MonoBehaviour
    {
        #region CONST
        private const string SLAP_TAG = "Slap";
        #endregion

        [SerializeField] private float baseKnockbackStrength;
        [SerializeField] private float scaledKnockbackStrength;
        [SerializeField] private Vector3 staticSimulatedSpeed;
        [SerializeField] private UnityEvent<Vector3> OnSlapped;

        #region Component References
        [Header("Components")]
        [SerializeReference, ReadOnly] private Rigidbody rb;

        [ContextMenu("Get Component References")]
        private void Reset()
        {
            rb = GetComponent<Rigidbody>();
        }

        #endregion

        /// <summary>
        /// Knocks this object back when they hit a slap hitbox.
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.CompareTag(SLAP_TAG))
            {
                GetSlapped(collision.attachedRigidbody.position, collision.attachedRigidbody.linearVelocity);
            }
        }

        /// <summary>
        /// Hittin an obstacle also counts as being slapped.
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(SLAP_TAG))
            {
                GetSlapped(collision.gameObject.transform.position, staticSimulatedSpeed);
            }
        }

        /// <summary>
        /// Causes this object to be knocked away from the attacking rigidbody.
        /// </summary>
        /// <param name="attackingRigidbody">The rigidbody of the player that slapped this object.</param>
        private void GetSlapped(Vector3 position, Vector3 velocity)
        {
            Vector3 knockbackDirection = (rb.position - position).normalized;
            float knockbackStrength = (scaledKnockbackStrength * velocity.magnitude) 
                + baseKnockbackStrength;

            OnSlapped?.Invoke(knockbackDirection * knockbackStrength);
            rb.AddForce(knockbackDirection * knockbackStrength, ForceMode.Impulse);
        }
    }
}
