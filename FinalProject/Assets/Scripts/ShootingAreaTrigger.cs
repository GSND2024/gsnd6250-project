using UnityEngine;
using UnityEngine.SceneManagement; 
public class ShootingAreaTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(GlobalGameState.bulletCount);
            if (GlobalGameState.bulletCount >= 6)
            {
                SceneManager.LoadScene("ShootingArea");
            }
        }
    }
}
