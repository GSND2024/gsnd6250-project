using UnityEngine;

public class BulletTrigger : MonoBehaviour
{
    [Header("Spin Settings")]
    public Vector3 rotationSpeed = new Vector3(0f, 60f, 0f); // Degrees per second

    [Header("Bob Settings")]
    public float bobAmplitude = 0.25f; // How high/low it moves
    public float bobFrequency = 2f;    // Speed of bobbing

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Spin object
        transform.Rotate(rotationSpeed * Time.deltaTime);

        // Bob up and down
        float newY = startPos.y + Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Find a bullet");
            GlobalGameState.bulletCount += 1;
            Destroy(gameObject);
        }
    }
}
