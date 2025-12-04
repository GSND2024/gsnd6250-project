using UnityEngine;
using UnityEngine.SceneManagement; 
public class OutsideTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GlobalGameState.readyToGoOutside)
            {
                SceneManager.LoadScene("HedgeMaze");
            }
        }
    }
}
