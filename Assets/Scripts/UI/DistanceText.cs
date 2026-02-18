/*****************************************************************************
// File Name : DistanceText.cs
// Author : Brandon Koederitz
// Creation Date : 2/16/2026
// Last Modified : 2/16/2026
//
// Brief Description :  Changes text to show the distance between two objects.
*****************************************************************************/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFlight
{
    public class DistanceText : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private Transform obj1;
        [SerializeField] private Transform obj2;
        [SerializeField] private Image heighIcon;

        private void LateUpdate()
        {
            text.text = Mathf.RoundToInt(Vector3.Distance(obj1.position, obj2.position)) + "m";
            if (obj1.position.y > obj2.position.y)
            {
                heighIcon.transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                heighIcon.transform.eulerAngles = new Vector3(0, 0, 180);
            }
        }
    }
}
