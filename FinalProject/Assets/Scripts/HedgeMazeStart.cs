using UnityEngine;
using TMPro;

public class HedgeMazeStart : MonoBehaviour
{
    public Dialogue dialogue;
    public DialogueManager dialogueManager;
    public TMP_Text bulletText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueManager.StartDialogue(dialogue);
    }

    void Update()
    {
        UpdateBulletText();
    }
    
    void UpdateBulletText()
    {
        if (bulletText != null)
            bulletText.text = "Bullets: " + GlobalGameState.bulletCount + " / 6";
    }
}
