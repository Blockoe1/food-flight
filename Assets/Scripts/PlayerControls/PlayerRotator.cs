/*****************************************************************************
// File Name : PlayerRotator.cs
// Author : Brandon Koederitz
// Creation Date : 1/27/2026
// Last Modified : 1/27/2026
//
// Brief Description :  Controls rotating the player based on Gyroscope/Stick Input.
*****************************************************************************/
using UnityEngine;

namespace FoodFlight
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerRotator : MonoBehaviour
    {
        #region Component References
        [SerializeReference] private Rigidbody rb;

        [ContextMenu("Get Component References")]
        private void Reset()
        {
            rb = GetComponent<Rigidbody>();
        }

        #endregion

        /// <summary>
        /// Resets this player back to their default rotation.
        /// </summary>
        public void ResetRotation()
        {
            rb.rotation = Quaternion.identity;
        }

        protected void SetRotation(Quaternion rotation)
        {
            rb.rotation = rotation;
        }
    }
}
