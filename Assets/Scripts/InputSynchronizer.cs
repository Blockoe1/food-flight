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

namespace FoodFlight
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputSynchronizer : MonoBehaviour
    {


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

        private async Task<int> GetJoyShock(int numControllers, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        private async Task<InputDevice> GetInputDevice(InputDevice[] devices, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}