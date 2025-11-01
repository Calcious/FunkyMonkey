using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelSelectIntro : MonoBehaviour
{
    [Header("Intro Settings")]
    public GameObject funkyHubButton;
    public TextMeshProUGUI introText;
    public CanvasGroup buttonContainer;

    [Header("Skip Settings")]
    public SkipController skipController;

    [Header("Text Settings")]
    [TextArea(2, 3)]
    public string journeyText = "Your journey begins at the Funky Hub!";

    [Header("Timing")]
    public float initialDelay = 1f;
    public float textFadeInDuration = 1f;
    public float hubHighlightDuration = 3f;
    public float textFadeOutDuration = 0.5f;

    private static bool hasPlayedIntro = false;
    private bool skipRequested = false;

    private void Start()
    {
        if (MainMenuUI.skipAllIntros)
        {
            hasPlayedIntro = true;
            MainMenuUI.skipAllIntros = false;
        }

        if (!hasPlayedIntro)
        {
            hasPlayedIntro = true;

            if (buttonContainer != null)
            {
                buttonContainer.interactable = false;
                buttonContainer.blocksRaycasts = false;
            }

            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.FadeInImmediate();
            }

            if (skipController != null)
            {
                skipController.OnSkipComplete += OnSkipRequested;
            }

            StartCoroutine(IntroSequence());
        }
        else
        {
            if (introText != null)
            {
                introText.gameObject.SetActive(false);
            }

            if (buttonContainer != null)
            {
                buttonContainer.interactable = true;
                buttonContainer.blocksRaycasts = true;
            }

            if (skipController != null)
            {
                skipController.gameObject.SetActive(false);
            }
        }
    }

    private void OnSkipRequested()
    {
        skipRequested = true;
    }

    private IEnumerator IntroSequence()
    {
        if (introText != null)
        {
            introText.text = journeyText;
            introText.color = new Color(1f, 1f, 1f, 0f);
        }

        yield return new WaitForSeconds(initialDelay);

        if (skipRequested)
        {
            SkipToHub();
            yield break;
        }

        if (funkyHubButton != null)
        {
            StartCoroutine(PulseButton(funkyHubButton));
        }

        if (introText != null)
        {
            yield return StartCoroutine(FadeInText());
        }

        if (skipRequested)
        {
            SkipToHub();
            yield break;
        }

        float remainingTime = hubHighlightDuration - textFadeInDuration - textFadeOutDuration;
        if (remainingTime > 0)
        {
            float elapsed = 0f;
            while (elapsed < remainingTime && !skipRequested)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        if (skipRequested)
        {
            SkipToHub();
            yield break;
        }

        if (introText != null)
        {
            yield return StartCoroutine(FadeOutText());
        }

        if (skipController != null)
        {
            skipController.DisableSkip();
        }

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene("Hub");
        }
    }

    private void SkipToHub()
    {
        StopAllCoroutines();

        if (skipController != null)
        {
            skipController.DisableSkip();
        }

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene("Hub");
        }
    }

    private IEnumerator PulseButton(GameObject button)
    {
        float elapsed = 0f;
        float pulseDuration = 0.5f;
        Vector3 originalScale = button.transform.localScale;
        Vector3 targetScale = originalScale * 1.15f;

        while (elapsed < hubHighlightDuration && !skipRequested)
        {
            float pulseProgress = Mathf.PingPong(elapsed / pulseDuration, 1f);
            button.transform.localScale = Vector3.Lerp(originalScale, targetScale, pulseProgress);
            elapsed += Time.deltaTime;
            yield return null;
        }

        button.transform.localScale = originalScale;
    }

    private IEnumerator FadeInText()
    {
        float elapsed = 0f;

        while (elapsed < textFadeInDuration && !skipRequested)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / textFadeInDuration);
            introText.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        introText.color = new Color(1f, 1f, 1f, 1f);
    }

    private IEnumerator FadeOutText()
    {
        float elapsed = 0f;

        while (elapsed < textFadeOutDuration && !skipRequested)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / textFadeOutDuration);
            introText.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        introText.color = new Color(1f, 1f, 1f, 0f);
    }

    private void OnDestroy()
    {
        if (skipController != null)
        {
            skipController.OnSkipComplete -= OnSkipRequested;
        }
    }
}
