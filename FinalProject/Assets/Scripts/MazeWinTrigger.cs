using UnityEngine;

public class MazeWinTrigger : MonoBehaviour
{
    public MazeRaceManager raceManager;
    public string ratTag = "Rat";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ratTag))
        {
            raceManager.WinRace();
        }
    }
}
