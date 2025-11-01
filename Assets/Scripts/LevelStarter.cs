using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelStarter : MonoBehaviour
{
    private void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "LevelSelect" || currentScene == "Hub" || currentScene == "MainMenu")
        {
            PlayerPrefs.DeleteKey("CurrentLevelName");
            return;
        }

        string levelName = PlayerPrefs.GetString("CurrentLevelName", "");

        if (!string.IsNullOrEmpty(levelName) && LevelNameDisplay.Instance != null)
        {
            LevelNameDisplay.Instance.ShowLevelName(levelName);
        }

        PlayerPrefs.DeleteKey("CurrentLevelName");
    }
}
