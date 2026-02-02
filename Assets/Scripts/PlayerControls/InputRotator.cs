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
        //private static readonly Vector3 IDEAL_NEG_X_DRIFT_VECTOR = new Vector3(1, 1, 0).normalized;
        //private static readonly Vector3 IDEAL_X_DRIFT_VECTOR = new Vector3(-1, 1, 0).normalized;
        //private static readonly Vector3 IDEAL_NEG_Z_DRIFT_VECTOR = new Vector3(0, 1, -1).normalized;
        //private static readonly Vector3 IDEAL_Z_DRIFT_VECTOR = new Vector3(0, 1, 1).normalized;
        //private const float IDEAL_Z_MOVE_ANGLE = -45f;
        //private const float IDEAL_X_MOVE_ANGLE = 45f;

        private static readonly Quaternion IDEAL_XP_QUAT = Quaternion.Euler(-135, -90, 90);
        private static readonly Quaternion IDEAL_XN_QUAT = Quaternion.Euler(-45, -90, 90);
        private static readonly Quaternion IDEAL_ZP_QUAT = Quaternion.Euler(-45, 0, 0);
        private static readonly Quaternion IDEAL_ZN_QUAT = Quaternion.Euler(-135, 0, 0);
        private static readonly Quaternion IDEAL_DIVE_QUAT = Quaternion.identity;
        #endregion

        [Header("Rotation Settings")]
        [SerializeField] private Vector3 defaultEuler = new Vector3(-90, 0, 0);
        [SerializeField] private float yawRotationSpeed;
        [SerializeField, Range(0, 1), Tooltip("The amount of deadzone to apply to the dive stick.  " +
            "Higher numbers help differentiate from rotation and dive.")] 
        private float diveDeadzone;

        private InputAction moveAction;
        private InputAction diveAction;

        private Quaternion defaultRotation;
        [SerializeField, ReadOnly] private Vector2 moveInput;
        [SerializeField, ReadOnly] private Vector2 diveInput;
        private float yawAngle;

        /// <summary>
        /// Setup/Subscribe/Unsubscribe input.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            moveAction = input.currentActionMap.FindAction(MOVE_ACTION_NAME);
            diveAction = input.currentActionMap.FindAction(DIVE_ACTION_NAME);

            defaultRotation = Quaternion.Euler(defaultEuler);
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
            Debug.Log("Move Canceled");
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
            Vector3 targetDirection = Vector3.up;
            Vector3 rotCorrection = Vector3.zero;

            //// Dive Input
            //// Y Axis controls dive.  Increases how vertical the player is.
            //targetDirection = Vector3.Lerp(targetDirection, Vector3.forward, Mathf.Abs(diveInput.y));

            //// Move Input
            //// X axis controls roll axis that translates into X movement.
            //Vector2 xVector = moveInput.x > 0 ? IDEAL_X_DRIFT_VECTOR : IDEAL_NEG_X_DRIFT_VECTOR;
            //targetDirection = Vector3.Lerp(targetDirection, xVector, Mathf.Abs(moveInput.x));
            //// Add correction to make the player face the right direction.
            //rotCorrection.x += -90 * Mathf.Abs(System.MathF.Sign(moveInput.x));
            //rotCorrection.y += 90 * System.MathF.Sign(moveInput.x);
            ////// Y axis controls pitch axis that translates into Z movement (affected by dive).
            //Vector3 zVector = moveInput.y > 0 ? IDEAL_Z_DRIFT_VECTOR : IDEAL_NEG_Z_DRIFT_VECTOR;
            //targetDirection = Vector3.Lerp(targetDirection, zVector, Mathf.Abs(moveInput.y));
            //// Only need to apply correction for negative Z input.
            //if (moveInput.y < 0)
            //{
            //    rotCorrection.y += -180;
            //    rotCorrection.x -= -180;
            //}
            //Vector3 appliedZVector = zVector * System.MathF.Sign(moveInput.y);
            //(appliedZVector.y, appliedZVector.z) = (appliedZVector.z, appliedZVector.y);
            //targetDirection += appliedZVector;

            // Swapping to a Euler system since I dont think we need to worry about gimbal lock and it's way simpler.
            // MoveInput
            // X Axis control roll axis.
            //Vector3 yEuler = Vector3.zero;
            //yEuler.y = IDEAL_X_MOVE_ANGLE * moveInput.x;
            //Quaternion yQuat = Quaternion.Euler(yEuler);
            //// Y Axis controls 
            //Vector3 xEuler = Vector3.zero;
            //xEuler.x = IDEAL_Z_MOVE_ANGLE * moveInput.y;
            //Quaternion xQuat = Quaternion.Euler(xEuler);

            //// X Axis of Dive controls Yaw axis rotation.
            ////yawAngle += diveInput.x * Time.fixedDeltaTime * yawRotationSpeed;
            ////Quaternion yawRotation = Quaternion.Euler(0, yawAngle, 0);

            ////Debug.Log($"Target Direction: {targetDirection}.  Yaw Angle: {yawAngle}.");
            //Debug.Log($"y Eulers: {yEuler}.  xEuler: {xEuler}");

            //Quaternion correction = Quaternion.Euler(rotCorrection);

            // Get the target rotation based on our target direction.
            //targetRotation = Quaternion.LookRotation(targetDirection.normalized, Vector3.up) * correction;

            //targetRotation = xQuat * yQuat * Quaternion.AngleAxis(-90, Vector3.right);
            //targetRotation = targetRotation * defaultRotation;
            Quaternion rot = defaultRotation;

            // SLERP towards dive first.
            if (Mathf.Abs(diveInput.y) > diveDeadzone)
            {
                rot = Quaternion.Slerp(rot, IDEAL_DIVE_QUAT, Mathf.Abs(diveInput.y));
            }

            //MoveInput
            // X = roll axis.
            Quaternion rollQuat = moveInput.x > 0 ? IDEAL_XP_QUAT : IDEAL_XN_QUAT;
            rot = Quaternion.Slerp(rot, rollQuat, Mathf.Abs(moveInput.x));
            // Y = pitch axis
            Quaternion pitchQuat = moveInput.y > 0 ? IDEAL_ZP_QUAT : IDEAL_ZN_QUAT;
            rot = Quaternion.Slerp(rot, pitchQuat, Mathf.Abs(moveInput.y));

            // Dive Input
            // X = yaw
            // Only take into account significant rotations.
            if (Mathf.Abs(diveInput.x) > diveDeadzone)
            {
                yawAngle += diveInput.x * Time.fixedDeltaTime * yawRotationSpeed;
            }
            Quaternion yawRotation = Quaternion.Euler(0, yawAngle, 0);
            rot = yawRotation * rot;

            targetRotation = rot;

            // Always run the base FixedUpdate after target rotation has been set.
            base.FixedUpdate();
        }

        /// <summary>
        /// The InputRotator should be enabled if the player does not have gyro.
        /// </summary>
        /// <param name="hasGyro">True if gyro is enabled.</param>
        protected override void CheckEnabled(bool hasGyro)
        {
            enabled = !hasGyro;
        }
    }
}
