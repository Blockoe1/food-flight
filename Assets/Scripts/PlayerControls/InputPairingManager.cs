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
        [SerializeField, Tooltip("Delay in ms to wait between pairing controllers.")] private int pairDelay;
        [SerializeField] private InputSynchronizer[] players;
        [SerializeField] private UnityEvent OnPairingBegin;
        [SerializeField] private UnityEvent<string> OnPlayerPairing;
        [SerializeField] private UnityEvent<string> OnPlayerCalibrating;
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
            CleanupJSL();
        }

        public void CleanupJSL()
        {
            Debug.Log("Cleaning up JSL");
            // Clean up JSL for all managed players as well.
            foreach(var device in players)
            {
                device.Unsync();
            }
            // If you don't dispose JSL data, it may cause a memory leak.
            JSL.JslDisconnectAndDisposeAll();
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
        /// Attempt to treat JSL.JslConnectDevices as async so there can be an async managed loading screen.
        /// </summary>
        /// <returns></returns>
        private Task<int> JslConnectDevicesAsync()
        {
            return Task.Run(() => JSL.JslConnectDevices());
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

            // Add delay for testing.
            //await Task.Delay(2000);

            // Load the JSL and InputSystem controllers.

            OnPairingBegin?.Invoke();
            int jslNumConnected = await JslConnectDevicesAsync();
            InputDevice[] inputDevices = InputSystem.devices.ToArray();

            Debug.Log($"Connected {jslNumConnected} to JSL.");

            // If cancelled, skip proceeding with pairing.
            if (!ct.IsCancellationRequested)
            {
                // Sequentially pair each controller and update UI.
                foreach (var controller in players)
                {
                    Debug.Log("Pairing Controllers " + controller.name);

                    // Pair the controller.
                    OnPlayerPairing?.Invoke("Pairing " + controller.name);
                    await controller.PairControllers(jslNumConnected, inputDevices, ct);

                    // Stop calibration if this operationo was cancelled.
                    if (ct.IsCancellationRequested)
                    {
                        return;
                    }

                    // Configure the controller's calibration.
                    OnPlayerCalibrating?.Invoke("Calibrating " + controller.name);
                    await controller.CalibrateGyro(ct);

                    // Add an additional buffer delay between pairing each controller.
                    await Task.Delay(pairDelay);
                }
            }

            // Call a cleanup event.
            OnPairingEnd?.Invoke();

            // If the operation has been cancelled, clean up JSL in case OnDestroy has already been called.
            if (ct.IsCancellationRequested)
            {
                CleanupJSL();
                // If a cancel happens after JSL has been loaded, unload it.
                ct.ThrowIfCancellationRequested();
            }
        }
    }
}
