using System;
using UnityEngine;

[DefaultExecutionOrder(-100)] // init early
public class ChestManager : MonoBehaviour
{
    public static ChestManager Instance { get; private set; }

    [Tooltip("How many chests required to open gates.")]
    public int requiredChests = 3;

    public int Collected { get; private set; } = 0;
    public event Action<int> OnChestCountChanged;

    void Awake()
    {
        if (Instance && Instance != this)
        {
            Debug.LogWarning($"[ChestManager] Duplicate found on {name}. Destroying this one.");
            Destroy(gameObject); // keep the original
            return;
        }

        Instance = this;
        // Optional: uncomment if you truly want it persistent across scenes.
        // DontDestroyOnLoad(gameObject);

        Debug.Log($"[ChestManager] Ready on {name}. requiredChests={requiredChests}");
    }

    public void AddChest()
    {
        Collected++;
        Debug.Log($"[ChestManager] Count -> {Collected}");
        OnChestCountChanged?.Invoke(Collected);
    }
}
