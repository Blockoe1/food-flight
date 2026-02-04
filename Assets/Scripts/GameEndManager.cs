/*****************************************************************************
// File Name : GameEndManager.cs
// Author : Brandon Koederitz
// Creation Date : 2/3/2026
// Last Modified : 2/3/2026
//
// Brief Description :  Shows new options once all players have hit the ground.
*****************************************************************************/
using UnityEngine;
using UnityEngine.Events;

namespace FoodFlight
{
    public class GameEndManager : MonoBehaviour
    {
        [SerializeField] private int numPlayers = 2;
        [SerializeField] private UnityEvent OnGameEnd;

        private int playersLogged;
        private bool hasEnded = false;

        /// <summary>
        /// Loggs a player as complete.
        /// </summary>
        public void LogPlayerEnded()
        {
            playersLogged++;
            if (!hasEnded && playersLogged >= numPlayers)
            {
                hasEnded = true;
                OnGameEnd?.Invoke();
            }
        }
    }
}
