/*****************************************************************************
// File Name : InputSynchronizer.cs
// Author : Brandon Koederitz
// Creation Date : 1/26/2026
// Last Modified : 1/26/2026
//
// Brief Description :  Synchronizes input between the unity InputSystem and the JoyShock library.
*****************************************************************************/
using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;

namespace FoodFlight
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputSynchronizer : MonoBehaviour
    {
        #region Consts
        private const int PAIR_BUTTON_MASK = 0x01000; // The south button.
        private const string PAIR_CONTROL_PATH = "/buttonSouth";
        #endregion

        public event Action<Vector3> OnGyroUpdate;

        private InputSyncState sync;

        #region Component References
        [Header("Components")]
        [SerializeReference] private PlayerInput playerInput;

        /// <summary>
        /// Get components on reset.
        /// </summary>
        [ContextMenu("Get Component References")]
        protected virtual void Reset()
        {
            playerInput = GetComponent<PlayerInput>();
        }
        #endregion

        #region Nested
        private class InputSyncState
        {
            internal readonly int jslIndex;
            internal readonly InputDevice inputDevice;

            internal InputSyncState(int jslIndex, InputDevice inputDevice)
            {
                this.inputDevice = inputDevice;
                this.jslIndex = jslIndex;
            }
        }
        #endregion

        /// <summary>
        /// Read Gyro controls on Update as it needs to be continuous.
        /// </summary>
        private void Update()
        {
            if (sync != null)
            {
                var state = JSL.JslGetIMUState(sync.jslIndex);
                Vector3 gyro = new Vector3(-state.gyroX, -state.gyroY, state.gyroZ);
                OnGyroUpdate?.Invoke(gyro);
            }
        }

        /// <summary>
        /// Sets the SyncState of this synchronizer.
        /// </summary>
        /// <param name="jslIndex">The index of the controller in JSL.</param>
        /// <param name="inputDevice">The InputDevice used through the Unity InputSystem.</param>
        private void SetSyncState(int jslIndex, InputDevice inputDevice)
        {
            sync = new InputSyncState(jslIndex, inputDevice);
            playerInput.SwitchCurrentControlScheme(inputDevice);
        }

        /// <summary>
        /// Removes syncronization data from this InputSyncronizer.
        /// </summary>
        public void Unsync()
        {
            sync = null;
        }

        #region Pairing
        /// <summary>
        /// Pair a controller for 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task PairControllers(int numControllers, InputDevice[] inputDevices, CancellationToken ct)
        {
            // Create tasks for syncing the JoyShock and InputDevices.
            Task<int> joyShockTask = GetJoyShock(numControllers, ct);
            Task<InputDevice> inputDeviceTask = GetInputDevice(inputDevices, ct);

            // Create a task array to await tasks syncronously.
            Task[] tasks = new Task[2];
            tasks[0] = joyShockTask;
            tasks[1] = inputDeviceTask;
            await Task.WhenAll(tasks);

            int joyShockIndex = joyShockTask.GetAwaiter().GetResult();
            InputDevice inputDevice = inputDeviceTask.GetAwaiter().GetResult();

            Debug.Log($"Paired the JSL index {joyShockIndex} to the InputDevice {inputDevice}");

            SetSyncState(joyShockIndex, inputDevice);
        }

        /// <summary>
        /// Awaits a JoyShock input to identify a controller to pair.
        /// </summary>
        /// <param name="numControllers">The total number of controllers connected to JSL.</param>
        /// <param name="ct">The CancellationToken for this operation.</param>
        /// <returns>The index of the found JSL device.</returns>
        private async Task<int> GetJoyShock(int numControllers, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                // Loop through each JSL controller
                Debug.Log("Awaiting JSL Input.");
                for(int i = 0; i < numControllers; i++)
                {
                    var state = JSL.JslGetSimpleState(i);
                    if ((state.buttons & PAIR_BUTTON_MASK) == PAIR_BUTTON_MASK)
                    {
                        Debug.Log("Found JSL Device: " + i);
                        return i;
                    }

                }
                await Task.Yield();
            }
            Debug.Log("Pairing of JoyShock controller cancelled.");
            ct.ThrowIfCancellationRequested();
            return -1;
        }

        /// <summary>
        /// Awaits an InputSystem InputDevice input to identify a controller to pair.
        /// </summary>
        /// <param name="devices">The array of connected InputDevices.</param>
        /// <param name="ct">The CancellationToken for this operation.</param>
        /// <returns>The found InputDevice.</returns>
        private async Task<InputDevice> GetInputDevice(InputDevice[] devices, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                Debug.Log("Awaiting InputSystem Input");
                foreach (InputDevice device in devices)
                {
                    // Check if the device's south button is pressed.  If it is, return this controller.
                    if (device is Gamepad && device.GetChildControl("/buttonSouth").IsPressed())
                    {
                        Debug.Log("Found InputSystem Device: " + device);
                        return device;
                    }
                }
                await Task.Yield();
            }
            Debug.Log("Pairing of InputDevice controller cancelled.");
            ct.ThrowIfCancellationRequested();
            return null;
        }
        #endregion

        #region Calibration
        #endregion
    }
}