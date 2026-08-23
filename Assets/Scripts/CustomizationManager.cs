using UnityEngine;
using TMPro;

public class CustomizationManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI slotCounterText;
    public GameObject slotInventoryGrid;
    public GameObject slotCirclePrefab;

    [Header("Slot Management")]
    public int availableSlots = 0;

    private void Start()
    {
        LoadSlotCount();
        UpdateSlotCounter();
        PopulateSlotInventory();
    }

    private void LoadSlotCount()
    {
        int completedLevels = GetCompletedLevelCount();
        availableSlots = completedLevels;
    }

    private int GetCompletedLevelCount()
    {
        int count = 0;
        string[] levelNames = { "Level1", "Level2", "Level3" };

        foreach (string levelName in levelNames)
        {
            if (LevelCompletionManager.IsLevelCompleted(levelName))
            {
                count++;
            }
        }

        return count;
    }

    private void UpdateSlotCounter()
    {
        slotCounterText.text = $"Available Slots: {availableSlots}";
    }

    private void PopulateSlotInventory()
    {
        for (int i = 0; i < Mathf.Min(9, availableSlots); i++)
        {
            Transform existingSlot = slotInventoryGrid.transform.GetChild(i);
            if (existingSlot != null)
            {
                DraggableItem draggable = existingSlot.GetComponent<DraggableItem>();
                if (draggable == null)
                {
                    draggable = existingSlot.gameObject.AddComponent<DraggableItem>();
                }
                draggable.dragType = DraggableItem.DragType.Slot;
                draggable.itemName = $"Slot {i + 1}";
            }
        }
    }

    public void ConsumeSlot()
    {
        if (availableSlots > 0)
        {
            availableSlots--;
            UpdateSlotCounter();
        }
    }
}
