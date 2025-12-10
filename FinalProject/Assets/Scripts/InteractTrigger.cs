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

        if (gameObject.name == "CardFindGuy" && GlobalGameState.cardCount < 4 && GlobalGameState.cardFindStarted == false)
        {
            dialogue.name = "Royce Gallows";
            dialogue.sentences = new string[]
            {
                "Damn it, the aces are missing from my deck.",
                "Could you help me find em?",
                "Tell you what, heard you might be needin' a pistol for the tournament.",
                "If you find the missing cards, I'll lend you my old pistol."
            };
            GlobalGameState.cardFindStarted = true;
        }
        
        if (gameObject.name == "CardFindGuy" && GlobalGameState.cardCount < 4 && GlobalGameState.cardFindStarted == true)
        {
            dialogue.name = "Royce Gallows";
            dialogue.sentences = new string[]
            {
                "Still missing some cards.",
                "I can't start my game 'til I have 'em all.",
                "I'll still lend you the pistol if you find 'em."
            };
            GlobalGameState.cardFindStarted = true;
        }
        
        if (gameObject.name == "CardFindGuy" && GlobalGameState.cardCount >= 4 && GlobalGameState.readyToGoOutside == false)
        {
            dialogue.name = "Royce Gallows";
            dialogue.sentences = new string[]
            {
                "Thanks! Can finally get my game going.",
                "You can borrow my old one. Here."
            };
            Debug.Log("CardFindGuy");
            
            GlobalGameState.cardFindFinished = true;
        }
        
        if (gameObject.name == "CardFindGuy" && GlobalGameState.readyToGoOutside == true)
        {
            dialogue.name = "Royce Gallows";
            dialogue.sentences = new string[]
            {
                "My old pistol looks good on you."
            };
        }

        if (gameObject.name == "BeerGuy" && GlobalGameState.cardFindStarted == false)
        {
            dialogue.sentences = new string[]
            {
                "Man, I'm thirsty.", 
                "I sure could use *hic* another drink..."
            };
        }
        
        if (gameObject.name == "BeerGuy" && GlobalGameState.cardFindStarted == true && GlobalGameState.haveBeer == false)
        {
            dialogue.sentences = new string[]
            {
                "Heard you were *hic* looking for Royce's cards. I happen to have one...",
                "I might be willing to *hic* part with it if you get me another beer *hic*"
            };
            
            GlobalGameState.knowBeerForCard = true;
        }
        
        if (gameObject.name == "BeerGuy" && GlobalGameState.cardFindStarted == true && GlobalGameState.haveBeer == true)
        {
            dialogue.sentences = new string[]
            {
                "Oh thanks! *hic*",
                "Didn't think you'd actually *hic* get me one.",
                "Here's your card"
            };
            
            GlobalGameState.cardCount += 1;
            GlobalGameState.haveBeer = false;
        }

        if (gameObject.name == "Merrit Grigg" && GlobalGameState.knowBeerForCard == true &&
            GlobalGameState.haveCash == false)
        {
            dialogue.sentences = new string[]
            {
                "You need to get Flint another beer, eh?",
                "Well you gotta cough up the coin friend, and unfortunately your pockets are a little light.",
                "Maybe if you ask around you might be able to win the bit of extra change.",
                "If you find yourself with some extra wealth, come let me know, I can get you that beer."
            };
        }
        
        if (gameObject.name == "Merrit Grigg" && GlobalGameState.knowBeerForCard == true &&
            GlobalGameState.haveCash == true)
        {
            dialogue.sentences = new string[]
            {
                "Nice bets at that rat race.",
                "Lookin' to buy that beer now I take it.",
                "Here you are, thanks for your patronage."
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
            dialogue.name = "Merrit Grigg";
            dialogue.sentences = new string[]
            {
                "Alright everyone, lets start moseyin' our way outside now",
                "It's time for the tournament to get started!",
                "Make your way through the front door at your earliest convenience if ya please"
            };
            
            dialogueManager.StartDialogue(dialogue);
            GlobalGameState.cardFindFinished = false;
            GlobalGameState.readyToGoOutside =  true;
        }
    }

}