using UnityEngine.SceneManagement;
using UnityEngine;

namespace FoodFlight
{
    public class MenuOptions : MonoBehaviour
    {
        public GameObject MainSection;
        public GameObject Controls;

        public void Play()
        {
            SceneManager.LoadScene(1);
        }

        public void Quit()
        {
            Application.Quit();
        }


        //switches shit
        public void DoSmthn(int yep)
        {
            InputPairingManager.SetControlScheme( (ControlScheme)yep );
        }

        public void OpenControls()
        {
            Controls.SetActive(true);
        }
        public void CloseControls()
        {
            Controls.SetActive(false);
        }
    }
}
