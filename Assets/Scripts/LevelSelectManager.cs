using UnityEngine;

public class LevelSelectManager : MonoBehaviour
{
    public void LoadLevel(string levelName)
    {
        LoadLevelWithName(levelName, levelName);
    }

    public void LoadLevelWithName(string sceneName, string displayName)
    {
        PlayerPrefs.SetString("CurrentLevelName", displayName);
        PlayerPrefs.Save();

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(sceneName);
        }
    }

    public void LoadHub()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene("Hub");
        }
    }

    public void ReturnToMainMenu()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene("MainMenu");
        }
    }
}
