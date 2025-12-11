using System.Collections;
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
    public Dialogue fiveDialogue;

    public GameObject resultMenu;
    public Button replayButton;
    public Button endButton;

    public string replaySceneName = "ShootingArea";
    public string endSceneName = "EndScene";

    public MonoBehaviour lookController;

    public Transform muzzleTransform;
    public GameObject bulletTrailPrefab;
    public float trailSpeed = 200f;
    public float recoilKick = 2f;
    
    public GameObject hitEffectPrefab;

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

        Vector3 startPoint = muzzleTransform != null ? muzzleTransform.position : ray.origin;
        Vector3 hitPoint = startPoint + ray.direction * shootDistance;

        if (Physics.Raycast(ray, out RaycastHit hit, shootDistance))
        {
            hitPoint = hit.point;

            if (hit.collider.CompareTag("Target"))
            {
                destroyedTargets++;
                SpawnHitEffect(hit.point, hit.normal);
                Destroy(hit.collider.gameObject);
            }
        }

        SpawnTrail(startPoint, hitPoint);
        ApplyRecoil();
    }

    void SpawnTrail(Vector3 start, Vector3 end)
    {
        if (bulletTrailPrefab == null) return;

        GameObject trail = Instantiate(bulletTrailPrefab, start, Quaternion.identity);
        StartCoroutine(MoveTrail(trail, start, end));
    }

    IEnumerator MoveTrail(GameObject trail, Vector3 start, Vector3 end)
    {
        float distance = Vector3.Distance(start, end);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * (trailSpeed / distance);
            trail.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        trail.transform.position = end;
        Destroy(trail, 0.1f);
    }

    void ApplyRecoil()
    {
        if (lookController is PlayerController3D pc)
            pc.externalRecoil += recoilKick;
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
        else if (destroyedTargets == 4 && tieDialogue != null)
            dm.StartDialogue(tieDialogue);
        else if (destroyedTargets <= 3 && loseDialogue != null)
            dm.StartDialogue(loseDialogue);
        else if (destroyedTargets == 5 && fiveDialogue != null)
            dm.StartDialogue(fiveDialogue);
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
        SceneLoader.LoadScene(replaySceneName);
    }

    void GoToEndScene()
    {
        Time.timeScale = 1f;
        SceneLoader.LoadScene(endSceneName);
    }
    
    void SpawnHitEffect(Vector3 position, Vector3 normal)
{
    Debug.Log("HitEffect Spawned at: " + position);
    if (hitEffectPrefab == null) return;

    Quaternion rot = Quaternion.LookRotation(normal);
    Instantiate(hitEffectPrefab, position, rot);
}
}
