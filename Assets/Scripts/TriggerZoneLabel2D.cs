using UnityEngine;
using TMPro;

public class TriggerZoneLabel2D : MonoBehaviour
{
    [Header("Label Settings")]
    public string labelText = "Stage Select";
    public Vector3 labelOffset = new Vector3(0, 0.8f, 0);

    [Header("Text Appearance")]
    public float fontSize = 2f;
    public float textWidth = 20f;
    public float textHeight = 5f;
    public Color textColor = Color.white;
    public Color outlineColor = Color.black;
    public float outlineWidth = 0.2f;

    [Header("Auto Sizing")]
    public bool enableAutoSizing = true;
    public float fontSizeMin = 1f;
    public float fontSizeMax = 4f;

    private GameObject labelObject;
    private TextMeshPro textMesh;

    private void Start()
    {
        CreateLabel();
    }

    private void CreateLabel()
    {
        labelObject = new GameObject("Label");
        labelObject.transform.SetParent(transform);
        labelObject.transform.localPosition = labelOffset;
        labelObject.transform.localRotation = Quaternion.identity;
        labelObject.transform.localScale = Vector3.one;

        textMesh = labelObject.AddComponent<TextMeshPro>();
        textMesh.text = labelText;
        textMesh.fontSize = fontSize;
        textMesh.color = textColor;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.sortingOrder = 100;

        textMesh.fontStyle = FontStyles.Bold;
        textMesh.outlineWidth = outlineWidth;
        textMesh.outlineColor = outlineColor;

        if (enableAutoSizing)
        {
            textMesh.enableAutoSizing = true;
            textMesh.fontSizeMin = fontSizeMin;
            textMesh.fontSizeMax = fontSizeMax;
        }

        RectTransform rectTransform = textMesh.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(textWidth, textHeight);
        }
    }

    private void OnDestroy()
    {
        if (labelObject != null)
        {
            Destroy(labelObject);
        }
    }
}
