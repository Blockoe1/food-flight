/*****************************************************************************
// File Name : PlayerAirResistance.cs
// Author : Brandon Koederitz
// Creation Date : 1/28/2026
// Last Modified : 2/3/2026
//
// Brief Description :  Custom gravity implementation that simulates air resistance by taking the rotation of the 
// player into account when determining 
*****************************************************************************/
using CustomAttributes;
using UnityEngine;

namespace FoodFlight
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerAirResistance : AirResistance
    {
        [SerializeField] private float verticalVelocity;
        [SerializeField] private float horizontalVelocity;

        private float horizontalDrag;
        private float verticalDrag;

        /// <summary>
        /// Calculate the horizontal and vertical drag forces to lerp between.
        /// </summary>
        private void Awake()
        {
            horizontalDrag = AirResistance.CalculateDragFromTVelocity(horizontalVelocity, rb.mass);
            verticalDrag = AirResistance.CalculateDragFromTVelocity(verticalVelocity, rb.mass);
        }

        /// <summary>
        /// Gets the drag force based on the rotation of the object.
        /// </summary>
        /// <returns>The drag force for this FixedUpdate.</returns>
        protected override Vector3 GetDragForce()
        {
            return RotationDrag(rb.rotation, rb.linearVelocity.y);
        }

        /// <summary>
        /// Uses a pseudo-physics function to calculate the force to apply to the player.
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        private Vector3 RotationDrag(Quaternion rotation, float speed)
        {
            // Get the direction that the player is rotated in (Treating down as forward).
            Vector3 pointingVector = (rotation * Vector3.down);
            // Use two normalized vectors to ensure the resulting drag value is 0-1;
            float orientationFactor = Mathf.Abs(Vector3.Dot(pointingVector.normalized, Vector3.down));
            float dragForce = Mathf.Lerp(horizontalDrag, verticalDrag, orientationFactor);
            return AirResistance.CalculateDragForce(speed, dragForce);
        }
    }
}
