using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [Header("Menu Buttons")]
    public Button continueButton;
    public GameObject[] menuButtonsToHideOnStart;

    [Header("Transition Settings")]
    public GameObject transitionPanel;
    public Image fadeImage;
    public TextMeshProUGUI storyText;

    [Header("Skip Settings")]
    public SkipController skipController;

    [Header("Story Text")]
    [TextArea(3, 5)]
    public string firstStoryText = "In a land of funky beats and ancient honor...\n\nOne monkey must find his way.";

    [TextArea(3, 5)]
    public string secondStoryText = "Armed with his blade and unmatched style...\n\nThe journey begins.";

    [Header("Timing")]
    public float fadeOutDuration = 1f;
    public float textDisplayDuration = 4f;
    public float textFadeDuration = 1f;
    public float fadeToDuration = 1f;

    private bool skipRequested = false;
    public static bool skipAllIntros = false;

    private void Start()
    {
        UpdateContinueButton();
    }

    private void UpdateContinueButton()
    {
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(SaveManager.HasSaveData());
        }
    }

    public void ContinueGame()
    {
        SaveManager.Instance.LoadAndApplyGame();
    }

    public void StartGame()
    {
        skipAllIntros = false;

        if (menuButtonsToHideOnStart != null)
        {
            foreach (var button in menuButtonsToHideOnStart)
            {
                if (button != null)
                    button.SetActive(false);
            }
        }

        StartCoroutine(TransitionToLevel());
    }

    private IEnumerator TransitionToLevel()
    {
        transitionPanel.SetActive(true);

        if (skipController != null)
        {
            skipController.OnSkipComplete += OnSkipRequested;
        }

        yield return StartCoroutine(FadeToBlack());

        if (skipRequested)
        {
            SkipToHub();
            yield break;
        }

        yield return StartCoroutine(ShowStoryText(firstStoryText));

        if (skipRequested)
        {
            SkipToHub();
            yield break;
        }

        yield return StartCoroutine(ShowStoryText(secondStoryText));

        if (skipRequested)
        {
            SkipToHub();
            yield break;
        }

        yield return new WaitForSeconds(fadeToDuration);

        if (skipController != null)
        {
            skipController.DisableSkip();
        }

        SceneManager.LoadScene("LevelSelect");
    }

    private void OnSkipRequested()
    {
        skipRequested = true;
    }

    private void SkipToHub()
    {
        StopAllCoroutines();
        skipAllIntros = true;
        SceneManager.LoadScene("Hub");
    }

    private IEnumerator FadeToBlack()
    {
        float elapsed = 0f;
        Color fadeColor = fadeImage.color;

        while (elapsed < fadeOutDuration && !skipRequested)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeOutDuration);
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 1f);
    }

    private IEnumerator ShowStoryText(string text)
    {
        storyText.text = text;

        float elapsed = 0f;
        while (elapsed < textFadeDuration && !skipRequested)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / textFadeDuration);
            storyText.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        if (skipRequested) yield break;

        storyText.color = new Color(1f, 1f, 1f, 1f);

        elapsed = 0f;
        while (elapsed < textDisplayDuration && !skipRequested)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (skipRequested) yield break;

        elapsed = 0f;
        while (elapsed < textFadeDuration && !skipRequested)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / textFadeDuration);
            storyText.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        storyText.color = new Color(1f, 1f, 1f, 0f);
    }

    private void OnDestroy()
    {
        if (skipController != null)
        {
            skipController.OnSkipComplete -= OnSkipRequested;
        }
    }
}
