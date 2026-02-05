/*****************************************************************************
// File Name : ButtonHoverDetector.cs
// Author : Brandon Koederitz
// Creation Date : 2/5/2026
// Last Modified : 2/5/2026
//
// Brief Description :  Triggers a unity event when this button is selected or hovered over.
*****************************************************************************/
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace FoodFlight
{
    public class ButtonHoverDetector : MonoBehaviour, ISelectHandler, IPointerEnterHandler
    {
        [SerializeField] private UnityEvent OnSelectEvent;

        [field: SerializeField] public bool IgnoreNext { get; set; }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IgnoreNext)
            {
                IgnoreNext = false;
                return;
            }
            OnSelectEvent?.Invoke();
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (IgnoreNext)
            {
                IgnoreNext = false;
                return;
            }
            OnSelectEvent?.Invoke();
        }
    }
}
