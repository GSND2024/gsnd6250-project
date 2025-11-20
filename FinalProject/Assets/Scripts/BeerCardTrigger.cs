using UnityEngine;

public class BeerCardTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GlobalGameState.haveBeer)
            {
                Debug.Log("Find a card");
                GlobalGameState.haveBeer = false;
                GlobalGameState.cardCount += 1;
                Destroy(gameObject);
            }
            else
                Debug.Log("Need beer");
        }
    }
}
