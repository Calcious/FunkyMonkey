using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public string currentScene;
    public float saveTimestamp;

    public List<string> completedLevels = new List<string>();

    public List<string> acquiredEmblems = new List<string>();
    public string selectedEmblem;

    public CustomizationData customization = new CustomizationData();

    public GameSaveData()
    {
        saveTimestamp = Time.realtimeSinceStartup;
    }
}

[Serializable]
public class CustomizationData
{
    public List<MoveSlotData> moveSlots = new List<MoveSlotData>();
    public int availableSlotCircles;
}

[Serializable]
public class MoveSlotData
{
    public string slotName;
    public string assignedMoveName;
    public int slotCircleCount;

    public MoveSlotData(string name, string move, int circles)
    {
        slotName = name;
        assignedMoveName = move;
        slotCircleCount = circles;
    }
}
