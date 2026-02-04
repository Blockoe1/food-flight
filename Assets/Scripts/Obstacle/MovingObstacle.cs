using UnityEngine;

namespace FoodFlight
{
    public class MovingObstacle : MonoBehaviour
    {
        [SerializeField] private int speed;

        void FixedUpdate()
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }
    }
}
