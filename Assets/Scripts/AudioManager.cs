using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;

    private const string MASTER_VOLUME_KEY = "MasterVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolumeSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMasterVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }

        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    public float GetMasterVolume()
    {
        return PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
    }

    private void LoadVolumeSettings()
    {
        float savedVolume = GetMasterVolume();
        if (musicSource != null)
        {
            musicSource.volume = savedVolume;
        }
    }
}
