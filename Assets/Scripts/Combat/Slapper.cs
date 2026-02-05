/*****************************************************************************
// File Name : Attacker.cs
// Author : Brandon Koederitz
// Creation Date : 2/2/2026
// Last Modified : 2/2/2026
//
// Brief Description :  Controls enabling hitboxes to attack other players.
*****************************************************************************/
using CustomAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace FoodFlight
{
    [RequireComponent(typeof(PlayerInput))]
    public class Slapper : MonoBehaviour
    {
        [SerializeField] private float attackHitboxTime;
        [SerializeField] private float slapCooldown;
        [SerializeField] private AttackPairing[] attacks;

        private bool canSlap = true;

        #region Component References
        [Header("Components")]
        [SerializeReference, ReadOnly] protected PlayerInput input;

        [ContextMenu("Get Component References")]
        protected virtual void Reset()
        {
            input = GetComponent<PlayerInput>();
        }
        #endregion

        #region Nested
        [System.Serializable]
        private class AttackPairing
        {
            [SerializeField] private string actionName;
            [SerializeField] private GameObject hitbox;

            private Slapper slapperRef;
            private bool isAttacking;
            private InputAction action;

            /// <summary>
            /// Initializes this attack with shared settings.
            /// </summary>
            /// <param name="attackHitboxTime">The amount of time the hitbox exists.</param>
            internal void Initialize(Slapper attackerRef)
            {
                this.slapperRef = attackerRef;
            }

            /// <summary>
            /// Sets up this pairing's input action
            /// </summary>
            /// <param name="input"></param>
            internal void SetupInput(PlayerInput input)
            {
                action = input.currentActionMap.FindAction(actionName);

                action.performed += Attack_performed;
            }
            internal void CleanUpInput()
            {
                action.performed -= Attack_performed;
            }

            /// <summary>
            /// Performs the attack by enabling the hitbox.
            /// </summary>
            /// <param name="obj"></param>
            private void Attack_performed(InputAction.CallbackContext obj)
            {
                // Prevent double atttacks or attacks that are on cooldown.
                if (isAttacking || !slapperRef.canSlap) { return; }
                slapperRef.StartCoroutine(HitboxRoutine(slapperRef.attackHitboxTime));
            }

            /// <summary>
            /// Enables this attack's hitbox for a certain number of seconds.
            /// </summary>
            /// <param name="hitboxTime">The amount of time the hitbox is active for.</param>
            /// <returns></returns>
            private IEnumerator HitboxRoutine(float hitboxTime)
            {
                isAttacking = true;
                hitbox.SetActive(true);
                yield return new WaitForSeconds(hitboxTime);
                hitbox.SetActive(false);
                isAttacking = false;

                // Put slapping on a brief cooldown.
                slapperRef.DisableSlapping(slapperRef.slapCooldown);
            }
        }
        #endregion

        /// <summary>
        /// Setup/Clean up input.
        /// </summary>
        private void Awake()
        {
            foreach(var pair in attacks)
            {
                pair.Initialize(this);
                pair.SetupInput(input);
            }
        }
        private void OnDestroy()
        {
            foreach(var pair in attacks)
            {
                pair.CleanUpInput();
            }
        }

        /// <summary>
        /// Disables slapping for a period of time.
        /// </summary>
        /// <param name="disabledTime">The amount of time that slapping is disabled for.</param>
        public void DisableSlapping(float disabledTime)
        {
            if (!canSlap) { return; }
            StartCoroutine(DisableSlapRoutine(disabledTime));
        }
        private IEnumerator DisableSlapRoutine(float disabledTime)
        {
            canSlap = false;
            yield return new WaitForSeconds(disabledTime);
            canSlap = true;
        }
    }
}
