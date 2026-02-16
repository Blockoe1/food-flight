/*****************************************************************************
// File Name : PlayerRotator.cs
// Author : Brandon Koederitz
// Creation Date : 1/27/2026
// Last Modified : 1/27/2026
//
// Brief Description :  Controls rotating the player based on Gyroscope/Stick Input.
*****************************************************************************/
using CustomAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FoodFlight
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(InputSynchronizer))]
    public abstract class PlayerRotator : MonoBehaviour
    {
        #region CONSTS
        private const string RESET_ACTION_NAME = "Reset";
        #region Ideal Drift Vectors
        // Stick controls use reversed ideal vectors from the SkydivingMovement script since perpendicular is ideal
        // in the movement script.
        //private static readonly Vector3 IDEAL_NEG_X_DRIFT_VECTOR = new Vector3(1, 1, 0).normalized;
        //private static readonly Vector3 IDEAL_X_DRIFT_VECTOR = new Vector3(-1, 1, 0).normalized;
        //private static readonly Vector3 IDEAL_NEG_Z_DRIFT_VECTOR = new Vector3(0, 1, -1).normalized;
        //private static readonly Vector3 IDEAL_Z_DRIFT_VECTOR = new Vector3(0, 1, 1).normalized;
        //private const float IDEAL_Z_MOVE_ANGLE = -45f;
        //private const float IDEAL_X_MOVE_ANGLE = 45f;

        protected static readonly Quaternion IDEAL_XP_QUAT = Quaternion.Euler(-135, -90, 90);
        protected static readonly Quaternion IDEAL_XN_QUAT = Quaternion.Euler(-45, -90, 90);
        protected static readonly Quaternion IDEAL_ZP_QUAT = Quaternion.Euler(-45, 0, 0);
        protected static readonly Quaternion IDEAL_ZN_QUAT = Quaternion.Euler(-135, 0, 0);
        protected static readonly Quaternion IDEAL_DIVE_QUAT = Quaternion.identity;
        #endregion
        #endregion

        [SerializeField, Range(0, 1f), Tooltip("Controls the speed that this players rotation interpolates between " +
    "it's current and target rotation based on controller gyro.")]
        private float rotationSlerpSpeed = 0.5f;

        protected Quaternion targetRotation = Quaternion.identity;

        private InputAction resetRotationAction;

        #region Component References
        [Header("Components")]
        [SerializeReference, ReadOnly] protected Rigidbody rb;
        [SerializeReference, ReadOnly] protected PlayerInput input;
        [SerializeReference, ReadOnly] protected InputSynchronizer inSync;

        [ContextMenu("Get Component References")]
        protected virtual void Reset()
        {
            rb = GetComponent<Rigidbody>();
            input = GetComponent<PlayerInput>();
            inSync = GetComponent<InputSynchronizer>();
        }

        #endregion

        protected virtual void Awake()
        {
            resetRotationAction = input.currentActionMap.FindAction(RESET_ACTION_NAME);
            inSync.OnControlSchemeChanged += CheckEnabled;
        }
        protected virtual void OnDestroy()
        {
            inSync.OnControlSchemeChanged -= CheckEnabled;
        }

        /// <summary>
        /// Setup the reset function event when the component is enabled.
        /// </summary>
        protected virtual void OnEnable()
        {
            resetRotationAction.performed += ResetRotationAction_performed;
        }
        protected virtual void OnDisable()
        {
            resetRotationAction.performed -= ResetRotationAction_performed;
        }

        private void ResetRotationAction_performed(InputAction.CallbackContext obj)
        {
            ResetRotation();
        }

        /// <summary>
        /// Checks if this rotator should be enabled based on if the control scheme has gyro or not.
        /// </summary>
        /// <param name="hasGyro">True if gyro is supported, false if not.</param>
        protected abstract void CheckEnabled(bool hasGyro);

        /// <summary>
        /// Resets this player back to their default rotation.
        /// </summary>
        public virtual void ResetRotation()
        {
            rb.rotation = Quaternion.identity;
            targetRotation = Quaternion.identity;
        }

        /// <summary>
        /// Slerp towards the target rotation every FixedUpdate.
        /// </summary>
        protected virtual void FixedUpdate()
        {
            // Slerp towards the target rotation.
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSlerpSpeed);
        }
    }
}
