/*****************************************************************************
// File Name : ObjectGravitation.cs
// Author : Brandon Koederitz
// Creation Date : 2/5/2026
// Last Modified : 2/5/2026
//
// Brief Description :  Applies a gravitational force to this object, pulling it towards other gravitational objects.
*****************************************************************************/
using CustomAttributes;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FoodFlight
{
    [RequireComponent(typeof(Rigidbody))]
    public class ObjectGravitation : MonoBehaviour
    {
        #region CONST
        private const float GRAVITATIONAL_CONSTANT = 1;
        #endregion

        [SerializeField, Tooltip("How strong the gravitational forces exerted by this object are.")] 
        private float gravityStrength;
        [SerializeField, Tooltip("How much this object is affected by gravitational forces from other objects.")]
        private float gravityBias;

        private static readonly List<ObjectGravitation> gravityObjects = new List<ObjectGravitation>();

        #region Component References
        [Header("Components")]
        [SerializeReference, ReadOnly] protected Rigidbody rb;

        [ContextMenu("Get Component References")]
        private void Reset()
        {
            rb = GetComponent<Rigidbody>();
        }
        #endregion

        /// <summary>
        /// Add/Remove this object from the list of objects affected by gravity.
        /// </summary>
        private void OnEnable()
        {
            gravityObjects.Add(this);
        }
        private void OnDisable()
        {
            gravityObjects.Remove(this);
        }

        /// <summary>
        /// Apply force on this object based on it's distance from other gravity objects.
        /// </summary>
        private void FixedUpdate()
        {
            foreach (var gravityObject in gravityObjects)
            {
                // Skip applying foce based on the object itself.
                if (gravityObject == this) { continue; }
                Vector3 toVector = gravityObject.rb.position - rb.position;
                float gravityStrength = CalculateGravityStrength(gravityObject.gravityStrength, toVector.magnitude);
                rb.AddForce(gravityStrength * toVector.normalized);
            }
        }

        /// <summary>
        /// Calculates the force of gravity between two objects
        /// </summary>
        /// <remarks>
        /// In reality, it calculates acceleration due to gravity but we want the burger to be more affected.
        /// </remarks>
        /// <param name="gravityStrength">The gravity strength of the other object applying force to this object.</param>
        /// <param name="distance">The distance between the two objects.</param>
        /// <returns>The acceleration due to gravity this object experiences.</returns>
        private float CalculateGravityStrength(float gravityStrength, float distance)
        {
            // Prevent /0 error.
            if (distance == 0) {  return 0f; }
            return GRAVITATIONAL_CONSTANT * gravityStrength * gravityBias / Mathf.Pow(distance, 2);
        }
    }
}
