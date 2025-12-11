using UnityEngine;

public class CardTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Rat"))
        {
            Debug.Log("Find a card");
            GlobalGameState.cardCount += 1;
            Destroy(gameObject);
        }
    }
}
