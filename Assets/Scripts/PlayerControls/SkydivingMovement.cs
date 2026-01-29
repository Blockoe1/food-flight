/*****************************************************************************
// File Name : SkydivingMovement.cs
// Author : Brandon Koederitz
// Creation Date : 1/28/2026
// Last Modified : 1/28/2026
//
// Brief Description :  Controls player drifting based on their rotation.
*****************************************************************************/
using CustomAttributes;
using System.Net.NetworkInformation;
using UnityEngine;

namespace FoodFlight
{
    public class SkydivingMovement : MonoBehaviour
    {
        #region Drift Vectors
        private static readonly Vector3 IDEAL_NEG_X_DRIFT_VECTOR = new Vector3(1, -1, 0).normalized;
        private static readonly Vector3 IDEAL_X_DRIFT_VECTOR = new Vector3(-1, -1, 0).normalized;
        private static readonly Vector3 IDEAL_NEG_Z_DRIFT_VECTOR = new Vector3(0, -1, -1).normalized;
        private static readonly Vector3 IDEAL_Z_DRIFT_VECTOR = new Vector3(0, -1, 1).normalized;
        private static readonly float DEGREE_CORRECTION = Mathf.Sqrt(2) / 2;
        #endregion

        [SerializeField] private float maxDriftSpeed;
        [SerializeField] private float driftAcceleration;

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
        /// Applies drift velocity to the player based on their current rotation.
        /// </summary>
        private void FixedUpdate()
        {
            Vector2 targetDriftVelocity = Vector2.zero;

            // Calculate X Drift
            Vector3 xVector = rb.rotation * Vector3.right;
            targetDriftVelocity.x = (GetOrientationFitness(IDEAL_X_DRIFT_VECTOR, xVector) - GetOrientationFitness(IDEAL_NEG_X_DRIFT_VECTOR, xVector)) * maxDriftSpeed;
            //Debug.Log(GetOrientationFitness(IDEAL_X_DRIFT_VECTOR, xVector));
            // Caluclate Z Drift
            Vector3 zVector = rb.rotation * Vector3.forward;
            targetDriftVelocity.y = (GetOrientationFitness(IDEAL_Z_DRIFT_VECTOR, zVector) - GetOrientationFitness(IDEAL_NEG_Z_DRIFT_VECTOR, zVector)) * maxDriftSpeed;

            // Move our current velocity towards the target.
            Vector2 currentVel = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z);
            currentVel = Vector2.MoveTowards(currentVel, targetDriftVelocity, driftAcceleration);

            //Debug.Log($"Target Velocity: {targetDriftVelocity}.");

            rb.linearVelocity = new Vector3(currentVel.x, rb.linearVelocity.y, currentVel.y);
        }

        /// <summary>
        /// Gets how perpendicular the current vector is with the ideal vector
        /// </summary>
        /// <remarks>
        /// Check for perpendicularity (and use inverted vectors) to give more leniency, as the check is more circle shaped than precise.
        /// </remarks>
        /// <param name="idealVector"></param>
        /// <param name="currentVector"></param>
        /// <returns>Range between 1 (perpendicular) and 0 (parallel)</returns>
        private static float GetOrientationFitness(Vector3 idealVector, Vector3 currentVector)
        {
            float orientationFactor = Mathf.Abs(Vector3.Dot(currentVector.normalized, idealVector.normalized));
            return 1 - orientationFactor;
        }
    }
}
