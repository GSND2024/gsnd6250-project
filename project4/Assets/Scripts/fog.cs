using UnityEngine;
using System.Collections;

public class fog : MonoBehaviour
{
    public float fadeDuration = 0.25f;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeAndDisable());
        }
    }

    private IEnumerator FadeAndDisable()
    {
        float elapsed = 0f;
        Color originalColor = sr.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Ensure fully transparent before disabling
        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        gameObject.SetActive(false);
    }
}
