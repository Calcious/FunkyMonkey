using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class SceneBootstrapper : MonoBehaviour
{
    [Header("Global Manager Prefab")]
    public GameObject globalManagerPrefab;

    private void Awake()
    {
        EnsureGlobalManagerExists();
    }

    private void EnsureGlobalManagerExists()
    {
        if (GlobalHubMenuManager.Instance == null)
        {
            Debug.Log("[SceneBootstrapper] GlobalHubMenuManager not found, creating from prefab");

            if (globalManagerPrefab != null)
            {
                Instantiate(globalManagerPrefab);
            }
            else
            {
                GameObject managerGO = new GameObject("GlobalHubMenuManager");
                GlobalHubMenuManager manager = managerGO.AddComponent<GlobalHubMenuManager>();

                GameObject prefab = Resources.Load<GameObject>("HubMenuPanel");
                if (prefab != null)
                {
                    manager.hubMenuPrefab = prefab;
                }
                else
                {
                    Debug.LogWarning("[SceneBootstrapper] Could not find HubMenuPanel in Resources folder");
                }
            }
        }
        else
        {
            Debug.Log("[SceneBootstrapper] GlobalHubMenuManager already exists");
        }
    }
}
