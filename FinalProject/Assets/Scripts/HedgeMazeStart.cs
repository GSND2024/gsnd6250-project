using UnityEngine;

public class HedgeMazeStart : MonoBehaviour
{
    public Dialogue dialogue;
    public DialogueManager dialogueManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueManager.StartDialogue(dialogue);
    }
    
}
