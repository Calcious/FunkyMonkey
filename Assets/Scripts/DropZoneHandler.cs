using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DropZoneHandler : MonoBehaviour
{
    public string slotName;
    public TextMeshProUGUI moveNameText;
    public Transform slotCircleContainer;
    public int maxSlots = 4;

    private DraggableItem currentMove;
    private List<DraggableItem> currentSlots = new List<DraggableItem>();

    public bool CanAccept(DraggableItem.DragType dragType)
    {
        if (dragType == DraggableItem.DragType.Move)
        {
            return true;
        }
        else if (dragType == DraggableItem.DragType.Slot)
        {
            return currentSlots.Count < maxSlots;
        }
        return false;
    }

    public void AcceptItem(DraggableItem item)
    {
        if (item.dragType == DraggableItem.DragType.Move)
        {
            if (currentMove != null)
            {
                Destroy(currentMove.gameObject);
            }

            currentMove = item;
            moveNameText.text = item.itemName;
        }
        else if (item.dragType == DraggableItem.DragType.Slot)
        {
            if (currentSlots.Count < maxSlots)
            {
                currentSlots.Add(item);
                item.transform.SetParent(slotCircleContainer, false);
            }
        }
    }

    public void RemoveSlot(DraggableItem slot)
    {
        currentSlots.Remove(slot);
    }
}
