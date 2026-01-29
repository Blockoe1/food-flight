/*****************************************************************************
// File Name : StickRotator.cs
// Author : Brandon Koederitz
// Creation Date : 1/28/2026
// Last Modified : 1/28/2026
//
// Brief Description :  Controls player rotation based on stick input.  The target rotation is based on using left stick
// as 2D movement, not rotation.
*****************************************************************************/
using CustomAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FoodFlight
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputRotator : PlayerRotator
    {
        #region CONSTS
        private const string MOVE_ACTION_NAME = "Move";
        private const string DIVE_ACTION_NAME = "Dive";
        #endregion

        #region Ideal Drift Vectors
        // Stick controls use reversed ideal vectors from the SkydivingMovement script since perpendicular is ideal
        // in the movement script.
        private static readonly Vector3 IDEAL_NEG_X_DRIFT_VECTOR = new Vector3(-1, -1, 0).normalized;
        private static readonly Vector3 IDEAL_X_DRIFT_VECTOR = new Vector3(1, -1, 0).normalized;
        private static readonly Vector3 IDEAL_NEG_Z_DRIFT_VECTOR = new Vector3(0, -1, 1).normalized;
        private static readonly Vector3 IDEAL_Z_DRIFT_VECTOR = new Vector3(0, -1, -1).normalized;
        #endregion


        private InputAction moveAction;
        private InputAction diveAction;

        private Vector2 moveInput;
        private Vector2 diveInput;

        #region Component References
        [Header("Components")]
        [SerializeReference, ReadOnly] private PlayerInput input;

        [ContextMenu("Get Component References")]
        protected override void Reset()
        {
            base.Reset();
            input = GetComponent<PlayerInput>();
        }
        #endregion

        /// <summary>
        /// Setup/Unsubscribe input.
        /// </summary>
        private void Awake()
        {
            moveAction = input.actions.FindAction(MOVE_ACTION_NAME);
            diveAction = input.actions.FindAction(DIVE_ACTION_NAME);

            moveAction.performed += MoveAction_performed;
            moveAction.canceled += MoveAction_canceled;

            diveAction.performed += DiveAction_performed;
            diveAction.canceled += DiveAction_canceled;
        }
        private void OnDestroy()
        {
            moveAction.performed -= MoveAction_performed;
            moveAction.canceled -= MoveAction_canceled;

            diveAction.performed -= DiveAction_performed;
            diveAction.canceled -= DiveAction_canceled;
        }

        #region Input Handlers
        /// <summary>
        /// Handle move input
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void MoveAction_performed(InputAction.CallbackContext obj)
        {
            moveInput = obj.ReadValue<Vector2>();

            // X axis controls roll axis that translates into X movement.

            // Y axis controls pitch axis that translates into Y movement (affected by dive).
        }
        private void MoveAction_canceled(InputAction.CallbackContext obj)
        {
            moveInput = Vector2.zero;
        }

        private void DiveAction_performed(InputAction.CallbackContext obj)
        {
            diveInput = obj.ReadValue<Vector2>();

            // X Axis controls Yaw axis rotation.

            // Y Axis controls dive.
        }
        private void DiveAction_canceled(InputAction.CallbackContext obj)
        {
            diveInput = Vector2.zero;
        }
        #endregion

        /// <summary>
        /// Every FixedUpdate, apply any unapplied gyro rotation.
        /// </summary>
        protected override void FixedUpdate()
        {
            // Calculate the target direction based on input.
            Vector3 targetDirection = Vector3.zero;

            // Get the target rotation based on our target direction.
            targetRotation = Quaternion.LookRotation(targetDirection, Vector3.forward);

            // Always run the base FixedUpdate after target rotation has been set.
            base.FixedUpdate();
        }
    }
}
