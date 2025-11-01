using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class UIDebugger : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.f1Key.wasPressedThisFrame)
        {
            DebugUIRaycasts();
        }
    }

    private void DebugUIRaycasts()
    {
        Debug.Log("=== UI RAYCAST DEBUG ===");

        EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("NO EVENTSYSTEM FOUND!");
            return;
        }
        Debug.Log($"EventSystem found: {eventSystem.gameObject.name} (enabled: {eventSystem.enabled})");

        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Mouse.current.position.ReadValue();

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        Debug.Log($"Mouse Position: {Mouse.current.position.ReadValue()}");
        Debug.Log($"Raycast hits: {results.Count}");

        foreach (RaycastResult result in results)
        {
            Debug.Log($"  Hit: {result.gameObject.name} (Canvas: {result.gameObject.GetComponentInParent<Canvas>()?.name})");
        }

        GraphicRaycaster[] raycasters = FindObjectsByType<GraphicRaycaster>(FindObjectsSortMode.None);
        Debug.Log($"\nTotal GraphicRaycasters in scene: {raycasters.Length}");
        foreach (var raycaster in raycasters)
        {
            Canvas canvas = raycaster.GetComponent<Canvas>();
            Debug.Log($"  Raycaster on: {raycaster.gameObject.name} (enabled: {raycaster.enabled}, sort order: {canvas?.sortingOrder})");
        }
    }
}
