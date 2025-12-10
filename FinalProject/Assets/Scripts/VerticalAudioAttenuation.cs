using UnityEngine;

public class VerticalAudioAttenuation : MonoBehaviour
{
    public Transform player; // assign the player object
    public float floorHeight = 0f; // Y level of floor 1
    public float maxDistance = 3f; // how far up before fully muffled
    public float minVolume = 0.1f; // muffled volume on floor 2
    public float maxVolume = 1.0f; // full volume on floor 1

    private AudioSource audioSource;

    [Header("Low Pass Settings")]
    public float maxCutoff = 22000f;   // normal hearing range, full clarity
    public float minCutoff = 800f;     // muffled, like behind a floor
    private AudioLowPassFilter lowPass;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lowPass = GetComponent<AudioLowPassFilter>();

        if (lowPass == null)
        {
            // Auto-add if missing
            lowPass = gameObject.AddComponent<AudioLowPassFilter>();
        }
    }

    private void Update()
    {
        float yDist = Mathf.Abs(player.position.y - floorHeight);

        // 0 if at floor height, 1 if above maxDistance or higher
        float t = Mathf.Clamp01(yDist / maxDistance);

        // Lerp volume between max and min
        audioSource.volume = Mathf.Lerp(maxVolume, minVolume, t);
        lowPass.cutoffFrequency = Mathf.Lerp(maxCutoff, minCutoff, t);
    }
}