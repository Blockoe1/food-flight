using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FoodFlight
{
    public class MovementMonitor : MonoBehaviour
    {
        public GameObject Player1;
        public GameObject Player2;
        public GameObject Ground;
        public float Player1toground;
        public float Player2toground;
        public float starttoground;
        public float player1toplayer2;
        public float player2toplayer1;
        public TMP_Text GroundShowythingP1;
        public TMP_Text GroundShowythingP2;
        public TMP_Text P1P2showything;
        public TMP_Text P2P1showything;
        public GameObject P2Higher;
        public GameObject P1Higher;

        private void Start()
        {
            starttoground = Player1.transform.position.y - Ground.transform.position.y;
            Player1toground = Player1.transform.position.y - Ground.transform.position.y;
            Player2toground = Player2.transform.position.y - Ground.transform.position.y;
            player1toplayer2 = Vector3.Distance(Player1.transform.position, Player2.transform.position);
            player2toplayer1 = Vector3.Distance(Player2.transform.position, Player1.transform.position);
            GroundShowythingP1.text = (int)Player1toground + "m";
            GroundShowythingP2.text = (int)Player2toground + "m";
            //P1P2showything.text = (int)player1toplayer2 + "m";
            //P2P1showything.text = (int)player2toplayer1 + "m";
        }

        public void FixedUpdate()
        {
            //if(Player1.transform.position.y < Player2.transform.position.y)
            //{
            //    P2Higher.SetActive(true);
            //    P1Higher.SetActive(false);
            //}
            //else
            //{
            //    P1Higher.SetActive(true);
            //    P2Higher.SetActive(false);
            //}
            Player1toground = Player1.transform.position.y - Ground.transform.position.y;
            Player2toground = Player2.transform.position.y - Ground.transform.position.y;
            player1toplayer2 = Vector3.Distance(Player1.transform.position, Player2.transform.position);
            player2toplayer1 = Vector3.Distance(Player2.transform.position, Player1.transform.position);

            //P1P2showything.text = (int)player1toplayer2+"m";
            //P2P1showything.text = (int)player2toplayer1+"m";
            GroundShowythingP1.text = (int)Player1toground + "m";
            GroundShowythingP2.text = (int)Player2toground + "m";
        }
    }
}
