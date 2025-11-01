using UnityEngine;

public class HubStarter : MonoBehaviour
{
    [Header("Hub Display Settings")]
    public string hubDisplayName = "Funky Hub";
    public bool showNameOnStart = true;

    private void Start()
    {
        if (showNameOnStart && LevelNameDisplay.Instance != null)
        {
            LevelNameDisplay.Instance.ShowLevelName(hubDisplayName);
        }
    }
}
