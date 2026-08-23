using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// One-time tool: converts each LevelButton's full-tile "completed" slash overlay
// into a centered checkmark badge that fills most of the tile without spilling
// past its border (the original slash extended beyond the tile edges).
public static class CompletionBadgeConverter
{
    private static readonly Color BadgeColor = new Color(0.20f, 0.70f, 0.35f, 1f);
    private const float FillRatio = 0.8f; // fraction of the tile's shorter side the badge circle occupies

    [MenuItem("Funky Monkey/Convert Completion Slashes to Checkmark Badges")]
    private static void Convert()
    {
        var levelButtons = Object.FindObjectsByType<LevelButton>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int updated = 0;

        foreach (var levelButton in levelButtons)
        {
            if (levelButton.completionSlash == null || levelButton.slashImage == null)
            {
                Debug.LogWarning($"[CompletionBadgeConverter] Skipping '{levelButton.name}' - missing completionSlash/slashImage reference.", levelButton);
                continue;
            }

            var badgeGO = levelButton.completionSlash;
            var badgeRect = badgeGO.GetComponent<RectTransform>();
            var parentRect = badgeGO.transform.parent as RectTransform;

            if (parentRect == null)
            {
                Debug.LogWarning($"[CompletionBadgeConverter] Skipping '{levelButton.name}' - completionSlash has no RectTransform parent.", levelButton);
                continue;
            }

            Undo.RecordObject(badgeRect, "Convert Completion Badge");
            float side = FillRatio * Mathf.Min(parentRect.rect.width, parentRect.rect.height);
            badgeRect.anchorMin = new Vector2(0.5f, 0.5f);
            badgeRect.anchorMax = new Vector2(0.5f, 0.5f);
            badgeRect.pivot = new Vector2(0.5f, 0.5f);
            badgeRect.sizeDelta = new Vector2(side, side);
            badgeRect.anchoredPosition = Vector2.zero;

            Undo.RecordObject(levelButton.slashImage, "Convert Completion Badge");
            levelButton.slashImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            levelButton.slashImage.type = Image.Type.Simple;
            levelButton.slashImage.color = BadgeColor;

            Undo.RecordObject(levelButton, "Convert Completion Badge");
            levelButton.slashColor = BadgeColor;

            var checkText = badgeGO.transform.Find("CheckmarkText");
            TextMeshProUGUI tmp;
            if (checkText == null)
            {
                var textGO = new GameObject("CheckmarkText", typeof(RectTransform));
                Undo.RegisterCreatedObjectUndo(textGO, "Convert Completion Badge");
                textGO.transform.SetParent(badgeGO.transform, false);

                var textRect = textGO.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;

                tmp = textGO.AddComponent<TextMeshProUGUI>();
            }
            else
            {
                tmp = checkText.GetComponent<TextMeshProUGUI>();
            }

            tmp.text = "✓";
            tmp.enableAutoSizing = true;
            tmp.fontSizeMax = 200;
            tmp.fontSizeMin = 10;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;
            tmp.color = Color.white;

            EditorUtility.SetDirty(badgeGO);
            updated++;
        }

        if (updated > 0)
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        string message = $"Updated {updated} level button(s) in scene '{EditorSceneManager.GetActiveScene().name}'.\n\nRemember to save the scene (Ctrl+S) for this to stick.";
        Debug.Log($"[CompletionBadgeConverter] {message}");
        EditorUtility.DisplayDialog("Completion Badge Converter", message, "OK");
    }
}
