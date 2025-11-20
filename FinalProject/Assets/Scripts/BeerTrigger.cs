using UnityEngine;

public class BeerTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GlobalGameState.haveCash)
            {
                Debug.Log("Buy Beer");
                GlobalGameState.haveBeer = true;
                Destroy(gameObject);
            }
        }
    }
}
