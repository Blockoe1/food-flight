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
    public class BurgerDebuffer : MonoBehaviour
    {
        [Header("Air Resistance")]
        [SerializeField] private float verticalVelocity;
        [SerializeField] private float horizontalVelocity;
        [Header("Movement")]
        [SerializeField] private float maxDriftSpeed;
        [SerializeField] private float driftAcceleration;
        [Header("Misc")]
        [SerializeField] private float gravityBias;
        [SerializeField] private float slapCooldown;

        private float burgerVerticalDrag;
        private float burgerHorizontalDrag;
        private float normalVerticalDrag;
        private float normalHorizontalDrag;

        private float normalDriftSpeed;
        private float normalDriftAcceleration;
        private float normalGravityBias;
        private float normalSlapCooldown;

        #region Component References
        [Header("Components")]
        [SerializeReference, ReadOnly] private PlayerAirResistance airResistance;
        [SerializeReference, ReadOnly] private SkydivingMovement movement;
        [SerializeReference, ReadOnly] private ObjectGravitation gravitator;
        [SerializeReference, ReadOnly] private Slapper slapper;

        [ContextMenu("Get Component References")]
        private void Reset()
        {
            airResistance = GetComponent<PlayerAirResistance>();
            movement = GetComponent<SkydivingMovement>();
            gravitator = GetComponent<ObjectGravitation>();
            slapper = GetComponent<Slapper>();
        }

        #endregion

        /// <summary>
        /// Calculate the corresponding drag values and get them from the air resistance script.
        /// </summary>
        private void Awake()
        {
            normalHorizontalDrag = airResistance.HorizontalDrag;
            normalVerticalDrag = airResistance.VerticalDrag;

            normalDriftSpeed = movement.MaxDriftSpeed;
            normalDriftAcceleration = movement.DriftAcceleration;

            normalGravityBias = gravitator.GravityBias;

            normalSlapCooldown = slapper.SlapCooldown;

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

            movement.MaxDriftSpeed = maxDriftSpeed;
            movement.DriftAcceleration = driftAcceleration;

            gravitator.GravityBias = gravityBias;
            slapper.SlapCooldown = slapCooldown;
        }

        /// <summary>
        /// Returns to normal air resistance.
        /// </summary>
        public void SetNormal()
        {
            airResistance.HorizontalDrag = normalHorizontalDrag;
            airResistance.VerticalDrag = normalVerticalDrag;

            movement.MaxDriftSpeed = normalDriftSpeed;
            movement.DriftAcceleration = normalDriftAcceleration;

            gravitator.GravityBias = normalGravityBias;
            slapper.SlapCooldown = normalSlapCooldown;
        }
    }
}
