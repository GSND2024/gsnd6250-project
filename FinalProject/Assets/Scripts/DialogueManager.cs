using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup rootCanvas;          // on DialogueRoot
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private TextMeshProUGUI continueText;    // small "Press Enter >>"

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float charDelay = 0.02f;
    [SerializeField] private KeyCode advanceKey = KeyCode.Return;

    private readonly Queue<string> _sentences = new Queue<string>();
    private Coroutine _typingRoutine;
    private bool _isTyping;
    private string _currentSentence;

    private bool _active;

    private void Awake()
    {
        // Start hidden
        if (rootCanvas != null)
        {
            rootCanvas.alpha = 0f;
            rootCanvas.interactable = false;
            rootCanvas.blocksRaycasts = false;
        }

        if (bodyText != null)
        {
            bodyText.textWrappingMode = TMPro.TextWrappingModes.Normal;
            bodyText.overflowMode = TMPro.TextOverflowModes.Overflow;
        }

        if (continueText != null)
            continueText.gameObject.SetActive(false);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogue == null) return;

        _active = true;
        GlobalGameState.dialogueActive = true; // if you still use this elsewhere

        // Optional: pause game
        Time.timeScale = 0f;

        _sentences.Clear();
        foreach (var s in dialogue.sentences)
            _sentences.Enqueue(s);

        nameText.text = dialogue.name;

        ShowUI();
        DisplayNextSentence();
    }

    private void ShowUI()
    {
        if (rootCanvas == null) return;

        rootCanvas.interactable = true;
        rootCanvas.blocksRaycasts = true;
        StopAllCoroutines();
        StartCoroutine(FadeCanvas(rootCanvas, 0f, 1f, fadeDuration));
    }

    private void HideUI()
    {
        if (rootCanvas == null) return;

        rootCanvas.interactable = false;
        rootCanvas.blocksRaycasts = false;
        StopAllCoroutines();
        StartCoroutine(FadeCanvas(rootCanvas, rootCanvas.alpha, 0f, fadeDuration));
    }

    private void DisplayNextSentence()
    {
        if (_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        _currentSentence = _sentences.Dequeue();

        if (_typingRoutine != null)
            StopCoroutine(_typingRoutine);

        if (continueText != null)
            continueText.gameObject.SetActive(false);

        _typingRoutine = StartCoroutine(TypeSentence(_currentSentence));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        _isTyping = true;
        bodyText.text = "";

        foreach (char c in sentence)
        {
            bodyText.text += c;

            float t = 0f;
            while (t < charDelay)
            {
                t += Time.unscaledDeltaTime; // still works when Time.timeScale = 0
                yield return null;
            }
        }

        _isTyping = false;

        if (continueText != null)
            continueText.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!_active) return;

        if (Input.GetKeyDown(advanceKey) || Input.GetKeyDown(KeyCode.Space))
        {
            if (_isTyping)
            {
                // Skip typing and show the whole line
                if (_typingRoutine != null)
                    StopCoroutine(_typingRoutine);

                bodyText.text = _currentSentence;
                _isTyping = false;

                if (continueText != null)
                    continueText.gameObject.SetActive(true);
            }
            else
            {
                DisplayNextSentence();
            }
        }
    }

    private void EndDialogue()
    {
        _active = false;
        GlobalGameState.dialogueActive = false;
        Time.timeScale = 1f;

        HideUI();
    }

    private IEnumerator FadeCanvas(CanvasGroup cg, float from, float to, float duration)
    {
        float t = 0f;
        cg.alpha = from;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        cg.alpha = to;
    }
}
