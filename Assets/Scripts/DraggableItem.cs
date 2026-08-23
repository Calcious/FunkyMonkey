using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum DragType { Move, Slot }
    public DragType dragType;
    public string itemName;

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector2 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;

        transform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling();

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        bool droppedSuccessfully = false;

        if (eventData.pointerEnter != null)
        {
            DropZoneHandler dropZone = eventData.pointerEnter.GetComponent<DropZoneHandler>();
            if (dropZone == null)
            {
                dropZone = eventData.pointerEnter.GetComponentInParent<DropZoneHandler>();
            }

            if (dropZone != null && dropZone.CanAccept(dragType))
            {
                dropZone.AcceptItem(this);
                droppedSuccessfully = true;
            }
        }

        if (!droppedSuccessfully)
        {
            transform.SetParent(originalParent, false);
            rectTransform.anchoredPosition = originalPosition;
        }
    }
}
