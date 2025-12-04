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

        // If a dialogue is currently running, hide prompt and ignore input
        if (GlobalGameState.dialogueActive)
        {
            HidePrompt();
            return;
        }

        if (distance <= interactDistance)
        {
            if (!playerInRange)
            {
                playerInRange = true;
                ShowPrompt();
            }

            // Only start dialogue if the prompt is actually visible
            if (Input.GetKeyDown(KeyCode.Space) &&
                promptText != null &&
                promptText.gameObject.activeSelf)
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

        HidePrompt();                          // hide "Press SPACE"
        
        if (gameObject.name == "CardFindGuy" && GlobalGameState.cardCount < 4)
        {
            dialogue.name = "CardFindGuy";
            dialogue.sentences = new string[]
            {
                "I still missing some cards!"
            };
            GlobalGameState.cardFindStarted = true;
        }
        
        if (gameObject.name == "CardFindGuy" && GlobalGameState.cardCount >= 1 && GlobalGameState.readyToGoOutside == false)
        {
            dialogue.name = "CardFindGuy";
            dialogue.sentences = new string[]
            {
                "Thanks you find them all",
                "Here is my old pistol"
            };
            Debug.Log("CardFindGuy");
            
            GlobalGameState.cardFindFinished = true;
        }
        
        if (gameObject.name == "CardFindGuy" && GlobalGameState.readyToGoOutside == true)
        {
            dialogue.name = "CardFindGuy";
            dialogue.sentences = new string[]
            {
                "My old pistol looks nice on you."
            };
        }

        if (gameObject.name == "BeerGuy" && GlobalGameState.cardFindStarted == false)
        {
            dialogue.sentences = new string[]
            {
                "I want a beer"
            };
        }
        
        if (gameObject.name == "BeerGuy" && GlobalGameState.cardFindStarted == true && GlobalGameState.haveBeer == false)
        {
            dialogue.sentences = new string[]
            {
                "if you give me a beer I give a card"
            };
            
            GlobalGameState.knowBeerForCard = true;
        }
        
        if (gameObject.name == "BeerGuy" && GlobalGameState.cardFindStarted == true && GlobalGameState.haveBeer == true)
        {
            dialogue.sentences = new string[]
            {
                "Thanks for the beer here is your card"
            };
            
            GlobalGameState.cardCount += 1;
            GlobalGameState.haveBeer = false;
        }

        if (gameObject.name == "Merrit Grigg" && GlobalGameState.knowBeerForCard == true &&
            GlobalGameState.haveCash == false)
        {
            dialogue.sentences = new string[]
            {
                "I have beer if you have cash"
            };
        }
        
        if (gameObject.name == "Merrit Grigg" && GlobalGameState.knowBeerForCard == true &&
            GlobalGameState.haveCash == true)
        {
            dialogue.sentences = new string[]
            {
                "here is your beer"
            };
            GlobalGameState.haveBeer = true;
            GlobalGameState.haveCash = false;
            GlobalGameState.knowBeerForCard = false;
        }

        dialogueManager.StartDialogue(dialogue);

    }

    void OnEnable()
    {
        DialogueManager.OnDialogueEnded += TavernOwnerDialogue;
    }

    void OnDisable()
    {
        DialogueManager.OnDialogueEnded -= TavernOwnerDialogue;
    }

    void TavernOwnerDialogue()
    {
        if (GlobalGameState.cardFindFinished)
        {
            dialogue.name = "Tavern Owner";
            dialogue.sentences = new string[]
            {
                "Lets go outside"
            };
            
            dialogueManager.StartDialogue(dialogue);
            GlobalGameState.cardFindFinished = false;
            GlobalGameState.readyToGoOutside =  true;
        }
    }

}