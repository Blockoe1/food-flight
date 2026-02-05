/*****************************************************************************
// File Name : DistanceMeter2.cs
// Author : Brandon Koederitz
// Creation Date : 2/3/2026
// Last Modified : 2/3/2026
//
// Brief Description :  Sorry micah this is more efficient.
*****************************************************************************/
using UnityEngine;
using UnityEngine.UI;

namespace FoodFlight.UI
{
    public class DistanceMeter2 : MonoBehaviour
    {
        [SerializeField] private Transform trackedObject;
        [SerializeField] private Slider slider;
        [SerializeField] private float top;
        [SerializeField] private float bottom;

        /// <summary>
        /// Update the slider value to match the progress of the object towards the ground.
        /// </summary>
        private void LateUpdate()
        {
            float relPos = trackedObject.position.y - bottom;
            slider.value = relPos / (top - bottom);
        }
    }
}
