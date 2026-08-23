using UnityEngine;
using TMPro;
using System.Collections;

public class SaveNotificationUI : MonoBehaviour
{
    private static SaveNotificationUI instance;
    public static SaveNotificationUI Instance => instance;

    [Header("UI References")]
    public TextMeshProUGUI notificationText;
    public CanvasGroup canvasGroup;

    [Header("Animation Settings")]
    public float fadeInDuration = 0.3f;
    public float displayDuration = 2f;
    public float fadeOutDuration = 0.5f;

    private Coroutine currentNotification;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
    }

    public void ShowNotification(string message)
    {
        if (currentNotification != null)
        {
            StopCoroutine(currentNotification);
        }

        currentNotification = StartCoroutine(ShowNotificationCoroutine(message));
    }

    private IEnumerator ShowNotificationCoroutine(string message)
    {
        notificationText.text = message;

        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(displayDuration);

        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
