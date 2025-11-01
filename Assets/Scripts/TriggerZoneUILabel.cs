using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TriggerZoneUILabel : MonoBehaviour
{
    [Header("Label Settings")]
    public string labelText = "Stage Select";
    public Vector3 worldOffset = new Vector3(0, 1.5f, 0);
    public float fontSize = 36f;
    public Color textColor = Color.white;

    private Canvas canvas;
    private TextMeshProUGUI textUI;
    private GameObject labelObject;

    private void Start()
    {
        CreateUILabel();
    }

    private void CreateUILabel()
    {
        labelObject = new GameObject("TriggerZoneUILabel");

        canvas = labelObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 10;

        CanvasScaler scaler = labelObject.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10;

        labelObject.transform.SetParent(transform);
        labelObject.transform.localPosition = worldOffset;
        labelObject.transform.localRotation = Quaternion.identity;
        labelObject.transform.localScale = Vector3.one * 0.01f;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(labelObject.transform);

        textUI = textObj.AddComponent<TextMeshProUGUI>();
        textUI.text = labelText;
        textUI.fontSize = fontSize;
        textUI.color = textColor;
        textUI.alignment = TextAlignmentOptions.Center;

        RectTransform rectTransform = textUI.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(400, 100);
        rectTransform.anchoredPosition = Vector2.zero;
    }
}
