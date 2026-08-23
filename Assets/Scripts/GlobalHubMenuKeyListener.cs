using UnityEngine;
using UnityEngine.InputSystem;

public class GlobalHubMenuKeyListener : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("[GlobalHubMenuKeyListener] Start called");
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
        {
            Debug.Log("[GlobalHubMenuKeyListener] M key detected!");

            HubMenuManager hubMenuManager = null;

            if (GlobalHubMenuManager.Instance != null)
            {
                hubMenuManager = GlobalHubMenuManager.Instance.GetHubMenuManager();
                Debug.Log($"[GlobalHubMenuKeyListener] HubMenuManager from Instance: {hubMenuManager != null}");
            }

            if (hubMenuManager == null)
            {
                hubMenuManager = FindFirstObjectByType<HubMenuManager>(FindObjectsInactive.Include);
                Debug.Log($"[GlobalHubMenuKeyListener] HubMenuManager from FindFirstObjectByType (including inactive): {hubMenuManager != null}");
            }

            if (hubMenuManager != null)
            {
                Debug.Log("[GlobalHubMenuKeyListener] Calling ToggleMenu!");
                hubMenuManager.ToggleMenu();
            }
            else
            {
                Debug.LogWarning("[GlobalHubMenuKeyListener] HubMenuManager not found!");
            }
        }
    }
}
