using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GlobalHubMenuManager : MonoBehaviour
{
    private static GlobalHubMenuManager instance;
    public static GlobalHubMenuManager Instance => instance;

    [Header("Hub Menu Prefab")]
    public GameObject hubMenuPrefab;

    private GameObject currentHubMenuInstance;
    private HubMenuManager hubMenuManager;
    private bool isReadOnlyMode = false;

    public HubMenuManager GetHubMenuManager() => hubMenuManager;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        CheckCurrentScene();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void CheckCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        OnSceneLoaded(currentScene, LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;
        Debug.Log($"[GlobalHubMenuManager] Scene loaded: {sceneName}");

        if (sceneName == "Hub")
        {
            isReadOnlyMode = false;
            FindExistingHubMenu();
        }
        else if (sceneName != "MainMenu" && sceneName != "LevelSelect")
        {
            isReadOnlyMode = true;
            StartCoroutine(InstantiateHubMenuForReadOnlyDelayed());
        }
        else
        {
            if (currentHubMenuInstance != null)
            {
                Debug.Log("[GlobalHubMenuManager] Destroying hub menu instance for MainMenu/LevelSelect");
                Destroy(currentHubMenuInstance);
                currentHubMenuInstance = null;
                hubMenuManager = null;
            }
        }
    }

    private void FindExistingHubMenu()
    {
        hubMenuManager = FindFirstObjectByType<HubMenuManager>();
        if (hubMenuManager != null)
        {
            Debug.Log("[GlobalHubMenuManager] Found existing HubMenuManager in Hub scene");
            hubMenuManager.SetReadOnlyMode(false);
        }
        else
        {
            Debug.LogWarning("[GlobalHubMenuManager] Could not find HubMenuManager in Hub scene!");
        }
    }

    private IEnumerator InstantiateHubMenuForReadOnlyDelayed()
    {
        yield return new WaitForEndOfFrame();

        if (currentHubMenuInstance == null && hubMenuPrefab != null)
        {
            Debug.Log("[GlobalHubMenuManager] Instantiating hub menu for read-only mode");

            Canvas canvas = FindScreenSpaceCanvas();
            if (canvas == null)
            {
                Debug.Log("[GlobalHubMenuManager] No Screen Space canvas found, creating one");
                GameObject canvasGO = new GameObject("GlobalHubMenuCanvas");
                canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 1000;
                canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

                DontDestroyOnLoad(canvasGO);
            }

            currentHubMenuInstance = Instantiate(hubMenuPrefab, canvas.transform);
            currentHubMenuInstance.name = "HubMenuPanel (Read-Only)";

            hubMenuManager = currentHubMenuInstance.GetComponentInChildren<HubMenuManager>(true);

            if (hubMenuManager != null)
            {
                hubMenuManager.gameObject.SetActive(true);

                yield return null;

                Debug.Log("[GlobalHubMenuManager] Setting read-only mode on HubMenuManager");
                hubMenuManager.SetReadOnlyMode(true);
                hubMenuManager.ForceCloseMenu();

                Debug.Log($"[GlobalHubMenuManager] HubMenuManager initialized and closed");
            }
            else
            {
                Debug.LogError("[GlobalHubMenuManager] HubMenuManager component not found in prefab!");
            }
        }
        else if (hubMenuPrefab == null)
        {
            Debug.LogError("[GlobalHubMenuManager] Hub Menu Prefab is not assigned!");
        }
    }

    private Canvas FindScreenSpaceCanvas()
    {
        Canvas[] allCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);

        foreach (Canvas canvas in allCanvases)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay ||
                canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                Debug.Log($"[GlobalHubMenuManager] Found Screen Space canvas: {canvas.gameObject.name}");
                return canvas;
            }
        }

        return null;
    }

    public bool IsReadOnlyMode()
    {
        return isReadOnlyMode;
    }
}
