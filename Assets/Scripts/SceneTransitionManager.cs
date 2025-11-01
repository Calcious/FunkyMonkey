using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Transition Settings")]
    public Image fadeImage;
    public float fadeInDuration = 1f;
    public float fadeOutDuration = 1f;
    public float minimumFadeDisplayTime = 0.5f;

    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;

            if (fadeImage != null)
            {
                fadeImage.raycastTarget = false;
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isTransitioning)
        {
            StartCoroutine(FadeIn());
        }
    }

    public void LoadScene(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionToScene(sceneName));
        }
    }

    public void LoadSceneWithDelay(string sceneName, float delay)
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionToSceneWithDelay(sceneName, delay));
        }
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        isTransitioning = true;

        yield return StartCoroutine(FadeOut());

        yield return new WaitForSecondsRealtime(minimumFadeDisplayTime);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    private IEnumerator TransitionToSceneWithDelay(string sceneName, float delay)
    {
        isTransitioning = true;

        yield return StartCoroutine(FadeOut());

        yield return new WaitForSecondsRealtime(delay);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        fadeImage.raycastTarget = true;

        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeOutDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, 1f);
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color color = fadeImage.color;
        fadeImage.color = new Color(color.r, color.g, color.b, 1f);
        fadeImage.raycastTarget = true;

        yield return new WaitForSecondsRealtime(0.1f);

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeInDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, 0f);
        fadeImage.raycastTarget = false;
        isTransitioning = false;
    }

    public void FadeInImmediate()
    {
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            fadeImage.color = new Color(color.r, color.g, color.b, 0f);
            fadeImage.raycastTarget = false;
        }
        isTransitioning = false;
    }
}
