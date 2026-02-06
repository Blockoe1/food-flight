/*****************************************************************************
// File Name : AirResistance.cs
// Author : Brandon Koederitz
// Creation Date : 2/3/2026
// Last Modified : 2/3/2026
//
// Brief Description :  Applies an air resistance force to an object based on their speed;
*****************************************************************************/
using CustomAttributes;
using UnityEngine;

namespace FoodFlight
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class AirResistance : MonoBehaviour
    {
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
        /// Containually apply a drag force to the player based on their rotation and current speed.
        /// </summary>
        private void FixedUpdate()
        {
            Vector3 dragForce = GetDragForce();
            rb.AddForce(dragForce, ForceMode.Acceleration);
        }

        /// <summary>
        /// Gets the drag force to apply to this object this update.
        /// </summary>
        protected abstract Vector3 GetDragForce();

        /// <summary>
        /// Uses a pseudo-physics fomula to calculate the drag applied at max horizontal and max vertical angles
        /// </summary>
        /// <remarks>Done this way to give design an easier time.</remarks>
        /// <param name="terminalVelocity"></param>
        /// <returns></returns>
        public static float CalculateDragFromTVelocity(float terminalVelocity, float mass)
        {
            // Prevent divide by 0 error.
            if (terminalVelocity == 0f) { return 0f; } 
            return 2 * mass * Physics.gravity.y / Mathf.Pow(terminalVelocity, 2);
        }

        /// <summary>
        /// Uses a pseudo-physics function to calculate the force to apply to the player.
        /// </summary>
        /// <param name="speed">The speed the object is moving at.</param>
        /// <param name="dragForce">The magnitude of the drag force to apply.</param>
        /// <returns>The drag force to apply.</returns>
        protected static Vector3 CalculateDragForce(float speed, float dragForce)
        {
            return Mathf.Pow(speed, 2) * System.MathF.Sign(speed) * dragForce / 2 * Vector2.up;
        }
    }
}
