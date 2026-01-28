using UnityEngine;

public class BurgerScript : MonoBehaviour
{
    [SerializeField] private GameObject burger;
    [SerializeField] private int points = 0;
    

    private void OnCollisionEnter(Collision collision)
    {
        burger.gameObject.SetActive(false);
        points += 100;
        Debug.Log("Yum yum yum!");
    }
}
