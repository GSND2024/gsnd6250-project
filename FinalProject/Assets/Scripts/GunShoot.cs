using UnityEngine;

public class GunShoot : MonoBehaviour
{
    public AudioSource audioSource;
    public Camera cam;
    public float shootDistance = 100f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (audioSource != null)
                audioSource.Play();

            ShootRay();
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
                Destroy(hit.collider.gameObject);
        }
    }
}

