using UnityEngine;
using System.Collections;

public class LevelMusicStopper : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeOutDuration = 2f;

    private void Start()
    {
        MusicManager musicManager = FindFirstObjectByType<MusicManager>();

        if (musicManager != null)
        {
            StartCoroutine(FadeOutAndDestroy(musicManager));
        }
    }

    private IEnumerator FadeOutAndDestroy(MusicManager musicManager)
    {
        AudioSource audioSource = musicManager.GetComponent<AudioSource>();

        if (audioSource != null)
        {
            float startVolume = audioSource.volume;
            float elapsed = 0f;

            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeOutDuration);
                yield return null;
            }

            audioSource.volume = 0f;
        }

        Destroy(musicManager.gameObject);
    }
}
