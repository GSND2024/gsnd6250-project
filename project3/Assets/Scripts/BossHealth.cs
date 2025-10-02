using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class BossHealth : MonoBehaviour
{
    [Header("HP (Hits To Kill)")]
    [Tooltip("How many hits this boss can take before dying.")]
    public int maxHits = 8;

    [Tooltip("Optional: damage per player hit (usually 1).")]
    public int damagePerHit = 1;

    [Header("UI")]
    [Tooltip("World-space slider showing remaining HP. Optional but recommended.")]
    public Slider hpSlider;
    [Tooltip("Hide the bar when boss is at full HP.")]
    public bool hideWhenFull = true;

    [Header("Feedback")]
    public float hitFlashTime = 0.07f;
    public Color hitFlashColor = new Color(1f, 0.5f, 0.5f, 1f);

    public event System.Action<BossHealth> OnDied;

    private int currentHits; // how many hits taken so far
    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;
    private bool dead;

    void Awake()
    {
        currentHits = 0;
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        originalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
            originalColors[i] = spriteRenderers[i].color;

        if (hpSlider != null)
        {
            hpSlider.minValue = 0;
            hpSlider.maxValue = maxHits;
            hpSlider.wholeNumbers = true;
            hpSlider.value = maxHits;
            if (hideWhenFull) hpSlider.gameObject.SetActive(false);
        }
    }

    /// <summary>Call this when the player’s attack connects.</summary>
    public void ApplyHit(int amount = -1)
    {
        if (dead) return;

        // amount<0 means use damagePerHit (1 by default)
        int dmg = (amount < 0) ? damagePerHit : amount;
        currentHits += Mathf.Max(1, dmg); // counting hits taken

        // UI update
        if (hpSlider != null)
        {
            int remaining = Mathf.Max(0, maxHits - currentHits);
            hpSlider.value = remaining;
            if (hideWhenFull && remaining < maxHits && !hpSlider.gameObject.activeSelf)
                hpSlider.gameObject.SetActive(true);
        }

        // small flash
        if (hitFlashTime > 0f) StartCoroutine(HitFlash());

        // death check
        if (currentHits >= maxHits)
        {
            Die();
        }
    }

    private System.Collections.IEnumerator HitFlash()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
            spriteRenderers[i].color = hitFlashColor;

        yield return new WaitForSeconds(hitFlashTime);

        for (int i = 0; i < spriteRenderers.Length; i++)
            spriteRenderers[i].color = originalColors[i];
    }

    private void Die()
    {
        dead = true;
        OnDied?.Invoke(this);

        // Optional: drop loot, play SFX/VFX, etc. before despawn.
        // For now, just despawn.
        Destroy(gameObject);
    }
}
