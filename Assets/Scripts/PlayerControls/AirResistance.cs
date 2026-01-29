/*****************************************************************************
// File Name : SkydiveGravity.cs
// Author : Brandon Koederitz
// Creation Date : 1/28/2026
// Last Modified : 1/28/2026
//
// Brief Description :  Custom gravity implementation that simulates air resistance by taking the rotation of the 
// player into account when determining 
*****************************************************************************/
using CustomAttributes;
using UnityEngine;

namespace FoodFlight
{
    [RequireComponent(typeof(Rigidbody))]
    public class AirResistance : MonoBehaviour
    {
        [SerializeField] private float verticalVelocity;
        [SerializeField] private float horizontalVelocity;

        private float horizontalDrag;
        private float verticalDrag;

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
        /// Calculate the horizontal and vertical drag forces to lerp between.
        /// </summary>
        private void Awake()
        {
            horizontalDrag = CalculateDragFromTVelocity(horizontalVelocity);
            verticalDrag = CalculateDragFromTVelocity(verticalVelocity);
        }

        /// <summary>
        /// Uses a pseudo-physics fomula to calculate the drag applied at max horizontal and max vertical angles
        /// </summary>
        /// <remarks>Done this way to give design an easier time.</remarks>
        /// <param name="terminalVelocity"></param>
        /// <returns></returns>
        private float CalculateDragFromTVelocity(float terminalVelocity)
        {
            return 2 * rb.mass * Physics.gravity.y / Mathf.Pow(terminalVelocity, 2);
        }

        /// <summary>
        /// Containually apply a drag force to the player based on their rotation and current speed.
        /// </summary>
        private void FixedUpdate()
        {
            Vector3 dragForce = CalculateDragForce(rb.rotation, rb.linearVelocity.y);
            rb.AddForce(dragForce, ForceMode.Acceleration);
            Debug.Log(rb.linearVelocity);
        }

        /// <summary>
        /// Uses a pseudo-physics function to calculate the force to apply to the player.
        /// </summary>
        /// <param name="pitchAngle"></param>
        /// <param name="speed"></param>
        /// <param name="dragStrength"></param>
        /// <returns></returns>
        private Vector3 CalculateDragForce(Quaternion rotation, float speed)
        {
            // Get the direction that the player is rotated in (Treating down as forward).
            Vector3 pointingVector = (rotation * Vector3.down);
            // Use two normalized vectors to ensure the resulting drag value is 0-1;
            float orientationFactor = Mathf.Abs(Vector3.Dot(pointingVector.normalized, Vector3.down));
            float dragForce = Mathf.Lerp(horizontalDrag, verticalDrag, orientationFactor);
            return Mathf.Pow(speed, 2) * dragForce / 2 * Vector2.down;
        }
    }
}
