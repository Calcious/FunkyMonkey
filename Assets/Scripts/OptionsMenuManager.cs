using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject mainMenuUI;
    public GameObject optionsMenuPanel;
    public GameObject audioSettingsPanel;
    public GameObject visualSettingsPanel;

    [Header("Audio Settings")]
    public Slider masterVolumeSlider;

    private void Start()
    {
        if (masterVolumeSlider != null && AudioManager.Instance != null)
        {
            masterVolumeSlider.value = AudioManager.Instance.GetMasterVolume();
            masterVolumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }

    public void OpenOptionsMenu()
    {
        if (mainMenuUI != null)
            mainMenuUI.SetActive(false);

        if (optionsMenuPanel != null)
            optionsMenuPanel.SetActive(true);

        HideAllSettings();
    }

    public void CloseOptionsMenu()
    {
        if (optionsMenuPanel != null)
            optionsMenuPanel.SetActive(false);

        if (mainMenuUI != null)
            mainMenuUI.SetActive(true);

        HideAllSettings();
    }

    public void ShowAudioSettings()
    {
        HideAllSettings();
        if (audioSettingsPanel != null)
            audioSettingsPanel.SetActive(true);
    }

    public void ShowVisualSettings()
    {
        HideAllSettings();
        if (visualSettingsPanel != null)
            visualSettingsPanel.SetActive(true);
    }

    private void HideAllSettings()
    {
        if (audioSettingsPanel != null)
            audioSettingsPanel.SetActive(false);

        if (visualSettingsPanel != null)
            visualSettingsPanel.SetActive(false);
    }

    private void OnVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(value);
        }
    }

    private void OnDestroy()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
        }
    }
}
