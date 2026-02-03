/*****************************************************************************
// File Name : InputPairingManager.cs
// Author : Brandon Koederitz
// Creation Date : 1/27/2026
// Last Modified : 1/27/2026
//
// Brief Description :  Sets up controller pairing for all players.
*****************************************************************************/
using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace FoodFlight
{
    public class InputPairingManager : MonoBehaviour
    {
        [SerializeField, Tooltip("Delay in ms to wait between pairing controllers.")] private int pairDelay;
        [SerializeField] private InputSynchronizer[] players;
        [SerializeField] private InputActionReference cancelAction;
        [Header("Events")]
        [SerializeField] private UnityEvent OnPairingBegin;
        [SerializeField] private UnityEvent<string> OnPlayerPairing;
        [SerializeField] private UnityEvent<string> OnPlayerCalibrating;
        [SerializeField] private UnityEvent OnPairingEnd;

        private static ControlScheme controlScheme = ControlScheme.Keyboard;
        private CancellationTokenSource pairToken;

        private static InputPairingManager instance;

        #region Unity Messages
        /// <summary>
        /// Manage singleton instance.
        /// </summary>
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.Log("Mutiple InputPairingManagers found.");
                return;
            }
            else
            {
                instance = this;
            }

            cancelAction.action.performed += CancelAction_performed;
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
            cancelAction.action.performed -= CancelAction_performed;

            // Clean up any JSL and pairing data.
            CancelPairing();
            CleanupJSL();
        }

        /// <summary>
        /// Allows the player to manually cancel pairing via button input.
        /// </summary>
        /// <param name="obj"></param>
        private void CancelAction_performed(InputAction.CallbackContext obj)
        {
            CancelPairing();
        }

        /// <summary>
        /// When the game starts, set the correct control scheme for all managed players.
        /// </summary>
        void Start()
        {
            UpdateControlScheme();
            //SetGyroControls();
        }
        #endregion

        #region Statics
        /// <summary>
        /// Sets the current control scheme for the game to use.
        /// </summary>
        /// <param name="controlScheme">The control scheme to use.</param>
        public static void SetControlScheme(ControlScheme scheme)
        {
            controlScheme = scheme;

            // Immediately set the given control scheme
            if (instance != null)
            {
                instance.UpdateControlScheme();
            }
        }
        #endregion

        /// <summary>
        /// Updates the control scheme of all managed players.
        /// </summary>
        private void UpdateControlScheme()
        {
            switch(controlScheme)
            { 
                case ControlScheme.Keyboard:
                    SetKeyboard();
                    break;
                case ControlScheme.GamepadGyro:
                    SetGyro();
                    break;
                case ControlScheme.Gamepad:
                    SetGamepad();
                    break;
            }
        }

        #region Keyboard Setup
        /// <summary>
        /// Sets up the players with keyboard input.
        /// </summary>
        [ContextMenu("Test Keyboard")]
        public void SetKeyboard()
        {
            CleanupJSL();
            // Set the control scheme of all managed players to the keyboard input device.
            InputDevice keyboard = InputSystem.GetDevice<Keyboard>();
            foreach (var player in players)
            {
                player.OverrideControlScheme(keyboard);
            }
        }
        #endregion

        #region Controller Setup

        /// <summary>
        /// Sets the gamepad control scheme without gyro.
        /// </summary>
        [ContextMenu("Test Gamepad")]
        public void SetGamepad()
        {
            CleanupJSL();
            // Have each player select a gamepad by reading the south button.
            pairToken = new CancellationTokenSource();
            PairControllersAsync(pairToken.Token);
        }

        /// <summary>
        /// Pair a controller to all players.
        /// </summary>
        public async Task PairControllersAsync(CancellationToken ct)
        {
            void OnCanceled()
            {
                // Always claenup JSL if the operation is canceled as OnDestroy may be called too early.
                Debug.Log("Operation PairControllers was canceled");
            }

            Time.timeScale = 0f;

            OnPairingBegin?.Invoke();
            InputDevice[] inputDevices = InputSystem.devices.ToArray();

            // If we've already been canceled, call cancel cleanup.
            if (ct.IsCancellationRequested)
            {
                OnCanceled();
            }
            // If cancelled, skip proceeding with pairing.
            try
            {
                // Sequentially pair each controller and update UI.
                foreach (var controller in players)
                {
                    Debug.Log("Pairing Controllers " + controller.name);

                    // Pair the controller.
                    OnPlayerPairing?.Invoke("Pairing " + controller.name);
                    await controller.PairController(inputDevices, ct);

                    // Add an additional buffer delay between pairing each controller.
                    await Task.Delay(pairDelay);
                }
                Debug.Log("Task PairControllers has finished successfully.");
            }
            catch (OperationCanceledException)
            {
                OnCanceled();
            }
            finally
            {
                // Call a cleanup event.
                OnPairingEnd?.Invoke();
                Time.timeScale = 1f;
                Debug.Log("Operation PairControllers has ended.");
            }
        }
        #endregion
        #region Gyro Input Setup

        /// <summary>
        /// Public interface functions for pairing controllers to each player.
        /// </summary>
        [ContextMenu("Test Gyro")]
        public void SetGyro()
        {
            pairToken = new CancellationTokenSource();
            PairControllersAsyncGyro(pairToken.Token);
        }
        /// <summary>
        /// Cleans up any JSL data.
        /// </summary>
        public void CleanupJSL()
        {
            Debug.Log("Cleaning up JSL");
            // Clean up JSL for all managed players as well.
            foreach(var device in players)
            {
                if (device != null)
                {
                    device.Unsync();
                }
            }
            // If you don't dispose JSL data, it may cause a memory leak.
            JSL.JslDisconnectAndDisposeAll();
        }

        /// <summary>
        /// Cancels input pairing.
        /// </summary>
        public void CancelPairing()
        {
            if (pairToken != null)
            {
                pairToken.Cancel();
            }
            else
            {
                Debug.Log("Could not cancel pairing as no pair token exists.");
            }
        }

        /// <summary>
        /// Attempt to treat JSL.JslConnectDevices as async so there can be an async managed loading screen.
        /// </summary>
        /// <returns></returns>
        private Task<int> JslConnectDevicesAsync()
        {
            Task<int> task = Task.Run(() => JSL.JslConnectDevices());
            Debug.Log("Task JslConnectDevicesAsync has finished.");
            return task;
        }

        /// <summary>
        /// Pair a controller to all players.
        /// </summary>
        public async Task PairControllersAsyncGyro(CancellationToken ct)
        {
            void OnCanceled()
            {
                // Always claenup JSL if the operation is canceled as OnDestroy may be called too early.
                Debug.Log("Operation PairControllers was canceled");
                CleanupJSL();
            }

            Time.timeScale = 0f;

            // Remove any previous pairing.
            foreach (var controller in players)
            {
                if (controller != null)
                {
                    controller.Unsync();
                }
            }

            // Add delay for testing.
            //await Task.Delay(2000);

            // Load the JSL and InputSystem controllers.

            OnPairingBegin?.Invoke();
            int jslNumConnected = await JslConnectDevicesAsync();
            InputDevice[] inputDevices = InputSystem.devices.ToArray();

            Debug.Log($"Connected {jslNumConnected} to JSL.");

            // If we've already been canceled, call cancel cleanup.
            if (ct.IsCancellationRequested)
            {
                OnCanceled();
            }
            // If cancelled, skip proceeding with pairing.
            try
            {
                // Sequentially pair each controller and update UI.
                foreach (var controller in players)
                {
                    Debug.Log("Pairing Controllers " + controller.name);

                    // Pair the controller.
                    OnPlayerPairing?.Invoke("Pairing " + controller.name);
                    await controller.PairControllerGyro(jslNumConnected, inputDevices, ct);

                    // If the operation was canceled halfway through, throw the exception.
                    ct.ThrowIfCancellationRequested();

                    // Configure the controller's calibration.
                    OnPlayerCalibrating?.Invoke("Calibrating " + controller.name);
                    await controller.CalibrateGyro(ct);

                    // Add an additional buffer delay between pairing each controller.
                    await Task.Delay(pairDelay);
                }
                Debug.Log("Task PairControllers has finished successfully.");
            }
            catch (OperationCanceledException)
            {
                OnCanceled();
            }
            finally
            {
                // Call a cleanup event.
                OnPairingEnd?.Invoke();
                Time.timeScale = 1f;
                Debug.Log("Operation PairControllers has ended.");
            }
        }
        #endregion
    }
}
