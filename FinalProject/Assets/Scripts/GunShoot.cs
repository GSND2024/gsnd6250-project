using UnityEngine;
using TMPro;

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
    public DialogueManager dm;

    public Dialogue winDialogue;
    public Dialogue tieDialogue;
    public Dialogue loseDialogue;

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoText();
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

    // ReSharper disable Unity.PerformanceAnalysis
    void ShowResult()
    {
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(true);

        if (destroyedTargets == 6 && winDialogue != null)
            dm.StartDialogue(winDialogue);
        else if (destroyedTargets == 5 && tieDialogue != null)
            dm.StartDialogue(tieDialogue);
        else if (destroyedTargets <= 4 && loseDialogue != null)
            dm.StartDialogue(loseDialogue);
    }
}