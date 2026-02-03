using UnityEngine.SceneManagement;
using UnityEngine;

namespace FoodFlight
{
    public class MenuOptions : MonoBehaviour
    {
        public void Play()
        {
            SceneManager.LoadScene(1);
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
