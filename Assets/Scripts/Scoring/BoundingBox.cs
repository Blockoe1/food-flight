/*****************************************************************************
// File Name : BoundingBOx.cs
// Author : Brandon Koederitz
// Creation Date : 2/2/2026
// Last Modified : 2/2/2026
//
// Brief Description :  Prevents an object from going beyond the bounds of the level.
*****************************************************************************/
using CustomAttributes;
using UnityEngine;

namespace FoodFlight
{
    public class BoundingBox : MonoBehaviour
    {
        [SerializeField] private Vector2 levelBounds;

        #region Component References
        [Header("Components")]
        [SerializeReference, ReadOnly] protected Rigidbody rb;

        [ContextMenu("Get Component References")]
        private void Reset()
        {
            rb = GetComponent<Rigidbody>();
        }
        #endregion

        private void FixedUpdate()
        {
            // Clamp the player's position to the level's bounds.
            Vector3 pos = rb.position;
            pos.x = Mathf.Clamp(pos.x, -levelBounds.x, levelBounds.x);
            pos.z = Mathf.Clamp(pos.z, -levelBounds.y, levelBounds.y);
            rb.MovePosition(pos);
        }
    }
}
