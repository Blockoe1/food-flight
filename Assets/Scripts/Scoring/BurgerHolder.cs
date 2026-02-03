/*****************************************************************************
// File Name : BurgerHolder.cs
// Author : Brandon Koederitz
// Creation Date : 2/3/2026
// Last Modified : 2/3/2026
//
// Brief Description :  Allows a player to hold the burger and tracks the player's current score.
*****************************************************************************/
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace FoodFlight
{
    public class BurgerHolder : MonoBehaviour
    {
        [SerializeField] private float scoreInterval;
        [SerializeField] private int scoreAmount;
        [SerializeField] private float dropDisableTime;
        [SerializeField] private UnityEvent OnGainBurger;

        private BurgerScript heldBurger;
        private int score;

        public event Action<int, int> OnScoreUpdate;

        #region Properties
        private bool HoldingBurger => heldBurger != null;
        private int Score
        {
            get { return score; }
            set
            {
                int scoreChange = value - score;
                score = value;
                OnScoreUpdate?.Invoke(score, scoreChange);
            }
        }
        #endregion

        /// <summary>
        /// Grabs the burger when the player enters it's grab radius.
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            if (!HoldingBurger && other.gameObject.TryGetComponent(out BurgerScript burger))
            {
                GrabBurger(burger);
            }
        }

        /// <summary>
        /// Has the player grab the burger.
        /// </summary>
        /// <param name="burger"></param>
        private void GrabBurger(BurgerScript burger)
        {
            heldBurger = burger;

            // Disable the burger while it's held.
            OnGainBurger?.Invoke();
            heldBurger.gameObject.SetActive(false);

            StartCoroutine(BurgerScoreRoutine());
        }

        /// <summary>
        /// Causes this player to drop the burger if it is held.
        /// </summary>
        /// <param name="burgerForce">The force to apply the burger when it is dropped.</param>
        public void DropBurger(Vector3 burgerForce)
        {
            if (heldBurger == null) { return; }

            // Snap the burger to the player's position.
            heldBurger.transform.position = transform.position;
            heldBurger.gameObject.SetActive(true);

            // Give the burger it's designated 
            heldBurger.Body.AddForce(burgerForce, ForceMode.Impulse);

            heldBurger.DisableGrabbing(dropDisableTime);

            heldBurger = null;

            Debug.Log(name + " dropped the burger.");
        }

        /// <summary>
        /// Continually increases the player's socre while holding the burger.
        /// </summary>
        /// <param name="heldBurger">The GameObject representing the burger this player is holding.</param>
        /// <returns></returns>
        private IEnumerator BurgerScoreRoutine()
        {
            while(HoldingBurger)
            {
                Score += scoreAmount;
                yield return new WaitForSeconds(scoreInterval);
            }
        }
    }
}
