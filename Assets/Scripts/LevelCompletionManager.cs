using UnityEngine;

public class LevelCompletionManager : MonoBehaviour
{
    private const string COMPLETION_PREFIX = "LevelCompleted_";

    public static void MarkLevelAsCompleted(string levelName)
    {
        if (string.IsNullOrEmpty(levelName)) return;

        PlayerPrefs.SetInt(COMPLETION_PREFIX + levelName, 1);
        PlayerPrefs.Save();
        Debug.Log($"Level '{levelName}' marked as completed!");
    }

    public static bool IsLevelCompleted(string levelName)
    {
        if (string.IsNullOrEmpty(levelName)) return false;
        return PlayerPrefs.GetInt(COMPLETION_PREFIX + levelName, 0) == 1;
    }

    public static void ClearAllCompletions()
    {
        string[] levels = new string[]
        {
            "Metal", "Pop", "Emo", "Punk", "Grunge",
            "Synth", "Dub", "FunkyFinal", "Level1", "Level2"
        };

        foreach (string level in levels)
        {
            PlayerPrefs.DeleteKey(COMPLETION_PREFIX + level);
        }
        PlayerPrefs.Save();
        Debug.Log("All level completions cleared!");
    }

    public static void ResetLevelCompletion(string levelName)
    {
        if (string.IsNullOrEmpty(levelName)) return;
        PlayerPrefs.DeleteKey(COMPLETION_PREFIX + levelName);
        PlayerPrefs.Save();
    }
}
