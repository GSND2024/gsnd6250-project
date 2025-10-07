using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Chest : MonoBehaviour
{
    [Header("FX (optional)")]
    public AudioSource pickupSfx;
    public GameObject pickupVfx;

    Collider2D _col;
    SpriteRenderer _sr;
    bool _collected;

    void Awake()
    {
        _col = GetComponent<Collider2D>();
        _col.isTrigger = true; // must be trigger
        _sr = GetComponentInChildren<SpriteRenderer>();

        // Late-bind ChestManager just in case
        if (ChestManager.Instance == null)
            Debug.LogWarning("[Chest] No ChestManager.Instance at Awake. Will try to find one on pickup.");
        Debug.Log($"[Chest] Awake on {name} | layer={gameObject.layer} | isTrigger={_col.isTrigger}");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_collected) return;

        Debug.Log($"[Chest] Trigger with {other.name} (tag={other.tag})");
        if (!other.CompareTag("Player")) return;

        // inside OnTriggerEnter2D:
        var mgr = ChestManager.Instance;
        if (mgr == null)
        {
            Debug.LogError("[Chest] No ChestManager.Instance in scene. Add one GameObject with ChestManager.");
            return;
        }
        mgr.AddChest();

        Debug.Log($"[Chest] Collected! New count = {mgr.Collected}");

        if (pickupSfx) pickupSfx.Play();
        if (pickupVfx) Instantiate(pickupVfx, transform.position, Quaternion.identity);

        if (_sr) _sr.enabled = false;
        _col.enabled = false;
        Destroy(gameObject, pickupSfx ? pickupSfx.clip.length : 0f);
    }
}
