using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class HubMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject hubMenuUI;
    public TextMeshProUGUI contentText;

    [Header("Emblem Select")]
    public GameObject emblemSelectContent;

    private bool isMenuOpen = false;
    private PlayerControls controls;
    private PauseMenuManager pauseMenuManager;

    private const string DEFAULT_TEXT = "";
    private const string CUSTOMIZATION_TEXT = "CUSTOMIZE HERE TBD";

    private void Awake()
    {
        controls = new PlayerControls();
        pauseMenuManager = FindFirstObjectByType<PauseMenuManager>();

        controls.Player.Pause.performed += ctx => OnPausePressed();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            ToggleMenu();
        }
    }

    private void OnPausePressed()
    {
        if (isMenuOpen)
        {
            CloseMenu();
        }
    }

    private void ToggleMenu()
    {
        if (isMenuOpen)
            CloseMenu();
        else
            OpenMenu();
    }

    private void OpenMenu()
    {
        hubMenuUI.SetActive(true);
        isMenuOpen = true;
        contentText.text = DEFAULT_TEXT;
        Time.timeScale = 0f;

        HideAllContent();

        if (pauseMenuManager != null)
        {
            pauseMenuManager.enabled = false;
        }
    }

    private void CloseMenu()
    {
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

    private void HideAllContent()
    {
        if (emblemSelectContent != null)
        {
            emblemSelectContent.SetActive(false);
        }

        contentText.text = DEFAULT_TEXT;
    }

    public void ShowCustomization()
    {
        HideAllContent();
        contentText.text = CUSTOMIZATION_TEXT;
    }

    public void ShowOption2()
    {
        HideAllContent();

        if (emblemSelectContent != null)
        {
            emblemSelectContent.SetActive(true);
        }
    }

    public void ShowOption3()
    {
        HideAllContent();
        contentText.text = "Option 3 content coming soon...";
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

}
