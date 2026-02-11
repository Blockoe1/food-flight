/*****************************************************************************
// File Name : SkydivingMovement.cs
// Author : Brandon Koederitz
// Creation Date : 1/28/2026
// Last Modified : 1/31/2026
//
// Brief Description :  Controls player drifting based on their rotation.
*****************************************************************************/
using CustomAttributes;
using System.Collections;
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

        [field: SerializeField] public float MaxDriftSpeed { get; set; }
        [field: SerializeField] public float DriftAcceleration { get; set; }
        [SerializeField] private float disabledAcceleration;
        [SerializeField] private Vector2 levelBounds;

        private bool canMove = true;

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
        /// Debug
        /// </summary>
        private void Update()
        {
            // Calculate a quaternion to rotate all the ideal vectors by so they align with the  players orientation.
            Vector3 forward = (rb.rotation * Vector3.down);
            Vector2 rotVector = new Vector2(forward.x, forward.z);
            float rotAngle = Mathf.Atan2(rotVector.x, rotVector.y) * Mathf.Rad2Deg;
            Quaternion idealRotQuat = Quaternion.Euler(0, rotAngle, 0);

            // Rotate the X ideal vectors by your pitch axis as well.
            Vector2 pitchVector = new Vector2(forward.y, forward.z);
            float pitchAngle = Mathf.Atan2(pitchVector.x, pitchVector.y) * Mathf.Rad2Deg;
            Quaternion idealPitchQuat = Quaternion.Euler(-pitchAngle, 0, 0);

            Debug.DrawLine(rb.position, rb.position + idealRotQuat * idealPitchQuat * IDEAL_X_DRIFT_VECTOR * 5, Color.red);
            Debug.DrawLine(rb.position, rb.position + idealRotQuat * idealPitchQuat * IDEAL_NEG_X_DRIFT_VECTOR * 5, Color.red);
            Debug.DrawLine(rb.position, rb.position + idealRotQuat * IDEAL_Z_DRIFT_VECTOR * 5, Color.red);
            Debug.DrawLine(rb.position, rb.position + idealRotQuat * IDEAL_NEG_Z_DRIFT_VECTOR * 5, Color.red);
            Debug.DrawLine(rb.position, rb.position + rb.rotation * Vector3.right * 5, Color.green);
            Debug.DrawLine(rb.position, rb.position + rb.rotation * Vector3.forward * 5, Color.green);
            Debug.DrawLine(rb.position, rb.position + rb.linearVelocity * 5, Color.blue);
        }

        /// <summary>
        /// Applies drift velocity to the player based on their current rotation.
        /// </summary>
        private void FixedUpdate()
        {
            Vector2 targetDriftVelocity = Vector2.zero;
            float acceleration = disabledAcceleration;

            // Prevent any force adding if moving is disabled.
            if (canMove)
            {
                acceleration = DriftAcceleration;

                // Calculate a quaternion to rotate all the ideal vectors by so they align with the players orientation.
                Vector3 forward = (rb.rotation * Vector3.down);
                Vector2 rotVector = new Vector2(forward.x, forward.z);
                float rotAngle = Mathf.Atan2(rotVector.x, rotVector.y) * Mathf.Rad2Deg;
                Quaternion idealRotQuat = Quaternion.Euler(0, rotAngle, 0);

                // Rotate the X ideal vectors by your pitch axis as well.
                Vector2 pitchVector = new Vector2(forward.y, forward.z);
                float pitchAngle = Mathf.Atan2(pitchVector.x, pitchVector.y) * Mathf.Rad2Deg;
                Quaternion idealPitchQuat = Quaternion.Euler(-pitchAngle, 0, 0);

                // Calculate X Drift
                targetDriftVelocity.x = CalculateDrift(rb.rotation * Vector3.right,
                    idealRotQuat * IDEAL_X_DRIFT_VECTOR, idealRotQuat * IDEAL_NEG_X_DRIFT_VECTOR);
                //Debug.Log(GetOrientationFitness(IDEAL_X_DRIFT_VECTOR, xVector));

                // Caluclate Z Drift
                targetDriftVelocity.y = CalculateDrift(rb.rotation * Vector3.forward,
                    idealRotQuat * IDEAL_Z_DRIFT_VECTOR, idealRotQuat * IDEAL_NEG_Z_DRIFT_VECTOR);

                // Rotate the target drift velocity so that it matches the player's orientation.
                Vector3 rotatedVel = idealRotQuat * new Vector3(targetDriftVelocity.x, 0, targetDriftVelocity.y);
                targetDriftVelocity = new Vector2(rotatedVel.x, rotatedVel.z);
            }

            // Move our current velocity towards the target.
            Vector2 currentVel = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z);
            currentVel = Vector2.MoveTowards(currentVel, targetDriftVelocity, acceleration);

            //Debug.Log($"Target Velocity: {targetDriftVelocity}.  Current Velocity: {currentVel}");
            //Debug.DrawLine(rb.position, rb.position + new Vector3(currentVel.x, 0, currentVel.y) * 5, Color.orange, 1f);
            //Debug.DrawLine(rb.position, rb.position + new Vector3(targetDriftVelocity.x, 0, targetDriftVelocity.y) * 5, Color.purple, 1f);

            rb.linearVelocity = new Vector3(currentVel.x, rb.linearVelocity.y, currentVel.y);

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
                GetOrientationFitness(idealNegVector, currentVector)) * MaxDriftSpeed;
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

        /// <summary>
        /// The amount of time the player's movement is disabled for.
        /// </summary>
        /// <param name="seconds">The time in seconds to disable.</param>
        public void DisableForSeconds(float seconds)
        {
            if (canMove)
            {
                StartCoroutine(DisableRoutine(seconds));
            }
        }
        private IEnumerator DisableRoutine(float seconds)
        {
            canMove = false;
            yield return new WaitForSeconds(seconds);
            canMove = true;
        }
    }
}
