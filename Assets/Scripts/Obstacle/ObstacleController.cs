using UnityEngine;

namespace FoodFlight
{
    public class ObstacleController : MonoBehaviour
    {
        public bool isContacted = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                gameObject.SetActive(false);
                isContacted = true;


            }
        }
    }
}
