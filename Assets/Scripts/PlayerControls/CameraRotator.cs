/*****************************************************************************
// File Name : CameraRotator.cs
// Author : Brandon Koederitz
// Creation Date : 2/2/2026
// Last Modified : 2/2/2026
//
// Brief Description :  Rotates the camera to face in the forward direction of the player.
*****************************************************************************/
using CustomAttributes;
using Unity.Cinemachine;
using UnityEngine;

namespace FoodFlight
{
    public class CameraRotator : MonoBehaviour
    {
        [SerializeField] private float boomDistance = 2;

        #region Component References
        [Header("Components")]
        [SerializeReference, ReadOnly] private CinemachineCamera cam;
        [SerializeReference, ReadOnly] private CinemachineFollow follow;

        [ContextMenu("Get Component References")]
        private void Reset()
        {
            cam = GetComponent<CinemachineCamera>();
            follow = GetComponent<CinemachineFollow>();
        }
        #endregion

        private void LateUpdate()
        {
            Vector3 forward = (cam.Target.TrackingTarget.rotation * Vector3.down);
            Vector2 rotVector = new Vector2(forward.x, forward.z) * -boomDistance;
            // Set the offset of the CinemachineFollow based on the forward direction.
            follow.FollowOffset = new Vector3(rotVector.x, follow.FollowOffset.y, rotVector.y);
        }
    }
}
