using UnityEngine;
using UnityEngine.SceneManagement; 
public class ShootingAreaTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public DialogueManager dialogueManager;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(GlobalGameState.bulletCount);
            if (GlobalGameState.bulletCount >= 6)
            {
                dialogue.name = "Merrit Grigg";
                dialogue.sentences = new string[]
                {
                    "Nice work! You found all the bullets.",
                    "Follow me to the shooting gallery."
                };
                dialogueManager.StartDialogue(dialogue);
                
                StartCoroutine(WaitForDialogueThenLoad());
            } else
            {
                dialogue.name = "Merrit Grigg";
                dialogue.sentences = new string[]
                {
                    "You haven't got all your bullets yet.",
                    "Keep looking, then come back here once you've found 'em all."
                };
                dialogueManager.StartDialogue(dialogue);
            }
        }
    }
    private System.Collections.IEnumerator WaitForDialogueThenLoad()
    {
        // Wait until the dialogue flag becomes false
        Debug.Log("starting coroutine");
        while (GlobalGameState.inDialogue)
        {
            Debug.Log("waiting");
            yield return null;
        }

        
        SceneLoader.LoadScene("ShootingArea");
    }
}
