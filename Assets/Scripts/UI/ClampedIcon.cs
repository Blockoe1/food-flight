/*****************************************************************************
// File Name : Clamped Icon.cs
// Author : Brandon Koederitz
// Creation Date : 2/3/2026
// Last Modified : 2/3/2026
//
// Brief Description :  Causes a UI object to track an object's position on the screen, clamping within certain bounds.
*****************************************************************************/
using UnityEngine;

namespace FoodFlight.UI
{
    public class ClampedIcon : MonoBehaviour
    {
        [SerializeField] private Transform trackedObject;
        [SerializeField] private Camera targetCamera;
        [SerializeField] private GameObject hiddenIcon;
        [SerializeField] private Vector3 baseOffset;
        [SerializeField] private Vector2 screenOffset;
        [SerializeField] private float margin;
        [SerializeField] private float cameraDistance = 10f;

        private RectTransform rectTransform => (RectTransform)transform;

        private void LateUpdate()
        {
            if (!trackedObject.gameObject.activeInHierarchy)
            {
                hiddenIcon.SetActive(false);
                return;
            }
            else
            {
                hiddenIcon.SetActive(true);
            }

                // Clamp the tracked object's position to the camera's height.
                Vector3 worldPos = trackedObject.position + baseOffset;
            worldPos.y = Mathf.Min(worldPos.y, targetCamera.transform.position.y - cameraDistance);

            Vector2 realOriginPos = targetCamera.WorldToScreenPoint(worldPos);
            // Clamp the origin position so that it cannot be above the camera's tracked position.
            
            Vector2 spacing = rectTransform.sizeDelta + (Vector2.one * margin);

            // The actual base position that this predictor will be centered around after it has been clamped to
            // be within the bounds of the canvas.
            float clampedX = Mathf.Clamp(realOriginPos.x, (Screen.width * targetCamera.rect.x) + spacing.x,
                (Screen.width * targetCamera.rect.x) + (Screen.width * targetCamera.rect.width) - spacing.x);
            float clampedY = Mathf.Clamp(realOriginPos.y, (Screen.height * targetCamera.rect.y) + spacing.y,
                (Screen.height * targetCamera.rect.y) + (Screen.height * targetCamera.rect.height) - spacing.y);
            Vector2 displayOriginPos = new Vector2(clampedX, clampedY);

            // If at least one clamp was applied, show the image.  Else, hide.
            if (clampedX != realOriginPos.x || clampedY != realOriginPos.y)
            {
                if (hiddenIcon  != null)
                {
                    hiddenIcon.SetActive(true);
                }
            }
            else
            {
                if (hiddenIcon != null)
                {
                    hiddenIcon.SetActive(false);
                }
            }

            rectTransform.position = displayOriginPos;
        }
    }
}
