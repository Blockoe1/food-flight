/*****************************************************************************
// File Name : PauseMenu.cs
// Author : Brandon Koederitz
// Creation Date : 2/4/2026
// Last Modified : 2/4/2026
//
// Brief Description :  Controls the opening and closing of the pause menu.
*****************************************************************************/
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FoodFlight
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private Button initialButton;
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private InputActionReference pauseAction;
        [SerializeField] private UnityEvent<bool> OnMenuToggled;

        private bool isPaused;

        /// <summary>
        /// Setup Input.
        /// </summary>
        private void Awake()
        {
            pauseAction.action.performed += Pause_pressed;
        }
        private void OnDestroy()
        {
            pauseAction.action.performed -= Pause_pressed;
        }

        /// <summary>
        /// Toggle the current pause state when the pause button is pressed.
        /// </summary>
        /// <param name="obj"></param>
        private void Pause_pressed(InputAction.CallbackContext obj)
        {
            SetPaused(!isPaused);
        }

        /// <summary>
        /// Sets the current pause state of the game.
        /// </summary>
        /// <param name="paused"></param>
        public void SetPaused(bool paused)
        {
            // Prevent any pausing if the game is paused from an external source.
            if (Time.timeScale == 0 && !isPaused)
            {
                return;
            }

            isPaused = paused;
            pauseMenuPanel.SetActive(isPaused);
            Time.timeScale = isPaused ? 0 : 1;
            OnMenuToggled?.Invoke(isPaused);

            if (isPaused)
            {
                initialButton.Select();
            }
        }

        /// <summary>
        /// Switches the control scheme during gameplay.
        /// </summary>
        /// <param name="controlScheme"></param>
        public void SetControlScheme(int controlScheme)
        {
            // Unpauses the pause menu so that it doesn't bug with the InputManager.
            SetPaused(false);

            InputPairingManager.SetControlScheme((ControlScheme)controlScheme);
        }

        public void ReturnToMenu()
        {
            SetPaused(false);
            SceneManager.LoadScene("MainMenu");
        }
    }
}
