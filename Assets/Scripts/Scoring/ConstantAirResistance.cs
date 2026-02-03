/*****************************************************************************
// File Name : ConstantAirResistance.cs
// Author : Brandon Koederitz
// Creation Date : 2/3/2026
// Last Modified : 2/3/2026
//
// Brief Description :  Applies a costnant air resistance force to the object.
*****************************************************************************/
using UnityEngine;

namespace FoodFlight
{
    public class ConstantAirResistance : AirResistance
    {
        [SerializeField] private float terminalVelocity;

        private float drag;

        /// <summary>
        /// Caluclate the drag force needed to meet the object's max velocity.
        /// </summary>
        private void Awake()
        {
            drag = AirResistance.CalculateDragFromTVelocity(terminalVelocity, rb.mass);
        }

        /// <summary>
        /// Apply a constant drag force.
        /// </summary>
        /// <returns></returns>
        protected override Vector3 GetDragForce()
        {
            return AirResistance.CalculateDragForce(rb.linearVelocity.y, drag);
        }
    }
}
