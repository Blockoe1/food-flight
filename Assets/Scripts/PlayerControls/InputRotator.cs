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

        [Header("Rotation Settings")]
        [SerializeField] private float yawRotationSpeed;


        private InputAction moveAction;
        private InputAction diveAction;

        [SerializeField, ReadOnly] private Vector2 moveInput;
        [SerializeField, ReadOnly] private Vector2 diveInput;
        private float yawAngle;

        /// <summary>
        /// Setup/Subscribe/Unsubscribe input.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            moveAction = input.actions.FindAction(MOVE_ACTION_NAME);
            diveAction = input.actions.FindAction(DIVE_ACTION_NAME);
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            moveAction.performed += MoveAction_performed;
            moveAction.canceled += MoveAction_canceled;

            diveAction.performed += DiveAction_performed;
            diveAction.canceled += DiveAction_canceled;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            moveAction.performed -= MoveAction_performed;
            moveAction.canceled -= MoveAction_canceled;

            diveAction.performed -= DiveAction_performed;
            diveAction.canceled -= DiveAction_canceled;
        }

        #region Input Handlers
        /// <summary>
        /// Read move input.
        /// </summary>
        /// <param name="obj"></param>
        private void MoveAction_performed(InputAction.CallbackContext obj)
        {
            moveInput = obj.ReadValue<Vector2>();
        }
        private void MoveAction_canceled(InputAction.CallbackContext obj)
        {
            moveInput = Vector2.zero;
        }

        /// <summary>
        /// Read dive input.
        /// </summary>
        /// <param name="obj"></param>
        private void DiveAction_performed(InputAction.CallbackContext obj)
        {
            diveInput = obj.ReadValue<Vector2>();
        }
        private void DiveAction_canceled(InputAction.CallbackContext obj)
        {
            diveInput = Vector2.zero;
        }
        #endregion

        /// <summary>
        /// When resetting rotation, need to reset the stored yaw angle.
        /// </summary>
        public override void ResetRotation()
        {
            base.ResetRotation();
            yawAngle = 0;
        }

        /// <summary>
        /// Every FixedUpdate, apply any unapplied gyro rotation.
        /// </summary>
        protected override void FixedUpdate()
        {
            // Calculate the target direction based on input.
            Vector3 targetDirection = Vector3.zero;

            // Move Input
            // X axis controls roll axis that translates into X movement.
            Vector2 xVector = moveInput.x > 0 ? IDEAL_X_DRIFT_VECTOR : IDEAL_NEG_X_DRIFT_VECTOR;
            Vector3 appliedXVector = xVector * System.MathF.Sign(moveInput.x);
            (appliedXVector.y, appliedXVector.z) = (appliedXVector.z, appliedXVector.y);
            targetDirection += appliedXVector;
            // Y axis controls pitch axis that translates into Z movement (affected by dive).
            Vector2 zVector = moveInput.y > 0 ? IDEAL_Z_DRIFT_VECTOR : IDEAL_NEG_Z_DRIFT_VECTOR;
            Vector3 appliedZVector = zVector * System.MathF.Sign(moveInput.y);
            (appliedZVector.y, appliedZVector.z) = (appliedZVector.z, appliedZVector.y);
            targetDirection += appliedZVector;

            // Dive Input
            // Y Axis controls dive.  Increases how vertical the player is.
            targetDirection.y -= Mathf.Abs(diveInput.y);
            // X Axis controls Yaw axis rotation.
            yawAngle += diveInput.x * Time.fixedDeltaTime * yawRotationSpeed;
            Quaternion yawRotation = Quaternion.Euler(0, yawAngle, 0);
            targetDirection = yawRotation * targetDirection;

            Debug.Log($"Target Direction: {targetDirection}");

            // Get the target rotation based on our target direction.
            targetRotation = Quaternion.LookRotation(targetDirection, Vector3.forward);

            // Always run the base FixedUpdate after target rotation has been set.
            base.FixedUpdate();
        }
    }
}
