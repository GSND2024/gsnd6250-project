using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GateController : MonoBehaviour
{
    [Tooltip("Override global requirement? Leave 0 to use ChestManager.requiredChests.")]
    public int requiredChestsOverride = 0;

    [Header("Open Behavior")]
    public bool disableCollider = true;
    public bool hideSpriteOnOpen = true;
    public float fadeTime = 0.25f;

    Collider2D _collider;
    SpriteRenderer _sr;
    bool _opened;
    bool _subscribed;

    void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _sr = GetComponentInChildren<SpriteRenderer>();
    }

    void OnEnable()
    {
        TrySubscribe();
    }

    void Start()
    {
        // If manager existed late, we still check immediately.
        if (ChestManager.Instance != null)
        {
            Debug.Log($"[Gate] Start check. Count={ChestManager.Instance.Collected}");
            CheckAndOpen(ChestManager.Instance.Collected);
        }
        else
        {
            // Keep trying until the manager appears.
            StartCoroutine(WaitForManagerThenSubscribe());
        }
    }

    void OnDisable()
    {
        if (_subscribed && ChestManager.Instance != null)
        {
            ChestManager.Instance.OnChestCountChanged -= OnChestCountChanged;
            _subscribed = false;
        }
    }

    System.Collections.IEnumerator WaitForManagerThenSubscribe()
    {
        while (ChestManager.Instance == null) yield return null;
        TrySubscribe();
        Debug.Log($"[Gate] Late bind to ChestManager. Current count={ChestManager.Instance.Collected}");
        CheckAndOpen(ChestManager.Instance.Collected);
    }

    void TrySubscribe()
    {
        if (ChestManager.Instance != null && !_subscribed)
        {
            ChestManager.Instance.OnChestCountChanged += OnChestCountChanged;
            _subscribed = true;
        }
    }

    void OnChestCountChanged(int newCount)
    {
        Debug.Log($"[Gate] Chest count changed → {newCount}");
        CheckAndOpen(newCount);
    }

    void CheckAndOpen(int count)
    {
        if (_opened) return;

        int needed = requiredChestsOverride > 0
            ? requiredChestsOverride
            : (ChestManager.Instance ? ChestManager.Instance.requiredChests : 3);

        if (count >= needed)
        {
            Debug.Log($"[Gate] Opening. Needed={needed}, Count={count}");
            OpenGate();
        }
        else
        {
            Debug.Log($"[Gate] Not enough. Needed={needed}, Count={count}");
        }
    }

    void OpenGate()
    {
        _opened = true;

        if (disableCollider && _collider) _collider.enabled = false;

        if (_sr && hideSpriteOnOpen)
        {
            StartCoroutine(FadeOut());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    System.Collections.IEnumerator FadeOut()
    {
        float t = 0f;
        Color c = _sr ? _sr.color : Color.white;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            if (_sr)
            {
                float a = Mathf.Lerp(1f, 0f, t / fadeTime);
                _sr.color = new Color(c.r, c.g, c.b, a);
            }
            yield return null;
        }
        gameObject.SetActive(false);
    }

    // Right-click on the component header → “Open Now” for quick testing in Play Mode.
    [ContextMenu("Open Now")]
    void ContextOpenNow() => OpenGate();
}
