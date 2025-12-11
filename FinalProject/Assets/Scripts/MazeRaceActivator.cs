using UnityEngine;

public class MazeRaceActivator : MonoBehaviour
{
    public MazeRaceManager raceManager;
    public string playerTag = "Player";

    private bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            //prompt here
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            raceManager.StartRace();
        }
    }
}
