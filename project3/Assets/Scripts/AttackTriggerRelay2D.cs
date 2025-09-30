using UnityEngine;

/// Attach this to each attack object (J/K/L/I).
/// Requires a Collider2D set to "Is Trigger".
/// When it hits something tagged "Boss", it forwards the event to the player script.
[RequireComponent(typeof(Collider2D))]
public class AttackTriggerRelay2D : MonoBehaviour
{
    public PlayerMovement2D owner;
    
    public string key = "J";

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true; // ensure trigger for overlap events
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only forward if configured
        if (owner != null && other != null)
        {
            if (other.CompareTag("Boss"))
            {
                owner.OnAttackHit(key, other);
            }
        }
    }
}

