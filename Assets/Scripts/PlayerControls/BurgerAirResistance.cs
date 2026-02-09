/*****************************************************************************
// File Name : BurgerAirResistance.cs
// Author : Brandon Koederitz
// Creation Date : 2/5/2026
// Last Modified : 2/5/2026
//
// Brief Description :  Sets custom air resistance values 
*****************************************************************************/
using CustomAttributes;
using UnityEngine;

namespace FoodFlight
{
    [RequireComponent(typeof(PlayerAirResistance))]
    public class BurgerAirResistance : MonoBehaviour
    {
        [SerializeField] private float verticalVelocity;
        [SerializeField] private float horizontalVelocity;

        private float burgerVerticalDrag;
        private float burgerHorizontalDrag;
        private float normalVerticalDrag;
        private float normalHorizontalDrag;

        #region Component References
        [Header("Components")]
        [SerializeReference, ReadOnly] private PlayerAirResistance airResistance;

        [ContextMenu("Get Component References")]
        private void Reset()
        {
            airResistance = GetComponent<PlayerAirResistance>();
        }

        #endregion

        /// <summary>
        /// Calculate the corresponding drag values and get them from the air resistance script.
        /// </summary>
        private void Awake()
        {
            normalHorizontalDrag = airResistance.HorizontalDrag;
            normalVerticalDrag = airResistance.VerticalDrag;

            // Calculate the drag while holding the burger.
            burgerHorizontalDrag = AirResistance.CalculateDragFromTVelocity(horizontalVelocity, airResistance.Mass);
            burgerVerticalDrag = AirResistance.CalculateDragFromTVelocity(verticalVelocity, airResistance.Mass);
        }

        /// <summary>
        /// Enables burger air resistance.
        /// </summary>
        public void SetBurger()
        {
            airResistance.HorizontalDrag = burgerHorizontalDrag;
            airResistance.VerticalDrag = burgerVerticalDrag;
        }

        /// <summary>
        /// Returns to normal air resistance.
        /// </summary>
        public void SetNormal()
        {
            airResistance.HorizontalDrag = normalHorizontalDrag;
            airResistance.VerticalDrag = normalVerticalDrag;
        }
    }
}
