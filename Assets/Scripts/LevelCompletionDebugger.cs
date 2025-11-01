using UnityEngine;

public class LevelCompletionDebugger : MonoBehaviour
{
    [Header("Debug Tools")]
    [Tooltip("Press this key to clear all level completions")]
    public KeyCode clearAllKey = KeyCode.F12;

    [Tooltip("Press this key to log all completion states")]
    public KeyCode logStatesKey = KeyCode.F11;

    private void Update()
    {
        if (Input.GetKeyDown(clearAllKey))
        {
            LevelCompletionManager.ClearAllCompletions();
            Debug.Log("All level completions cleared!");
        }

        if (Input.GetKeyDown(logStatesKey))
        {
            LogAllCompletionStates();
        }
    }

    private void LogAllCompletionStates()
    {
        string[] levels = new string[]
        {
            "Metal", "Pop", "Emo", "Punk", "Grunge",
            "Synth", "Dub", "FunkyFinal", "Level1", "Level2"
        };

        Debug.Log("=== Level Completion States ===");
        foreach (string level in levels)
        {
            bool completed = LevelCompletionManager.IsLevelCompleted(level);
            Debug.Log($"{level}: {(completed ? "COMPLETED ?" : "Not Completed")}");
        }
        Debug.Log("==============================");
    }

    [ContextMenu("Clear All Completions")]
    public void ClearAll()
    {
        LevelCompletionManager.ClearAllCompletions();
    }

    [ContextMenu("Mark All as Completed (Testing)")]
    public void MarkAllCompleted()
    {
        string[] levels = new string[]
        {
            "Metal", "Pop", "Emo", "Punk", "Grunge",
            "Synth", "Dub", "FunkyFinal", "Level1", "Level2"
        };

        foreach (string level in levels)
        {
            LevelCompletionManager.MarkLevelAsCompleted(level);
        }
        Debug.Log("All levels marked as completed for testing!");
    }
}
