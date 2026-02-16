/*****************************************************************************
// File Name : Clamped Icon.cs
// Author : Brandon Koederitz
// Creation Date : 2/3/2026
// Last Modified : 2/3/2026
//
// Brief Description :  Causes a UI object to track an object's position on the screen, clamping within certain bounds.
*****************************************************************************/
using UnityEngine;
using UnityEngine.UI;

namespace FoodFlight.UI
{
    public class ClampedIcon : MonoBehaviour
    {
        [SerializeField] private Transform trackedObject;
        [SerializeField] private Camera targetCamera;
        [SerializeField] private Image hiddenImage;
        [SerializeField] private Vector3 baseOffset;
        [SerializeField] private Vector2 screenOffset;
        [SerializeField] private float margin;

        private RectTransform rectTransform => (RectTransform)transform;

        private void LateUpdate()
        {
            Vector2 realOriginPos = targetCamera.WorldToScreenPoint(trackedObject.position + baseOffset);
            Vector2 spacing = rectTransform.sizeDelta + (Vector2.one * margin);

            // The actual base position that this predictor will be centered around after it has been clamped to
            // be within the bounds of the canvas.
            float clampedX = Mathf.Clamp(realOriginPos.x, (Screen.width * targetCamera.rect.x) + spacing.x,
                (Screen.width * targetCamera.rect.x) + (Screen.width * targetCamera.rect.width) - spacing.x);
            float clampedY = Mathf.Clamp(realOriginPos.y, (Screen.height * targetCamera.rect.y) + spacing.y,
                (Screen.height * targetCamera.rect.y) + (Screen.height * targetCamera.rect.height) - spacing.y);
            Vector2 displayOriginPos = new Vector2(clampedX, clampedY);

            // If at least one clamp was applied, show the image.  Else, hide.

            rectTransform.position = displayOriginPos;
        }
    }
}
