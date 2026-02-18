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
        [SerializeField] private Vector3 dropOffset = Vector3.down;
        [SerializeField] private UnityEvent OnGainBurger;
        [SerializeField] private UnityEvent OnScorePoints;
        [SerializeField] private UnityEvent OnLoseBurger;

        private BurgerScript heldBurger;
        private int score;
        private bool isScoring;

        public event Action<int, int> OnScoreUpdate;

        #region Properties
        private bool HoldingBurger => heldBurger != null;
        public int Score
        {
            get { return score; }
            private set
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
            if (!HoldingBurger && other.gameObject.TryGetComponent(out BurgerScript burger) && burger.IsGrabable)
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

            if (!isScoring)
            {
                StartCoroutine(BurgerScoreRoutine());
            }
        }


        public void DropBurger()
        {
            DropBurger(Vector3.zero);   
        }
        /// <summary>
        /// Causes this player to drop the burger if it is held.
        /// </summary>
        /// <param name="burgerForce">The force to apply the burger when it is dropped.</param>
        public void DropBurger(Vector3 burgerForce)
        {
            if (heldBurger == null) { return; }

            heldBurger.DisableGrabbing();

            // Snap the burger to the player's position.
            heldBurger.transform.position = transform.position + dropOffset;
            heldBurger.gameObject.SetActive(true);

            // Give the burger it's designated force.
            heldBurger.Body.AddForce(burgerForce, ForceMode.Impulse);

            heldBurger.ScheduleGrabReenable(dropDisableTime);

            OnLoseBurger?.Invoke();
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
            isScoring = true;
            float timer = 0;
            while(HoldingBurger)
            {
                heldBurger.transform.position = transform.position;

                timer += Time.deltaTime;

                if (timer > scoreInterval)
                {
                    timer = 0;
                    Score += scoreAmount;
                    OnScorePoints?.Invoke();
                }
               
                yield return null;
            }
            isScoring = false;
        }
    }
}
