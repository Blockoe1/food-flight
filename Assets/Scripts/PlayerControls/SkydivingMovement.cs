/*****************************************************************************
// File Name : SkydivingMovement.cs
// Author : Brandon Koederitz
// Creation Date : 1/28/2026
// Last Modified : 1/31/2026
//
// Brief Description :  Controls player drifting based on their rotation.
*****************************************************************************/
using CustomAttributes;
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
        [SerializeField] private Vector2 levelBounds;

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

            // Calculate a quaternion to rotate all the ideal vectors by so they align with the  players orientation.
            Vector3 forward = (rb.rotation * Vector3.down);
            Vector2 rotVector = new Vector2(forward.x, forward.z);
            float rotAngle = Mathf.Atan2(rotVector.x, rotVector.y) * Mathf.Rad2Deg;
            Quaternion idealRotQuat = Quaternion.Euler(0, rotAngle, 0);

            // Calculate X Drift
            targetDriftVelocity.x = CalculateDrift(rb.rotation * Vector3.right,
                idealRotQuat * IDEAL_X_DRIFT_VECTOR, idealRotQuat * IDEAL_NEG_X_DRIFT_VECTOR);
            //Debug.Log(GetOrientationFitness(IDEAL_X_DRIFT_VECTOR, xVector));
            // Caluclate Z Drift
            targetDriftVelocity.y = CalculateDrift(rb.rotation * Vector3.forward,
                idealRotQuat * IDEAL_Z_DRIFT_VECTOR, idealRotQuat * IDEAL_NEG_Z_DRIFT_VECTOR);

            // Move our current velocity towards the target.
            Vector2 currentVel = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z);
            currentVel = Vector2.MoveTowards(currentVel, targetDriftVelocity, driftAcceleration);

            //Debug.Log($"Target Velocity: {targetDriftVelocity}.");

            Vector3 rotatedVel = idealRotQuat * new Vector3(currentVel.x, 0, currentVel.y);
            rb.linearVelocity = new Vector3(rotatedVel.x, rb.linearVelocity.y, rotatedVel.z);

            // Clamp the player's position to the level's bounds.
            Vector3 pos = rb.position;
            pos.x = Mathf.Clamp(pos.x, -levelBounds.x, levelBounds.x);
            pos.z = Mathf.Clamp(pos.z, -levelBounds.y, levelBounds.y);
            rb.MovePosition(pos);
        }

        /// <summary>
        /// Calculates the drift force based on a certain relative vector and the ideal vectors for + and - to compare it to.
        /// </summary>
        /// <param name="currentVector"></param>
        /// <param name="idealPosVector"></param>
        /// <param name="idealNegVector"></param>
        /// <returns></returns>
        private float CalculateDrift(Vector3 currentVector, Vector3 idealPosVector, Vector3 idealNegVector)
        {
            return (GetOrientationFitness(idealPosVector, currentVector) - 
                GetOrientationFitness(idealNegVector, currentVector)) * maxDriftSpeed;
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
