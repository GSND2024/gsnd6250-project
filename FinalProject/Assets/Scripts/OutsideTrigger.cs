using UnityEngine;
using UnityEngine.SceneManagement; 
public class OutsideTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GlobalGameState.cardCount >= 3)
            {
                SceneManager.LoadScene("HedgeMaze");
            }
        }
    }
}
