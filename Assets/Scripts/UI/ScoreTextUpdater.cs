/*****************************************************************************
// File Name : ScoreTextUpdater.cs
// Author : Brandon Koederitz
// Creation Date : 2/3/2026
// Last Modified : 2/3/2026
//
// Brief Description :  Updates the score displayed on this text when the linked player's score changes.
*****************************************************************************/
using TMPro;
using UnityEngine;

namespace FoodFlight
{
    public class ScoreTextUpdater : MonoBehaviour
    {
        [SerializeField] private TMP_Text textComponent;
        [SerializeField] private BurgerHolder player;

        /// <summary>
        /// Subscribe/Unsubscribe events.
        /// </summary>
        private void Awake()
        {
            player.OnScoreUpdate += UpdateScore;
        }
        private void OnDestroy()
        {
            player.OnScoreUpdate -= UpdateScore;
        }

        /// <summary>
        /// Updates the score displayed on this text component to match the player's score.
        /// </summary>
        /// <param name="currentScore"></param>
        /// <param name="scoreChange"></param>
        private void UpdateScore(int currentScore, int scoreChange)
        {
            textComponent.text = currentScore.ToString();
        }
    }
}
