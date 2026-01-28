using CustomAttributes;
using TMPro;
using UnityEngine;

namespace FoodFlight
{
    [RequireComponent(typeof(InputSynchronizer))]
    public class GyroRotator : PlayerRotator
    {
        [Header("Gyro Settings")]
        [SerializeField] private TMP_Text debugText;
        [SerializeField, Tooltip("Multiplied by the raw Gyro input to scale it down and calculate the rotation " +
            "applied to this player.")] 
        private float gyroSensitivity;
        [SerializeField, Tooltip("Gyro inputs lower than this threshold are ignored.")] private float gyroThreshold;

        #region Component References
        [Header("Components")]
        [SerializeReference, ReadOnly] private InputSynchronizer inSync;

        [ContextMenu("Get Component References")]
        protected override void Reset()
        {
            base.Reset();
            inSync = GetComponent<InputSynchronizer>();
        }
        #endregion

        /// <summary>
        /// Every FixedUpdate, apply any unapplied gyro rotation.
        /// </summary>
        protected override void FixedUpdate()
        {
            if  (inSync.CanRead)
            {
                // Read Gyro input and update the TargetRotation.
                Vector3 gyroVector = inSync.GetAccumulatedGyro();
                Vector3 processedGyro = IgnoreThreshold(gyroVector, gyroThreshold) * gyroSensitivity;
                Quaternion gyroQuat = Quaternion.Euler(processedGyro);
                targetRotation = targetRotation * gyroQuat;

                // Debug.
                SetGyroText(gyroVector, processedGyro);
            }

            // Always run the base FixedUpdate after target rotation has been set.
            base.FixedUpdate();
        }

        /// <summary>
        /// Filters out components of a Vector3 if they do not pass a certain threshold.
        /// </summary>
        /// <param name="vector">The vector to filter.</param>
        /// <param name="ignoreThreshold">The threshold that the vector's components have to surpass.</param>
        /// <returns>The vector with components that don't meet the threshold removed.</returns>
        private static Vector3 IgnoreThreshold(Vector3 vector, float ignoreThreshold)
        {
            vector.x = Mathf.Abs(vector.x) > ignoreThreshold ? vector.x : 0;
            vector.y = Mathf.Abs(vector.y) > ignoreThreshold ? vector.y : 0;
            vector.z = Mathf.Abs(vector.z) > ignoreThreshold ? vector.z : 0;
            return vector;
        }

        #region Debug
        private void SetGyroText(Vector3 gyro, Vector3 processed)
        {
            if (debugText == null) { return; }
            debugText.text = name + " Gyro: " + gyro + "\nProcessed Gyro: " + processed; 
        }
        #endregion
    }
}
