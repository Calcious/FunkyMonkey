using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// One-time tool: fixes the "Hold to Skip" UI two ways -
// 1) nudges the whole group right so the text stops spilling past the left
//    edge of the screen, and
// 2) converts the fill bar into a circular radial (pie-chart style) progress
//    indicator - the fill's FillMethod was already set to Radial360, but
//    Image.Type was left on Simple, so fillAmount was never actually visible.
public static class SkipUIFixer
{
    private static readonly Sprite CircleSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
    private const float CircleSize = 56f;

    [MenuItem("Funky Monkey/Fix Skip UI (Position + Radial Fill)")]
    private static void Fix()
    {
        var skipController = Object.FindFirstObjectByType<SkipController>(FindObjectsInactive.Include);
        if (skipController == null)
        {
            EditorUtility.DisplayDialog("Skip UI Fixer", "No SkipController found in the open scene.", "OK");
            return;
        }

        if (skipController.skipUI == null || skipController.skipBarFill == null)
        {
            EditorUtility.DisplayDialog("Skip UI Fixer", "SkipController is missing its skipUI/skipBarFill references.", "OK");
            return;
        }

        // 1) Fix off-screen positioning
        var skipUIRect = skipController.skipUI.GetComponent<RectTransform>();
        Undo.RecordObject(skipUIRect, "Fix Skip UI");
        skipUIRect.anchoredPosition = new Vector2(140f, 40f);

        // 2) Convert bar to a circular radial fill
        var fillImage = skipController.skipBarFill;
        var fillRect = fillImage.GetComponent<RectTransform>();
        var backgroundTransform = fillRect.parent;
        var backgroundImage = backgroundTransform != null ? backgroundTransform.GetComponent<Image>() : null;
        var backgroundRect = backgroundTransform as RectTransform;

        if (backgroundImage != null && backgroundRect != null)
        {
            Undo.RecordObject(backgroundRect, "Fix Skip UI");
            backgroundRect.sizeDelta = new Vector2(CircleSize, CircleSize);
            backgroundRect.anchoredPosition = new Vector2(70f, 0f);

            Undo.RecordObject(backgroundImage, "Fix Skip UI");
            backgroundImage.sprite = CircleSprite;
            backgroundImage.type = Image.Type.Simple;
        }
        else
        {
            Debug.LogWarning("[SkipUIFixer] Could not find skipBarFill's parent background Image - skipping background resize.");
        }

        Undo.RecordObject(fillRect, "Fix Skip UI");
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        fillRect.anchoredPosition = Vector2.zero;

        Undo.RecordObject(fillImage, "Fix Skip UI");
        fillImage.sprite = CircleSprite;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Radial360;

        EditorUtility.SetDirty(skipUIRect);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        string message = "Repositioned Skip UI and converted the fill bar to a circular radial indicator.\n\nRemember to save the scene (Ctrl+S).";
        Debug.Log($"[SkipUIFixer] {message}");
        EditorUtility.DisplayDialog("Skip UI Fixer", message, "OK");
    }
}
