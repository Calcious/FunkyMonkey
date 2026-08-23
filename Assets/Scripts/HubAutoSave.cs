using UnityEngine;

public class HubAutoSave : MonoBehaviour
{
    [Header("Auto Save Settings")]
    public float autoSaveInterval = 60f;

    [Header("Manual Save Key")]
    public KeyCode manualSaveKey = KeyCode.F1;

    private float timeSinceLastSave;

    private void Start()
    {
        SaveManager.Instance.SaveGame();
        ShowNotification("Autosaved Game");
        Debug.Log("Game auto-saved on Hub entry!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(manualSaveKey))
        {
            SaveManager.Instance.SaveGame();
            timeSinceLastSave = 0f;
            ShowNotification("Game Saved");
            Debug.Log("Manual save triggered!");
        }

        timeSinceLastSave += Time.deltaTime;

        if (timeSinceLastSave >= autoSaveInterval)
        {
            SaveManager.Instance.SaveGame();
            timeSinceLastSave = 0f;
            ShowNotification("Autosaved Game");
            Debug.Log("Auto-save triggered!");
        }
    }

    private void ShowNotification(string message)
    {
        if (SaveNotificationUI.Instance != null)
        {
            SaveNotificationUI.Instance.ShowNotification(message);
        }
    }
}
