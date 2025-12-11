using UnityEngine;
using TMPro;

public class InteractTrigger : MonoBehaviour
{
    public float interactDistance = 3f;
    
    public Dialogue dialogue;
    [SerializeField] private DialogueManager dialogueManager;

    public MazeRaceManager raceManager;

    public TextMeshProUGUI promptText;
    public TextMeshProUGUI cardCount;

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
        if (cardCount != null)
        {
            cardCount.text = "Cards: " + GlobalGameState.cardCount + " / 4";
        }
        

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
            Debug.Log("here");
            dialogue.sentences = new string[]
            {
                "Damn it, the aces are missing from my deck.",
                "Could you help me find em?",
                "Tell you what, heard you might be needin' a pistol for the challenge.",
                "If you find the missing cards, I'll lend you my old pistol."
            };
            GlobalGameState.cardFindStarted = true;
        }
        
        else if (gameObject.name == "CardFindGuy" && GlobalGameState.cardCount < 4 && GlobalGameState.cardFindStarted == true)
        {
            dialogue.sentences = new string[]
            {
                "Still missing some cards.",
                "I can't start my game 'til I have 'em all.",
                "Come to think of it, I dropped my deck right next to the rat race table earlier...",
                "I'll still lend you the pistol if you find 'em."
            };
        }
        
        else if (gameObject.name == "CardFindGuy" && GlobalGameState.cardCount >= 4 && GlobalGameState.readyToGoOutside == false)
        {
            dialogue.sentences = new string[]
            {
                "Thanks! Can finally get my game going.",
                "You can borrow my old one. Here."
            };
            
            GlobalGameState.cardFindFinished = true;
        }
        
        else if (gameObject.name == "CardFindGuy" && GlobalGameState.readyToGoOutside == true)
        {
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
        
        else if (gameObject.name == "BeerGuy" && GlobalGameState.cardFindStarted == true && GlobalGameState.haveBeer == false && GlobalGameState.gotBeerCard == false)
        {
            dialogue.sentences = new string[]
            {
                "Heard you were *hic* looking for Royce's cards. I happen to have one...",
                "I might be willing to *hic* part with it if you get me another beer *hic*"
            };
            
            GlobalGameState.knowBeerForCard = true;
        }
        
        else if (gameObject.name == "BeerGuy" && GlobalGameState.cardFindStarted == true && GlobalGameState.haveBeer == true)
        {
            dialogue.sentences = new string[]
            {
                "Oh thanks! *hic*",
                "Didn't think you'd actually *hic* get me one.",
                "Here's your card"
            };
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.Play();
            GlobalGameState.cardCount += 1;
            GlobalGameState.haveBeer = false;
            GlobalGameState.gotBeerCard = true;
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

        else if (gameObject.name == "Merrit Grigg" && GlobalGameState.knowBeerForCard == true &&
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

        else if (gameObject.name == "Merrit Grigg" && GlobalGameState.readyToGoOutside == true)
        {
            dialogue.sentences = new string[]
            {
                "If you don't mind headin' outside now",
                "The front door's over there, just walk on out"
            };
        }

        if (gameObject.name == "RatRaceGuy" && GlobalGameState.knowRatRace == false)
        {
            dialogue.sentences = new string[]
            {
                "Say friend you look like the betting type",
                "Come place a bet at our rat race table",
                "We put this here rat in the maze, and see if it can do get to the other side in 30 seconds!",
                "Tell you what, if it does I'll give you all the money I got in my pocket, enough for a beer!",
                "Sometimes it escapes though and runs around the bar, I always get it though, no need to worry about it getting in your drink!"
            };
            GlobalGameState.knowRatRace = true;
            GlobalGameState.startRatRace = true;
        }
        else if (gameObject.name == "RatRaceGuy" && GlobalGameState.knowRatRace == true && GlobalGameState.haveCash == false)
        {
            dialogue.sentences = new string[]
            {
                "Back for more?",
                "Let's see if it can make it through the maze this time"
            };
            GlobalGameState.startRatRace = true;
        }

        dialogueManager.StartDialogue(dialogue);

        if (GlobalGameState.startRatRace == true)
        {
            StartCoroutine(StartRatRace());
            GlobalGameState.startRatRace = false;
        }

        if (GlobalGameState.cardFindStarted == true)
        {
            StartCoroutine(ShowCardCollection());
        }
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
            GlobalGameState.readyToGoOutside = true;
        }
    }

    private System.Collections.IEnumerator StartRatRace()
    {
        while (GlobalGameState.inDialogue)
        {
            yield return null;
        }


        raceManager.StartRace();
    }
    private System.Collections.IEnumerator ShowCardCollection()
    {
        while (GlobalGameState.inDialogue)
        {
            yield return null;
        }

        
        if (cardCount != null)
        {
            cardCount.gameObject.SetActive(true);
        }
    }

}