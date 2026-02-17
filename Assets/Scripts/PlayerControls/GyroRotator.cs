using CustomAttributes;
using TMPro;
using UnityEngine;

namespace FoodFlight
{
    public class GyroRotator : PlayerRotator
    {
        [Header("Gyro Settings")]
        [SerializeField, Tooltip("Multiplied by the raw Gyro input to scale it down and calculate the rotation " +
            "applied to this player.")] 
        private float gyroSensitivity;
        [SerializeField, Tooltip("Gyro inputs lower than this threshold are ignored.")] private float gyroThreshold;

        private Quaternion internalControllerRotation = Quaternion.identity;
        private Quaternion internalControllerYaw = Quaternion.identity;

        private Quaternion internalPitch = Quaternion.identity;
        private Quaternion internalRoll = Quaternion.identity;

        /// <summary>
        /// Every FixedUpdate, apply any unapplied gyro rotation.
        /// </summary>
        protected override void FixedUpdate()
        {
            if  (inSync.CanRead)
            {
                //// Read Gyro input and update the TargetRotation.
                //Vector3 gyroVector = inSync.GetAccumulatedGyro();
                //// Swap the Y and Z gyro to account for straigt down.
                //(gyroVector.y, gyroVector.z) = (-gyroVector.z, gyroVector.y);
                //Vector3 processedGyro = IgnoreThreshold(gyroVector, gyroThreshold) * gyroSensitivity;
                ////processedGyro.z = 0;
                //Quaternion gyroQuat = Quaternion.Euler(processedGyro);
                //Quaternion yawQuat = Quaternion.Euler(new Vector3(0, 0, processedGyro.z));
                //internalControllerRotation = internalControllerRotation * gyroQuat;
                //internalControllerYaw = internalControllerYaw * yawQuat;
                ////targetRotation = targetRotation * gyroQuat;

                //// Calculate a player rotation based on the orientation of the controller.

                //Vector3 forwardVector = (internalControllerRotation * Vector3.down);
                //Vector3 yawForward = internalControllerYaw * Vector3.down;
                //Debug.DrawLine(rb.position, rb.position + forwardVector * 5, Color.red);
                //Debug.DrawLine(rb.position, rb.position + yawForward * 5, Color.red);

                //Vector3 rightVector = (internalControllerRotation * Vector3.right);
                //Vector3 yawRight = internalControllerYaw * Vector3.right;
                //Debug.DrawLine(rb.position, rb.position + rightVector * 5, Color.green);
                //Debug.DrawLine(rb.position, rb.position + yawRight * 5, Color.green);

                //float pitchAngle = (Vector3.Angle(yawForward, forwardVector));
                //if (forwardVector.z < 0)
                //{
                //    pitchAngle = (360 - pitchAngle) % 360;
                //}
                //pitchAngle = -pitchAngle;

                //Quaternion pitchQuat = Quaternion.Euler(pitchAngle, 0, 0);

                //Debug.Log(-(Vector3.Angle(yawForward, forwardVector) + (forwardVector.z > 0 ? 0 : 180)));

                //float rollAngle = Vector3.Angle(yawRight, rightVector);
                //if (rightVector.z < 0)
                //{
                //    rollAngle = (360 - rollAngle) % 360;
                //}
                //rollAngle = -rollAngle;
                //Quaternion rollQuat = Quaternion.Euler(0, rollAngle, 0);

                //targetRotation = pitchQuat * rollQuat;

                // Read Gyro input and update the TargetRotation.
                Vector3 gyroVector = inSync.GetAccumulatedGyro();
                // Swap the Y and Z gyro to account for straigt down.
                (gyroVector.y, gyroVector.z) = (-gyroVector.z, gyroVector.y);
                Vector3 processedGyro = IgnoreThreshold(gyroVector, gyroThreshold) * gyroSensitivity;

                //Debug.Log(processedGyro);

                Quaternion pitQuat = Quaternion.Euler(new Vector3(processedGyro.x, 0, 0));
                Quaternion rollQuat = Quaternion.Euler(new Vector3(0, processedGyro.y, 0));

                internalPitch = internalPitch * pitQuat;
                internalRoll = internalRoll * rollQuat;
                Quaternion usedPitch = internalPitch;
                if (isFlat)
                {
                    usedPitch = Quaternion.Euler(-90, 0, 0);
                }
                targetRotation = usedPitch * internalRoll;
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

        /// <summary>
        /// They gyro rotator should be enabled when the control scheme has gyro.
        /// </summary>
        /// <param name="hasGyro">True if gyro is enabled.</param>
        protected override void CheckEnabled(bool hasGyro)
        {
            enabled = hasGyro;
        }

        /// <summary>
        /// Reset the stored internal rotation of the controller.
        /// </summary>
        public override void ResetRotation()
        {
            base.ResetRotation();
            internalControllerRotation = Quaternion.identity;
            internalControllerYaw = Quaternion.identity;
            internalPitch = Quaternion.identity;
            internalRoll = Quaternion.identity;
        }
    }
}
