using UnityEngine;

namespace FoodFlight
{
    public class MovingObstacle : MonoBehaviour
    {
        [SerializeField] private int speed;
        [SerializeField] private ObstacleController obstacleController;

        void FixedUpdate()
        {
            if (obstacleController.isContacted == true)
            {
                transform.Translate(Vector3.left * speed * Time.deltaTime);
            }   
        }
    }
}
