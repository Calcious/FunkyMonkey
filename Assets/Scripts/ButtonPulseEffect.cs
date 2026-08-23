using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonPulseEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Pulse Settings")]
    public float pulseSpeed = 2f;
    public float pulseScale = 1.15f;

    [Header("Hover Settings")]
    public float hoverFontSizeMultiplier = 1.1f;

    private Vector3 originalScale;
    private float pulseTimer;
    private bool isHovering;

    private TextMeshProUGUI label;
    private float originalFontSize;

    private void Awake()
    {
        originalScale = transform.localScale;
        label = GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
        {
            originalFontSize = label.fontSize;
        }
    }

    private void Update()
    {
        if (isHovering) return;

        pulseTimer += Time.deltaTime * pulseSpeed;
        float scaleFactor = 1f + (Mathf.Sin(pulseTimer) * 0.5f + 0.5f) * (pulseScale - 1f);
        transform.localScale = originalScale * scaleFactor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        transform.localScale = originalScale;

        if (label != null)
        {
            label.fontSize = originalFontSize * hoverFontSizeMultiplier;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;

        if (label != null)
        {
            label.fontSize = originalFontSize;
        }
    }

    private void OnDisable()
    {
        transform.localScale = originalScale;

        if (label != null)
        {
            label.fontSize = originalFontSize;
        }
    }
}
