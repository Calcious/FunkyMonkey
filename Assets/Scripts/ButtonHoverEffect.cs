using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoverScale = 1.1f;
    public float animationSpeed = 5f;
    public bool changeCursor = true;

    private Vector3 originalScale;
    private Vector3 targetScale;

    private void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * animationSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;

        if (changeCursor && CursorManager.Instance != null)
        {
            CursorManager.Instance.SetHoverCursor();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;

        if (changeCursor && CursorManager.Instance != null)
        {
            CursorManager.Instance.SetDefaultCursor();
        }
    }
}
