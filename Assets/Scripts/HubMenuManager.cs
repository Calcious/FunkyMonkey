using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class HubMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject hubMenuUI;
    public TextMeshProUGUI contentText;

    [Header("Content Panels")]
    public GameObject emblemSelectContent;
    public GameObject customizationContent;
    public GameObject option3Content;

    [Header("Read-Only Indicator")]
    public GameObject readOnlyIndicator;
    public TextMeshProUGUI readOnlyText;

    private bool isMenuOpen = false;
    private bool isReadOnlyMode = false;
    private PlayerControls controls;
    private PauseMenuManager pauseMenuManager;

    private const string DEFAULT_TEXT = "";

    private void Awake()
    {
        Debug.Log("[HubMenuManager] Awake called");
        controls = new PlayerControls();
        pauseMenuManager = FindFirstObjectByType<PauseMenuManager>();

        controls.Player.Pause.performed += ctx => OnPausePressed();
    }

    private void OnEnable()
    {
        controls.Enable();
        Debug.Log("[HubMenuManager] OnEnable - controls enabled");
    }

    private void OnDisable()
    {
        controls.Disable();
        Debug.Log("[HubMenuManager] OnDisable - controls disabled");
    }

    private void OnPausePressed()
    {
        if (isMenuOpen)
        {
            CloseMenu();
        }
    }

    public void ToggleMenu()
    {
        Debug.Log($"[HubMenuManager] ToggleMenu called. isMenuOpen: {isMenuOpen}");
        if (isMenuOpen)
            CloseMenu();
        else
            OpenMenu();
    }

    private void OpenMenu()
    {
        Debug.Log("[HubMenuManager] Opening menu");
        hubMenuUI.SetActive(true);
        isMenuOpen = true;
        contentText.text = DEFAULT_TEXT;
        Time.timeScale = 0f;

        HideAllContent();

        if (readOnlyIndicator != null)
        {
            readOnlyIndicator.SetActive(isReadOnlyMode);
        }

        if (pauseMenuManager != null)
        {
            pauseMenuManager.enabled = false;
        }

        Debug.Log("[HubMenuManager] Menu opened successfully");
    }

    private void CloseMenu()
    {
        Debug.Log("[HubMenuManager] Closing menu");
        if (isMenuOpen)
        {
            hubMenuUI.SetActive(false);
            isMenuOpen = false;
            Time.timeScale = 1f;

            HideAllContent();

            if (pauseMenuManager != null)
            {
                pauseMenuManager.enabled = true;
            }
        }
    }

    public void ForceCloseMenu()
    {
        Debug.Log("[HubMenuManager] ForceCloseMenu called");

        bool foundAndClosed = false;

        if (hubMenuUI != null)
        {
            hubMenuUI.SetActive(false);
            foundAndClosed = true;
            Debug.Log("[HubMenuManager] hubMenuUI deactivated");
        }
        else
        {
            Debug.LogWarning("[HubMenuManager] hubMenuUI is null, searching for UI panel in children");

            UnityEngine.UI.Image[] images = GetComponentsInChildren<UnityEngine.UI.Image>(true);
            foreach (UnityEngine.UI.Image img in images)
            {
                if (img.transform != transform && img.transform.parent == transform)
                {
                    img.gameObject.SetActive(false);
                    hubMenuUI = img.gameObject;
                    foundAndClosed = true;
                    Debug.Log($"[HubMenuManager] Found and deactivated main panel: {img.gameObject.name}");
                    break;
                }
            }
        }

        if (!foundAndClosed)
        {
            Debug.LogError("[HubMenuManager] Could not find UI panel to close!");
            foreach (Transform child in transform)
            {
                Debug.Log($"[HubMenuManager] Child found: {child.name}, active: {child.gameObject.activeSelf}");
                if (child.GetComponent<UnityEngine.UI.Image>() != null)
                {
                    child.gameObject.SetActive(false);
                    hubMenuUI = child.gameObject;
                    Debug.Log($"[HubMenuManager] Deactivated: {child.name}");
                    break;
                }
            }
        }

        isMenuOpen = false;
        Time.timeScale = 1f;
        HideAllContent();
    }


    public void CloseMenuInitial()
    {
        Debug.Log("[HubMenuManager] CloseMenuInitial - ensuring menu starts closed");
        if (hubMenuUI != null)
        {
            hubMenuUI.SetActive(false);
        }
        isMenuOpen = false;
        Time.timeScale = 1f;
        HideAllContent();
    }

    private void HideAllContent()
    {
        if (emblemSelectContent != null)
        {
            emblemSelectContent.SetActive(false);
        }

        if (customizationContent != null)
        {
            customizationContent.SetActive(false);
        }

        if (option3Content != null)
        {
            option3Content.SetActive(false);
        }

        contentText.text = DEFAULT_TEXT;
    }

    public void ShowCustomization()
    {
        HideAllContent();

        if (customizationContent != null)
        {
            customizationContent.SetActive(true);

            if (isReadOnlyMode)
            {
                DisableInteractionsInPanel(customizationContent);
            }
        }
    }

    public void ShowOption2()
    {
        HideAllContent();

        if (emblemSelectContent != null)
        {
            emblemSelectContent.SetActive(true);

            if (isReadOnlyMode)
            {
                DisableInteractionsInPanel(emblemSelectContent);
            }
        }
    }

    public void ShowOption3()
    {
        HideAllContent();

        if (option3Content != null)
        {
            option3Content.SetActive(true);

            if (isReadOnlyMode)
            {
                DisableInteractionsInPanel(option3Content);
            }
        }
    }

    public void ShowOption4()
    {
        HideAllContent();
        contentText.text = "Option 4 content coming soon...";
    }

    public void CloseEmblemSelect()
    {
        HideAllContent();
    }

    public void SetReadOnlyMode(bool readOnly)
    {
        Debug.Log($"[HubMenuManager] SetReadOnlyMode called with: {readOnly}");
        isReadOnlyMode = readOnly;

        if (readOnlyIndicator != null)
        {
            readOnlyIndicator.SetActive(readOnly);
        }

        if (readOnlyText != null)
        {
            readOnlyText.text = readOnly ? "VIEW ONLY MODE" : "";
        }
    }

    private void DisableInteractionsInPanel(GameObject panel)
    {
        DraggableItem[] draggables = panel.GetComponentsInChildren<DraggableItem>(true);
        foreach (DraggableItem draggable in draggables)
        {
            draggable.enabled = false;
        }

        DropZoneHandler[] dropZones = panel.GetComponentsInChildren<DropZoneHandler>(true);
        foreach (DropZoneHandler dropZone in dropZones)
        {
            dropZone.enabled = false;
        }

        Button[] buttons = panel.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            if (button.gameObject.name.Contains("Close") ||
                button.gameObject.name.Contains("Exit"))
            {
                continue;
            }
            button.interactable = false;
        }

        ScrollRect[] scrollRects = panel.GetComponentsInChildren<ScrollRect>(true);
        foreach (ScrollRect scroll in scrollRects)
        {
            scroll.enabled = true;
        }
    }
}
