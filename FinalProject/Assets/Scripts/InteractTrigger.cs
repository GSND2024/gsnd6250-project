using UnityEngine;
using TMPro;

public class InteractTrigger : MonoBehaviour
{
    public float interactDistance = 3f;
    
    public Dialogue dialogue;
    [SerializeField] private DialogueManager dialogueManager;
    
    public TextMeshProUGUI promptText; 

    private Transform player;
    private bool playerInRange = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (promptText != null)
            promptText.gameObject.SetActive(false); 
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactDistance)
        {
            if (!playerInRange)
            {
                playerInRange = true;
                ShowPrompt();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                TriggerInteraction();
            }
        }
        else
        {
            if (playerInRange)
            {
                playerInRange = false;
                HidePrompt();
            }
        }
    }

    void ShowPrompt()
    {
        if (promptText != null)
        {
            promptText.text = "Press SPACE";
            promptText.gameObject.SetActive(true);
        }
    }

    void HidePrompt()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    void TriggerInteraction()
    {
        Debug.Log("Interaction triggered!");
        
        dialogueManager.StartDialogue(dialogue);

    }
}