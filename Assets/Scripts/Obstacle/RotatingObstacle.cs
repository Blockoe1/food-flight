using UnityEngine;

namespace FoodFlight
{
    public class RotatingObstacle : MonoBehaviour
    {
        [SerializeField] private int rotationsPerSecond;

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(0, rotationsPerSecond, 0);
        }
    }
}
