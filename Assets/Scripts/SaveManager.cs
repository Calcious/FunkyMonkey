using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class SaveManager : MonoBehaviour
{
    private static SaveManager instance;
    public static SaveManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("SaveManager");
                instance = go.AddComponent<SaveManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    private const string SAVE_FILE_NAME = "gamesave.json";
    private const string HAS_SAVE_KEY = "HasSaveData";

    private string SaveFilePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static bool HasSaveData()
    {
        return PlayerPrefs.GetInt(HAS_SAVE_KEY, 0) == 1 && File.Exists(Instance.SaveFilePath);
    }

    public void SaveGame()
    {
        GameSaveData saveData = new GameSaveData();

        saveData.currentScene = SceneManager.GetActiveScene().name;
        saveData.saveTimestamp = Time.realtimeSinceStartup;

        SaveCompletedLevels(saveData);
        SaveEmblems(saveData);
        SaveCustomization(saveData);

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SaveFilePath, json);

        PlayerPrefs.SetInt(HAS_SAVE_KEY, 1);
        PlayerPrefs.Save();

        Debug.Log($"Game saved to: {SaveFilePath}");
    }

    public GameSaveData LoadGame()
    {
        if (!File.Exists(SaveFilePath))
        {
            Debug.LogWarning("No save file found!");
            return null;
        }

        string json = File.ReadAllText(SaveFilePath);
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

        RestoreCompletedLevels(saveData);
        RestoreEmblems(saveData);

        Debug.Log($"Game loaded from: {SaveFilePath}");
        return saveData;
    }

    public void LoadAndApplyGame()
    {
        GameSaveData saveData = LoadGame();
        if (saveData != null)
        {
            SceneManager.LoadScene(saveData.currentScene);
        }
    }

    public void DeleteSave()
    {
        if (File.Exists(SaveFilePath))
        {
            File.Delete(SaveFilePath);
        }
        PlayerPrefs.DeleteKey(HAS_SAVE_KEY);
        PlayerPrefs.Save();
        Debug.Log("Save data deleted!");
    }

    private void SaveCompletedLevels(GameSaveData saveData)
    {
        string[] allLevels = { "Metal", "Pop", "Emo", "Punk", "Grunge", "Synth", "Dub", "FunkyFinal", "Level1", "Level2", "Level3" };

        foreach (string level in allLevels)
        {
            if (LevelCompletionManager.IsLevelCompleted(level))
            {
                saveData.completedLevels.Add(level);
            }
        }
    }

    private void RestoreCompletedLevels(GameSaveData saveData)
    {
        LevelCompletionManager.ClearAllCompletions();

        foreach (string level in saveData.completedLevels)
        {
            LevelCompletionManager.MarkLevelAsCompleted(level);
        }
    }

    private void SaveEmblems(GameSaveData saveData)
    {
        saveData.selectedEmblem = PlayerPrefs.GetString("SelectedEmblem", "");

        string[] possibleEmblems = { "Emblem1", "Emblem2", "Emblem3", "Emblem4", "Emblem5" };
        foreach (string emblem in possibleEmblems)
        {
            if (PlayerPrefs.GetInt($"Emblem_{emblem}", 0) == 1)
            {
                saveData.acquiredEmblems.Add(emblem);
            }
        }
    }

    private void RestoreEmblems(GameSaveData saveData)
    {
        PlayerPrefs.SetString("SelectedEmblem", saveData.selectedEmblem);

        foreach (string emblem in saveData.acquiredEmblems)
        {
            PlayerPrefs.SetInt($"Emblem_{emblem}", 1);
        }
    }

    private void SaveCustomization(GameSaveData saveData)
    {
        DropZoneHandler[] dropZones = FindObjectsOfType<DropZoneHandler>();

        foreach (DropZoneHandler zone in dropZones)
        {
            string moveName = zone.moveNameText != null ? zone.moveNameText.text : "";
            int circleCount = zone.slotCircleContainer != null ? zone.slotCircleContainer.childCount : 0;

            MoveSlotData slotData = new MoveSlotData(zone.slotName, moveName, circleCount);
            saveData.customization.moveSlots.Add(slotData);
        }

        CustomizationManager customManager = FindObjectOfType<CustomizationManager>();
        if (customManager != null)
        {
            saveData.customization.availableSlotCircles = customManager.availableSlots;
        }
    }
}
