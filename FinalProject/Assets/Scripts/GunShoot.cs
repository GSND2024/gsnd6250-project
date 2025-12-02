using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GunShoot : MonoBehaviour
{
    public AudioSource audioSource;
    public Camera cam;
    public float shootDistance = 100f;

    public int maxAmmo = 6;
    int currentAmmo;
    int destroyedTargets = 0;

    public TMP_Text ammoText;

    public GameObject dialogueCanvas;
    public Dialogue winDialogue;
    public Dialogue tieDialogue;
    public Dialogue loseDialogue;

    public GameObject resultMenu;
    public Button replayButton;
    public Button endButton;

    public string replaySceneName = "ShootingArea";
    public string endSceneName = "EndScene";

    public MonoBehaviour lookController;
    
    void OnEnable()
    {
        DialogueManager.OnDialogueEnded += ShowResultMenu;
    }

    void OnDisable()
    {
        DialogueManager.OnDialogueEnded -= ShowResultMenu;
    }

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoText();

        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        if (resultMenu != null)
            resultMenu.SetActive(false);

        if (replayButton != null)
            replayButton.onClick.AddListener(ReplayScene);

        if (endButton != null)
            endButton.onClick.AddListener(GoToEndScene);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentAmmo > 0)
        {
            if (audioSource != null)
                audioSource.Play();

            ShootRay();

            currentAmmo--;
            UpdateAmmoText();

            if (currentAmmo == 0)
                ShowResult();
        }
    }

    void ShootRay()
    {
        if (cam == null) return;

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = cam.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit hit, shootDistance))
        {
            if (hit.collider.CompareTag("Target"))
            {
                destroyedTargets++;
                Destroy(hit.collider.gameObject);
            }
        }
    }

    void UpdateAmmoText()
    {
        if (ammoText != null)
            ammoText.text = "Ammo: " + currentAmmo + "/" + maxAmmo;
    }

    void ShowResult()
    {
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(true);

        DialogueManager dm = FindObjectOfType<DialogueManager>();
        if (dm == null) return;

        if (destroyedTargets == 6 && winDialogue != null)
            dm.StartDialogue(winDialogue);
        else if (destroyedTargets == 5 && tieDialogue != null)
            dm.StartDialogue(tieDialogue);
        else if (destroyedTargets <= 4 && loseDialogue != null)
            dm.StartDialogue(loseDialogue);
    }

    void ShowResultMenu()
    {
        if (resultMenu != null)
            resultMenu.SetActive(true);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        if (lookController != null)
            lookController.enabled = false;
    }

    void ReplayScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(replaySceneName);
    }

    void GoToEndScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(endSceneName);
    }
}