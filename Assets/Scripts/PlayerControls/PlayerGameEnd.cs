/*****************************************************************************
// File Name : PlayerGameEnd.cs
// Author : Brandon Koederitz
// Creation Date : 2/4/2026
// Last Modified : 2/4/2026
//
// Brief Description :  Ends the game when both players hit the floor.
*****************************************************************************/
using CustomAttributes;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

namespace FoodFlight
{
    public class PlayerGameEnd : MonoBehaviour
    {
        #region CONSTS
        private const string END_GAME_TAG = "EndGame";
        #endregion

        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private MonoBehaviour[] disabledComponents;
        [SerializeField] private UnityEvent OnPlayerEnd;

        private bool hasEnded;

        #region Component References
        [Header("Components")]
        [SerializeReference, ReadOnly] private BurgerHolder burgerHolder;

        [ContextMenu("Get Component References")]
        private void Reset()
        {
            burgerHolder = GetComponent<BurgerHolder>();
        }
        #endregion

        /// <summary>
        /// Detects when a player hits the end of the level.
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter(Collider collision)
        {
            if (hasEnded) { return; }
            if (collision.gameObject.CompareTag(END_GAME_TAG))
            {
                hasEnded = true;
                if (scoreText != null)
                {
                    scoreText.text = burgerHolder.Score.ToString();
                }
                OnPlayerEnd?.Invoke();

                // Disable all set components.
                foreach (var component in disabledComponents)
                {
                    component.enabled = false;
                }
            }
        }
    }
}
