using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Name of the scene to load when player enters the trigger")]
    public string nextSceneName = "Level2";

    [Header("Completion Settings")]
    [Tooltip("Name of the level to mark as completed (usually the current level)")]
    public string currentLevelName;

    [Header("Optional Settings")]
    [Tooltip("Delay before loading the next scene (in seconds)")]
    public float transitionDelay = 0f;

    private void Start()
    {
        if (string.IsNullOrEmpty(currentLevelName))
        {
            currentLevelName = SceneManager.GetActiveScene().name;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LevelCompletionManager.MarkLevelAsCompleted(currentLevelName);

            if (transitionDelay > 0)
            {
                Invoke(nameof(LoadNextLevel), transitionDelay);
            }
            else
            {
                LoadNextLevel();
            }
        }
    }

    private void LoadNextLevel()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(nextSceneName);
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
