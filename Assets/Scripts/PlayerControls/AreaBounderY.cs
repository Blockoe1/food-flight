/*****************************************************************************
// File Name : AreaBounder.cs
// Author : Brandon Koederitz
// Creation Date : 2/5/2026
// Last Modified : 2/5/2026
//
// Brief Description :  Binds all objects with an area bounder to a certain limit so they stay close together.
*****************************************************************************/
using CustomAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace FoodFlight
{
    public class AreaBounderY : MonoBehaviour
    {
        [SerializeField] private float minDistance;
        [SerializeField] private float maxDistance;
        [SerializeField] private float maxCorrectionForce;
        [SerializeField] private Rigidbody[] boundedBodies;
        

        /// <summary>
        /// Applies a force to this rigidbody that pushes it to stay within the area's bounds.
        /// </summary>
        private void FixedUpdate()
        {
            // Calculate the average Y position of all the bounded bodies.
            float avgYPos = CalculateAvgY(boundedBodies);

            Debug.DrawLine(new Vector3(0, avgYPos, 0), new Vector3(0, avgYPos + 2, 0), Color.green);

            // Apply force to each bounded body based on how far away they are from the avg (middle) y pos.
            foreach (var boundedBody in boundedBodies)
            {
                ProcessBody(boundedBody, avgYPos);
            }
        }

        /// <summary>
        /// Applies force to a rigidbody based on it's distance from the middle Y.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="avgY"></param>
        private void ProcessBody(Rigidbody body, float avgY)
        {
            float dist = body.position.y - avgY;
            float distDir = System.MathF.Sign(dist);
            dist = Mathf.Abs(dist);

            // Bodies beyond the max distance get clamped to enforce a hard limit.
            if (dist > maxDistance)
            {
                // Clamp the player's position to the level's bounds.
                Vector3 pos = body.position;
                pos.y = Mathf.Clamp(pos.y, avgY - maxDistance, avgY + maxDistance);
                body.MovePosition(pos);
            }

            // Bodies between min and max recieve a force based on how far along they are.
            if (dist > minDistance)
            {
                float boundingDist = dist - minDistance;
                float boundingMargin = maxDistance - minDistance;
                float normalizedBounding = boundingDist / boundingMargin;

                float forceStrenght = Mathf.Lerp(0, maxCorrectionForce, normalizedBounding);

                body.AddForce(forceStrenght * distDir * Vector3.down);
                Debug.DrawLine(body.position, body.position + (forceStrenght * distDir * Vector3.down));
            }

            // Bodies within the min distance are not affected.
        }

        /// <summary>
        /// Calculates the average y position of a collection of rigidbodies.
        /// </summary>
        /// <param name="boundedBodies"></param>
        /// <returns></returns>
        private static float CalculateAvgY(Rigidbody[] boundedBodies)
        {
            float totalY = 0;
            int bodyCount = 0;
            foreach(var b in boundedBodies)
            {
                // Ignore the cheeseburger when it's disabled.
                if (b.gameObject.activeSelf)
                {
                    totalY += b.position.y;
                    bodyCount++;
                }
            }
            return totalY / bodyCount;
        }
    }
}
