using UnityEngine;
using TMPro;

public class InteractTrigger : MonoBehaviour
{
    public float interactDistance = 3f;
    public string interactMessage = "Press SPACE to interact";
    
    public Dialogue dialogue;
    [SerializeField] private DialogueManager dialogueManager;

    [Header("UI Reference")]
    public TextMeshProUGUI promptText; // 在 Inspector 把你的 TMP UI 拖进来

    private Transform player;
    private bool playerInRange = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (promptText != null)
            promptText.gameObject.SetActive(false); // 确保初始隐藏
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
            promptText.text = interactMessage;
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
        // TODO: 在这里写你的互动逻辑，比如：Talk/Hack
        // DialogueManager.Instance.BeginTalk();
        // HackManager.Instance.BeginHack(targetBot);
    }
}