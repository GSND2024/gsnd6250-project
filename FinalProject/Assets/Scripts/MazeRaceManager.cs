using UnityEngine;
using TMPro;

public class MazeRaceManager : MonoBehaviour
{
    [Header("Rat / Race Setup")]
    public GameObject ratObject;
    public RatController ratController;
    public Transform ratStartPoint;

    [Header("Player")]
    public GameObject playerRoot;                // optional, kept for reference
    public MonoBehaviour[] playerControlScripts; // movement / look scripts to disable
    public Camera playerCamera;
    public Camera ratCamera;

    [Header("Timer")]
    public float raceDurationSeconds = 30f;
    public TextMeshProUGUI timerText;

    [Header("UI")]
    public GameObject losePanel;   // optional
    public GameObject winPanel;    // optional

    private float timeRemaining;
    private bool raceActive = false;

    private void Start()
    {
        // Ensure starting state
        //if (ratObject != null) ratObject.SetActive(false);
        if (ratCamera != null) ratCamera.gameObject.SetActive(false);
        if (playerCamera != null) playerCamera.gameObject.SetActive(true);

        if (timerText != null) timerText.gameObject.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
    }

    private void Update()
    {
        if (!raceActive) return;

        // Countdown
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            UpdateTimerUI();
            LoseRace();
        }
        else
        {
            UpdateTimerUI();
        }

        // Escape to cancel race (optional)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndRace(); // treat as cancel
        }
    }

    // Called by MazeRaceActivator when player presses Space near the table
    public void StartRace()
    {
        if (raceActive) return;
        raceActive = true;

        // Put rat at start
        if (ratObject != null && ratStartPoint != null)
        {
            ratObject.transform.position = ratStartPoint.position;
            ratObject.transform.rotation = ratStartPoint.rotation;
        }

        if (ratObject != null) ratObject.SetActive(true);
        if (ratController != null) ratController.SetCanMove(true);

        // Disable player movement
        foreach (var script in playerControlScripts)
        {
            if (script != null) script.enabled = false;
        }

        // Switch cameras
        if (playerCamera != null) playerCamera.gameObject.SetActive(false);
        if (ratCamera != null) ratCamera.gameObject.SetActive(true);

        // Timer UI
        timeRemaining = raceDurationSeconds;
        if (timerText != null) timerText.gameObject.SetActive(true);
        UpdateTimerUI();

        if (losePanel != null) losePanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
    }

    private void LoseRace()
    {
        if (!raceActive) return;
        raceActive = false;

        if (losePanel != null) losePanel.SetActive(true);

        FinishRaceCommon();
    }

    // Called by MazeWinTrigger when rat enters win zone
    public void WinRace()
    {
        if (!raceActive) return;
        raceActive = false;

        if (winPanel != null) winPanel.SetActive(true);

        GlobalGameState.haveCash = true;

        FinishRaceCommon();
    }

    // Also used by Escape cancel
    public void EndRace()
    {
        if (!raceActive) return;
        raceActive = false;

        FinishRaceCommon();
    }

    private void FinishRaceCommon()
    {
        // Stop rat
        if (ratController != null) ratController.SetCanMove(false);
        if (ratObject != null) ratObject.SetActive(false);

        // Re-enable player controls
        foreach (var script in playerControlScripts)
        {
            if (script != null) script.enabled = true;
        }

        // Switch cameras back
        if (playerCamera != null) playerCamera.gameObject.SetActive(true);
        if (ratCamera != null) ratCamera.gameObject.SetActive(false);

        // Hide timer
        if (timerText != null) timerText.gameObject.SetActive(false);
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;
        timerText.text = Mathf.CeilToInt(timeRemaining).ToString();
    }
}
