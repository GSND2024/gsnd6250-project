using System;
using UnityEngine;

public class RatTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Entered Rat");
            GlobalGameState.haveCash = true;
            Destroy(gameObject);
        }
    }
}
