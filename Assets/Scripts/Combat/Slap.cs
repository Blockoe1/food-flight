/*****************************************************************************
// File Name : Attacker.cs
// Author : Brandon Koederitz
// Creation Date : 2/2/2026
// Last Modified : 2/2/2026
//
// Brief Description :  Controls enabling hitboxes to attack other players.
*****************************************************************************/
using CustomAttributes;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FoodFlight
{
    [RequireComponent(typeof(PlayerInput))]
    public class Slap : MonoBehaviour
    {
        [SerializeField] private float attackHitboxTime;
        [SerializeField] private AttackPairing[] attacks;

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

            private Slap attackerRef;
            private float attackHitboxTime;
            private bool isAttacking;
            private InputAction action;

            /// <summary>
            /// Initializes this attack with shared settings.
            /// </summary>
            /// <param name="attackHitboxTime">The amount of time the hitbox exists.</param>
            internal void Initialize(float attackHitboxTime, Slap attackerRef)
            {
                this.attackHitboxTime = attackHitboxTime;
                this.attackerRef = attackerRef;
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
                // Prevent double atttacks.
                if (isAttacking) { return; }
                attackerRef.StartCoroutine(HitboxRoutine(attackHitboxTime));
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
                pair.Initialize(attackHitboxTime, this);
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
    }
}
