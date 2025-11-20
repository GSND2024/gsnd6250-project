using UnityEngine;

public class BulletTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Find a bullet");
            GlobalGameState.bulletCount += 1;
            Destroy(gameObject);
        }
    }
}
