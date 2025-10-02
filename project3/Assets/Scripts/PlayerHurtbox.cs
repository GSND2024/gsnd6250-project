using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class PlayerHurtbox : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerMovement2D owner;

    [Header("Boss Contact")]
    [SerializeField] private int bossContactDamage = 1;
    [SerializeField] private float bossContactCooldown = 0.35f;

    [Header("Knockback")]
    [SerializeField] private float touchKnockbackForce = 7.5f;
    [SerializeField] private float touchKnockbackDuration = 0.08f;

    private float _lastTouchTime = -999f;

    private void Reset()
    {
        // Auto-assign owner if placed under the player
        if (owner == null) owner = GetComponentInParent<PlayerMovement2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Boss")) return;
        if (Time.time - _lastTouchTime < bossContactCooldown) return;
        _lastTouchTime = Time.time;

        if (owner != null && bossContactDamage > 0)
        {
            owner.TakeDamage_Public(bossContactDamage);

            // Knock the player away from the boss
            Vector2 dir = ((Vector2)owner.transform.position - (Vector2)other.bounds.center).normalized;
            if (dir.sqrMagnitude < 0.0001f) dir = Random.insideUnitCircle.normalized;
            owner.ApplyKnockback(dir, touchKnockbackForce, touchKnockbackDuration);
        }
    }
}
