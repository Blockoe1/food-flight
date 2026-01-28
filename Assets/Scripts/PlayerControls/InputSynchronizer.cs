/*****************************************************************************
// File Name : InputSynchronizer.cs
// Author : Brandon Koederitz
// Creation Date : 1/26/2026
// Last Modified : 1/26/2026
//
// Brief Description :  Synchronizes input between the unity InputSystem and the JoyShock library.
*****************************************************************************/
using CustomAttributes;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FoodFlight
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputSynchronizer : MonoBehaviour
    {
        #region Consts
        private const int PAIR_BUTTON_MASK = 0x01000; // The south button.
        private const string PAIR_CONTROL_PATH = "/buttonSouth";
        #endregion

        //public event Action<Vector3> OnGyroUpdate;

        private InputSyncState sync;

        #region Component References
        [Header("Components")]
        [SerializeReference, ReadOnly] private PlayerInput playerInput;

        /// <summary>
        /// Get components on reset.
        /// </summary>
        [ContextMenu("Get Component References")]
        protected virtual void Reset()
        {
            playerInput = GetComponent<PlayerInput>();
        }
        #endregion

        #region Properties
        public bool CanRead => sync != null;
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

        #region Getting Input
        /// <summary>
        /// Gets the accumulated Gyro input from the paired controller since the last time this function was run.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetAccumulatedGyro()
        {
            if (!CanRead) { return Vector3.zero; }
            JSL.JslGetAndFlushAccumulatedGyro(sync.jslIndex, out float x, out float y, out float z);
            return new Vector3(-x, -y, z);
        }
        #endregion

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
            bool IsButtonPressed(int index)
            {
                var state = JSL.JslGetSimpleState(index);
                return (state.buttons & PAIR_BUTTON_MASK) == PAIR_BUTTON_MASK;
            }

            async Task WaitUntilButtonReleased(int foundIndex)
            {
                while (!ct.IsCancellationRequested && IsButtonPressed(foundIndex))
                {
                    await Task.Yield();
                }
            }

            while (!ct.IsCancellationRequested)
            {
                // Loop through each JSL controller
                Debug.Log("Awaiting JSL Input.");
                for(int i = 0; i < numControllers; i++)
                {
                    if (IsButtonPressed(i))
                    {
                        Debug.Log("Found JSL Device: " + i);
                        // Don't return the found device until the button is released.
                        await WaitUntilButtonReleased(i);
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
            async Task WaitUntilButtonReleased(Gamepad foundGamepad)
            {
                while (!ct.IsCancellationRequested && foundGamepad.GetChildControl("/buttonSouth").IsPressed())
                {
                    await Task.Yield();
                }
            }

            while (!ct.IsCancellationRequested)
            {
                Debug.Log("Awaiting InputSystem Input");
                foreach (InputDevice device in devices)
                {
                    // Check if the device's south button is pressed.  If it is, return this controller.
                    if (device is Gamepad foundGamepad && device.GetChildControl("/buttonSouth").IsPressed())
                    {
                        Debug.Log("Found InputSystem Device: " + device);
                        // Don't return the found device until the button is released.
                        await WaitUntilButtonReleased(foundGamepad);
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