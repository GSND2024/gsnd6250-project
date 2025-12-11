using UnityEngine;

public class CardTrigger : MonoBehaviour
{
    public AudioClip clip;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Rat"))
        {
            Debug.Log("Find a card");
            GlobalGameState.cardCount += 1;
            AudioSource.PlayClipAtPoint(clip, transform.position);
            Destroy(gameObject);
        }
        if (gameObject.name == "ratCard")
        {
            Debug.Log("rat card collected");
            GlobalGameState.ratCardFound = true;
        }
        if (gameObject.name == "behindBarCard")
        {
            GlobalGameState.behindBarFound = true;
        }
    }
}
