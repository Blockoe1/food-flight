using UnityEngine;
using UnityEngine.UI;

namespace FoodFlight
{
    public class Distancemeter : MonoBehaviour
    {
        //public Slider Player1;
        //public Slider Player2;
        //public Slider Objective;
        public GameObject Player1Guy;
        public GameObject Player2Guy;
        //public GameObject ObjectiveGuy;
        public GameObject Ground;
        public float Player1toground;
        public float Player2toground;
        public float ObjectiveToGround;
        public float starttoground;

        private void Start()
        {
            starttoground = Player1Guy.transform.position.y-Ground.transform.position.y;
            Player1toground = Player1Guy.transform.position.y / starttoground;
            Player2toground = Player2Guy.transform.position.y / starttoground;

            //Player1.value = Player1toground;
            //Player2.value = Player2toground;

        }

        private void FixedUpdate()
        {
            //Player1toground = Player1Guy.transform.position.y / starttoground;
            //Player2toground = Player2Guy.transform.position.y / starttoground;

            //Player1.value = Player1toground;
            //Player2.value = Player2toground;
        }
    }
}
