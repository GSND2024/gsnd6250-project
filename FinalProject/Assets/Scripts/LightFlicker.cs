using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    private Light lightSource;
    [Header("Flicker Settings")]
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.2f;
    public float flickerSpeed = 0.1f; // lower = faster flicker
    private float targetIntensity;

    void Start()
    {
        lightSource = GetComponent<Light>();
        targetIntensity = lightSource.intensity;
    }

    void Update()
    {
        // Smoothly move toward a new random target intensity
        lightSource.intensity = Mathf.Lerp(lightSource.intensity, targetIntensity, Time.deltaTime * 10f);

        // Occasionally pick a new target
        if (Random.value < flickerSpeed)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity);
        }
    }
}
