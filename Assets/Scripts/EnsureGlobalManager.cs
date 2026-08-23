using UnityEngine;

[DefaultExecutionOrder(-100)]
public class EnsureGlobalManager : MonoBehaviour
{
    [Header("Hub Menu Settings")]
    public GameObject hubMenuPrefab;

    private void Awake()
    {
        Debug.Log("[EnsureGlobalManager] Awake called - checking for GlobalHubMenuManager");

        if (GlobalHubMenuManager.Instance == null)
        {
            Debug.Log("[EnsureGlobalManager] GlobalHubMenuManager not found, creating new one");

            GameObject managerGO = new GameObject("GlobalHubMenuManager");
            GlobalHubMenuManager manager = managerGO.AddComponent<GlobalHubMenuManager>();

            managerGO.AddComponent<GlobalHubMenuKeyListener>();

            if (hubMenuPrefab != null)
            {
                manager.hubMenuPrefab = hubMenuPrefab;
                Debug.Log("[EnsureGlobalManager] Hub Menu Prefab assigned successfully");
            }
            else
            {
                Debug.LogError("[EnsureGlobalManager] Hub Menu Prefab not assigned in Inspector!");
            }

            Debug.Log("[EnsureGlobalManager] Created GlobalHubMenuManager with KeyListener");
        }
        else
        {
            Debug.Log("[EnsureGlobalManager] GlobalHubMenuManager already exists (good!)");
        }
    }

    private void Start()
    {
        Debug.Log($"[EnsureGlobalManager] Start - GlobalHubMenuManager exists: {GlobalHubMenuManager.Instance != null}");
    }
}
