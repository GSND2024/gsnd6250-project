using UnityEngine;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float defaultDuration = 0.8f;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        canvasGroup.alpha = 1f; 
        StartCoroutine(FadeIn(defaultDuration));
    }

    public IEnumerator FadeIn(float duration)
    {
        float t = duration;
        while (t > 0)
        {
            t -= Time.deltaTime;
            canvasGroup.alpha = t / duration;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    public IEnumerator FadeOut(float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = t / duration;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }
}
