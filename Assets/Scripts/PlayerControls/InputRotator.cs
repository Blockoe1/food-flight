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
        private const float IDEAL_Z_ANGLE = 45;
        private const float IDEAL_X_ANGLE = 90;
        #endregion



        [Header("Rotation Settings")]
        [SerializeField] private Vector3 defaultEuler = new Vector3(-90, 0, 0);
        //[SerializeField] private float yawRotationSpeed;
        //[SerializeField, Range(0, 1), Tooltip("The amount of deadzone to apply to the dive stick.  " +
        //    "Higher numbers help differentiate from rotation and dive.")] 
        //private float diveDeadzone;

        private InputAction moveAction;
        private InputAction diveAction;

        private Quaternion defaultRotation;
        private Vector2 moveInput;
        private Vector2 diveInput;

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
            //Debug.Log("Move Canceled");
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
        }

        /// <summary>
        /// Every FixedUpdate, apply any unapplied gyro rotation.
        /// </summary>
        protected override void FixedUpdate()
        {
            // Calculate the target direction based on input.
            //Vector3 targetDirection = Vector3.up;
            //Vector3 rotCorrection = Vector3.zero;

            //Quaternion rot = defaultRotation;

            ////MoveInput
            //// X = roll axis.
            //Quaternion rollQuat = moveInput.x > 0 ? IDEAL_XP_QUAT : IDEAL_XN_QUAT;

            //// Adjust the roll quat based on dive.
            //Quaternion rollAdjust = Quaternion.Euler(0, 90, 0) * rollQuat;
            //rollQuat = Quaternion.SlerpUnclamped(rollQuat, rollAdjust, moveInput.x * diveInput.y);

            //rot = Quaternion.Slerp(rot, rollQuat, Mathf.Abs(moveInput.x));
            //// Y = pitch axis
            //Quaternion pitchQuat = moveInput.y > 0 ? IDEAL_ZP_QUAT : IDEAL_ZN_QUAT;

            //// Adjust the pitch quat based on moveInput.magnitude so that backwards moving 
            //if (moveInput.y < 0)
            //{
            //    Quaternion pitchAdjust = Quaternion.Euler(-90, 0, 0) * pitchQuat;
            //    pitchQuat = Quaternion.Slerp(pitchQuat, pitchAdjust, moveInput.magnitude * Mathf.Abs(diveInput.y));
            //}

            //rot = Quaternion.Slerp(rot, pitchQuat, Mathf.Abs(moveInput.y));


            //// Add some Slerping between the movement and dive.
            //float moveBias = Mathf.Lerp(1f, 0.5f, moveInput.magnitude);
            //rot = Quaternion.SlerpUnclamped(rot, IDEAL_DIVE_QUAT, diveInput.y * moveBias);

            // Adjust the default rotation based on dive input.
            Quaternion rot = Quaternion.SlerpUnclamped(defaultRotation, Quaternion.identity, diveInput.y);

            float xSign = diveInput.y < 0 ? -1 : 1;
            float ySign = (Mathf.Abs(diveInput.y) + (Mathf.Abs(moveInput.y) / 2)) >= 1 ? -1 : 1;
            Quaternion pitchQuat = Quaternion.Euler(Mathf.LerpUnclamped(0, IDEAL_Z_ANGLE, ySign * moveInput.y), 0, 0);
            Quaternion rollQuat = Quaternion.Euler(0, Mathf.LerpUnclamped(0, IDEAL_X_ANGLE, xSign * moveInput.x), 0);
            targetRotation = rot * pitchQuat * rollQuat;

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
