using UnityEngine;

namespace FoodFlight
{
    public class BurgerSurfingToggle : MonoBehaviour
    {
        public void ToggleBurgerSurfing()
        {
            BurgerHolder.CanBurgerSurf = !BurgerHolder.CanBurgerSurf;
        }
    }
}
