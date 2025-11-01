using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelNameDisplay : MonoBehaviour
{
    public static LevelNameDisplay Instance { get; private set; }

    [Header("UI References")]
    public Canvas displayCanvas;
    public TextMeshProUGUI levelNameText;

    [Header("Animation Settings")]
    public float fadeInDuration = 1f;
    public float displayDuration = 3f;
    public float fadeOutDuration = 1f;

    private GraphicRaycaster raycaster;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (displayCanvas != null)
            {
                raycaster = displayCanvas.GetComponent<GraphicRaycaster>();
                if (raycaster != null)
                {
                    raycaster.enabled = false;
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (displayCanvas != null)
        {
            displayCanvas.gameObject.SetActive(false);
        }
    }

    public void ShowLevelName(string levelName)
    {
        if (levelNameText != null)
        {
            StartCoroutine(DisplayLevelName(levelName));
        }
    }

    private IEnumerator DisplayLevelName(string levelName)
    {
        if (displayCanvas != null)
        {
            displayCanvas.gameObject.SetActive(true);
        }

        levelNameText.text = levelName.ToUpper();
        levelNameText.color = new Color(1f, 1f, 1f, 0f);

        yield return new WaitForSeconds(0.5f);

        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            levelNameText.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        levelNameText.color = new Color(1f, 1f, 1f, 1f);

        yield return new WaitForSeconds(displayDuration);

        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            levelNameText.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        levelNameText.color = new Color(1f, 1f, 1f, 0f);

        if (displayCanvas != null)
        {
            displayCanvas.gameObject.SetActive(false);
        }
    }
}
