using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [Header("Level Settings")]
    public string sceneName;
    public string displayName;

    [Header("Completion Visual")]
    public GameObject completionSlash;
    public Image slashImage;
    public Color slashColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);

    [Header("Disable When Completed")]
    public bool disableWhenCompleted = true;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        UpdateCompletionVisual();
    }

    private void OnEnable()
    {
        UpdateCompletionVisual();
    }

    public void UpdateCompletionVisual()
    {
        bool isCompleted = LevelCompletionManager.IsLevelCompleted(sceneName);

        if (completionSlash != null)
        {
            completionSlash.SetActive(isCompleted);
        }

        if (slashImage != null && isCompleted)
        {
            slashImage.color = slashColor;
        }

        if (disableWhenCompleted && button != null && isCompleted)
        {
            button.interactable = false;
        }
    }

    public void LoadThisLevel()
    {
        bool isCompleted = LevelCompletionManager.IsLevelCompleted(sceneName);

        if (disableWhenCompleted && isCompleted)
        {
            Debug.Log($"Level '{displayName}' is already completed!");
            return;
        }

        PlayerPrefs.SetString("CurrentLevelName", displayName);
        PlayerPrefs.Save();

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(sceneName);
        }
    }
}
