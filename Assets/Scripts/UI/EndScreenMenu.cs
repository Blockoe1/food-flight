using UnityEngine;
using UnityEngine.SceneManagement;

namespace FoodFlight
{
    public class EndScreenMenu : MonoBehaviour
    {
        public void ReturnToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
