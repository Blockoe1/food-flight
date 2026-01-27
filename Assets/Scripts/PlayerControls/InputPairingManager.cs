/*****************************************************************************
// File Name : InputPairingManager.cs
// Author : Brandon Koederitz
// Creation Date : 1/27/2026
// Last Modified : 1/27/2026
//
// Brief Description :  Sets up controller pairing for all players.
*****************************************************************************/
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace FoodFlight
{
    public class InputPairingManager : MonoBehaviour
    {
        [SerializeField] private int pairDelay;
        [SerializeField] private InputSynchronizer[] players;
        [SerializeField] private UnityEvent<string> OnPlayerPairing;
        [SerializeField] private UnityEvent OnPairingEnd;

        private CancellationTokenSource pairToken;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            PairControllers();
        }

        /// <summary>
        /// Cancell any pairing when the game ends.
        /// </summary>
        private void OnDestroy()
        {
            CancelPairing();
        }

        /// <summary>
        /// Public interface functions for pairing controllers to each player.
        /// </summary>
        public void PairControllers()
        {
            pairToken = new CancellationTokenSource();
            PairControllersAsync(pairToken.Token);
        }
        public void CancelPairing()
        {
            pairToken.Cancel();
        }

        /// <summary>
        /// Pair a controller to all players.
        /// </summary>
        public async Task PairControllersAsync(CancellationToken ct)
        {
            // Remove any previous pairing.
            foreach(var controller in players)
            {
                controller.Unsync();
            }

            // Load the JSL and InputSystem controllers.
            int jslNumConnected = JSL.JslConnectDevices();
            InputDevice[] inputDevices = InputSystem.devices.ToArray();

            // Sequentially pair each controller and update UI.
            foreach (var controller in players)
            {
                Debug.Log("Pairing Controllers " + controller.name);
                OnPlayerPairing?.Invoke(controller.name);
                await controller.PairControllers(jslNumConnected, inputDevices, ct);
                // Add an additional buffer delay between pairing each controller.
                await Task.Delay(pairDelay);
            }

            // Call a cleanup event.
            OnPairingEnd?.Invoke();
        }
    }
}
