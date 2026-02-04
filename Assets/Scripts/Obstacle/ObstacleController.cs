using UnityEngine;

namespace FoodFlight
{
    public class ObstacleController : MonoBehaviour
    {
        public bool isContacted = false;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                gameObject.SetActive(false);
                isContacted = true;
                
            }
        }
    }
}
