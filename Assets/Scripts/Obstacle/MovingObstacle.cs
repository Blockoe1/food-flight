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
                if(gameObject.tag == "Bird")
                {
                    transform.Translate(Vector3.left * speed * Time.deltaTime);
                }
                else
                {
                    transform.Translate(Vector3.forward * speed * Time.deltaTime);
                }
            }   
        }
    }
}
