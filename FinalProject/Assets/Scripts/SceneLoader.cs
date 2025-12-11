using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static FadeManager fade;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        if (fade == null)
            fade = FindObjectOfType<FadeManager>();
    }

    public static void LoadScene(string sceneName, float fadeDuration = 0.8f)
    {
        fade.StartCoroutine(LoadRoutine(sceneName, fadeDuration));
    }

    private static IEnumerator LoadRoutine(string sceneName, float duration)
    {
        yield return fade.FadeOut(duration);
        SceneManager.LoadScene(sceneName);
        yield return fade.FadeIn(duration);
    }
}