using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BossLevelController : MonoBehaviour
{
    [Header("Boss Discovery")]
    [Tooltip("If true, automatically finds all BossHealth in the scene on Start().")]
    public bool autoFindBosses = true;

    [Tooltip("If set and autoFindBosses is false, use this explicit list.")]
    public BossHealth[] bosses;

    [Header("Win Behavior")]
    [Tooltip("Optional UI to show on victory (e.g., a 'You Win' panel).")]
    public GameObject winUI;

    [Tooltip("Optional next scene to load on victory. If empty, game pauses on win.")]
    public string nextSceneName = "";

    [Tooltip("Small delay before showing UI / loading next scene, for dramatic effect.")]
    public float delayBeforeWin = 1.0f;

    private int _remaining;

    private void Start()
    {
        if (autoFindBosses || bosses == null || bosses.Length == 0)
            bosses = FindObjectsOfType<BossHealth>(true);

        _remaining = 0;
        foreach (var b in bosses)
        {
            if (b == null) continue;
            _remaining++;
            b.OnDied += HandleBossDied;
        }

        if (_remaining == 0)
            Debug.LogWarning("[BossLevelController] No bosses found to track.");
    }

    private void OnDestroy()
    {
        // Clean up event subscriptions
        if (bosses == null) return;
        foreach (var b in bosses)
        {
            if (b == null) continue;
            b.OnDied -= HandleBossDied;
        }
    }

    private void HandleBossDied(BossHealth b)
    {
        _remaining = Mathf.Max(0, _remaining - 1);
        if (_remaining == 0)
        {
            StartCoroutine(WinSequence());
        }
    }

    private IEnumerator WinSequence()
    {
        if (winUI != null) winUI.SetActive(true);
        yield return new WaitForSeconds(delayBeforeWin);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            // No next scene provided—just pause on win UI
            Time.timeScale = 0f;
        }
    }
}
