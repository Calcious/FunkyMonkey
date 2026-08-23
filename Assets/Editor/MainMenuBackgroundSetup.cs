using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// One-time tool: applies the new key-art background to the Main Menu.
// The art is a 1024x1024 square; the game renders at 1920x1080 (16:9), so it
// can't fill the frame without either cropping content or distorting it.
// Instead this fits the art to the screen height (a ~5% upscale - effectively
// lossless) and fills the leftover left/right strips with a generated
// gradient that approximates the artwork's palette, so there's no hard edge.
public static class MainMenuBackgroundSetup
{
    private const string ArtworkPath = "Assets/Sprites/UI/MainMenuBackground.png";
    private const string GradientPath = "Assets/Sprites/UI/MainMenuBackgroundGradient.png";

    // Neon gradient stops (bottom to top), sampled by eye from the artwork's own palette.
    private static readonly Color[] GradientStops =
    {
        new Color(0.02f, 0.02f, 0.03f),   // near-black, ground level
        new Color(0.35f, 0.22f, 0.05f),   // warm gold glow (hat/jewelry)
        new Color(0.05f, 0.28f, 0.26f),   // teal (jacket/guitar)
        new Color(0.45f, 0.07f, 0.30f),   // magenta/pink (title)
    };

    // Fraction of the unavoidable vertical crop taken from the top vs. the bottom
    // (0.5 = centered, 0 = keep the full top edge, crop only from the bottom).
    // Zero so the "FUNKY MONKEY" logo text at the very top is never clipped.
    private const float TopCropBias = 0f;

    private static readonly Color ButtonTextColor = new Color32(0xFF, 0xC9, 0x4D, 0xFF); // warm gold, matches established accent palette
    private static readonly Color ButtonOutlineColor = new Color(0f, 0f, 0f, 0.85f);
    private const float ButtonFontSizeMultiplier = 1.3f;

    [MenuItem("Funky Monkey/Apply Main Menu Background")]
    private static void Apply()
    {
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("Main Menu Background", "Exit Play Mode before running this tool - running it during Play can attach the background to a temporary runtime object instead of the real scene Canvas.", "OK");
            return;
        }

        if (!File.Exists(ArtworkPath))
        {
            EditorUtility.DisplayDialog("Main Menu Background", $"Couldn't find {ArtworkPath}. Copy the artwork there first.", "OK");
            return;
        }

        ConfigureArtworkImportSettings();
        Sprite gradientSprite = CreateGradientSprite();
        AssetDatabase.Refresh();
        Sprite artworkSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ArtworkPath);

        if (artworkSprite == null)
        {
            EditorUtility.DisplayDialog("Main Menu Background", "Artwork imported but the Sprite could not be loaded - try running this again.", "OK");
            return;
        }

        Canvas canvas = FindCanvasInActiveScene();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("Main Menu Background", "No Canvas found among the open scene's own root objects.", "OK");
            return;
        }

        RemoveStrayLayer("BackgroundGradient", canvas.transform);
        RemoveStrayLayer("BackgroundArtwork", canvas.transform);

        GameObject gradientGO = CreateOrGetLayer(canvas.transform, "BackgroundGradient", 0);
        var gradientImage = gradientGO.GetComponent<Image>();
        Undo.RecordObject(gradientImage, "Apply Main Menu Background");
        gradientImage.sprite = gradientSprite;
        gradientImage.type = Image.Type.Simple;
        gradientImage.preserveAspect = false;
        gradientImage.color = Color.white;

        GameObject artworkGO = CreateOrGetLayer(canvas.transform, "BackgroundArtwork", 1);
        var artworkImage = artworkGO.GetComponent<Image>();
        Undo.RecordObject(artworkImage, "Apply Main Menu Background");
        artworkImage.sprite = artworkSprite;
        artworkImage.type = Image.Type.Simple;
        artworkImage.preserveAspect = true;
        artworkImage.color = Color.white;

        // Scale the fit rect so its aspect ratio exactly matches the (square) artwork's -
        // with preserveAspect on, that makes the image fill the rect with zero gaps.
        // Since the rect is wider than the screen's aspect, this fills the screen width
        // completely (uniform scale, no distortion) at the cost of cropping top/bottom.
        var canvasRect = canvas.GetComponent<RectTransform>();
        var artworkRect = artworkGO.GetComponent<RectTransform>();
        Undo.RecordObject(artworkRect, "Apply Main Menu Background");

        float artworkAspect = (float)artworkSprite.rect.width / artworkSprite.rect.height;
        float targetHeight = canvasRect.rect.width / artworkAspect; // rect height needed for a square fit to span full width
        float verticalOverflow = targetHeight - canvasRect.rect.height;

        artworkRect.offsetMin = new Vector2(0f, -verticalOverflow * (1f - TopCropBias));
        artworkRect.offsetMax = new Vector2(0f, verticalOverflow * TopCropBias);

        HideRedundantTitleText(canvas.transform);
        RestyleMenuButtons();
        MakeButtonBackgroundsTransparent();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        string message = "Applied background art + gradient fill, hid the old plain title text, and restyled the menu button labels.\n\nRemember to save the scene (Ctrl+S).";
        Debug.Log($"[MainMenuBackgroundSetup] {message}");
        EditorUtility.DisplayDialog("Main Menu Background", message, "OK");
    }

    private static void RemoveStrayLayer(string name, Transform correctParent)
    {
        var activeScene = EditorSceneManager.GetActiveScene();
        foreach (GameObject root in activeScene.GetRootGameObjects())
        {
            if (root.name == name && root.transform.parent == null)
            {
                Debug.Log($"[MainMenuBackgroundSetup] Removing stray root-level '{name}' left over from a previous run.");
                Undo.DestroyObjectImmediate(root);
                continue;
            }

            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child != null && child.name == name && child.parent != correctParent)
                {
                    Debug.Log($"[MainMenuBackgroundSetup] Removing stray '{name}' parented under '{(child.parent != null ? child.parent.name : "null")}' from a previous run.");
                    Undo.DestroyObjectImmediate(child.gameObject);
                }
            }
        }
    }

    private static Canvas FindCanvasInActiveScene()
    {
        var activeScene = EditorSceneManager.GetActiveScene();
        foreach (GameObject root in activeScene.GetRootGameObjects())
        {
            Canvas canvas = root.GetComponentInChildren<Canvas>(true);
            if (canvas != null) return canvas;
        }
        return null;
    }

    private static void ConfigureArtworkImportSettings()
    {
        var importer = AssetImporter.GetAtPath(ArtworkPath) as TextureImporter;
        if (importer == null) return;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.mipmapEnabled = false;
        importer.maxTextureSize = 2048;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        importer.SaveAndReimport();
    }

    private static Color EvaluateGradient(float t)
    {
        float scaled = t * (GradientStops.Length - 1);
        int index = Mathf.Clamp(Mathf.FloorToInt(scaled), 0, GradientStops.Length - 2);
        float localT = scaled - index;
        return Color.Lerp(GradientStops[index], GradientStops[index + 1], localT);
    }

    private static Sprite CreateGradientSprite()
    {
        const int height = 256;
        var texture = new Texture2D(1, height, TextureFormat.RGBA32, false);
        for (int y = 0; y < height; y++)
        {
            float t = y / (float)(height - 1);
            texture.SetPixel(0, y, EvaluateGradient(t));
        }
        texture.Apply();

        byte[] pngData = texture.EncodeToPNG();
        Object.DestroyImmediate(texture);
        File.WriteAllBytes(GradientPath, pngData);
        AssetDatabase.ImportAsset(GradientPath);

        var importer = AssetImporter.GetAtPath(GradientPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(GradientPath);
    }

    private static GameObject CreateOrGetLayer(Transform canvasTransform, string name, int siblingIndex)
    {
        Transform existing = canvasTransform.Find(name);
        GameObject go;
        if (existing != null)
        {
            go = existing.gameObject;
        }
        else
        {
            go = new GameObject(name, typeof(RectTransform), typeof(Image));
            Undo.RegisterCreatedObjectUndo(go, "Apply Main Menu Background");
            go.transform.SetParent(canvasTransform, false);
        }

        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        go.transform.SetSiblingIndex(siblingIndex);
        go.GetComponent<Image>().raycastTarget = false;
        return go;
    }

    private static void HideRedundantTitleText(Transform canvasTransform)
    {
        Transform titleText = canvasTransform.Find("TitleText");
        if (titleText != null)
        {
            Undo.RecordObject(titleText.gameObject, "Apply Main Menu Background");
            titleText.gameObject.SetActive(false);
            Debug.Log("[MainMenuBackgroundSetup] Hid the old plain TitleText - the new artwork has its own title baked in.");
        }
    }

    private static void RestyleMenuButtons()
    {
        var mainMenuUI = Object.FindFirstObjectByType<MainMenuUI>();
        if (mainMenuUI == null || mainMenuUI.menuButtonsToHideOnStart == null)
        {
            Debug.LogWarning("[MainMenuBackgroundSetup] Could not find MainMenuUI's button list - skipping button restyle.");
            return;
        }

        foreach (var buttonGO in mainMenuUI.menuButtonsToHideOnStart)
        {
            if (buttonGO == null) continue;
            RestyleButtonText(buttonGO);
        }

        if (mainMenuUI.continueButton != null)
        {
            RestyleButtonText(mainMenuUI.continueButton.gameObject);
        }
    }

    private static void MakeButtonBackgroundsTransparent()
    {
        var mainMenuUI = Object.FindFirstObjectByType<MainMenuUI>();
        if (mainMenuUI == null || mainMenuUI.menuButtonsToHideOnStart == null) return;

        foreach (var buttonGO in mainMenuUI.menuButtonsToHideOnStart)
        {
            if (buttonGO == null) continue;
            var image = buttonGO.GetComponent<Image>();
            if (image == null) continue;

            Undo.RecordObject(image, "Apply Main Menu Background");
            Color c = image.color;
            image.color = new Color(c.r, c.g, c.b, 0f);
        }
    }

    private static void RestyleButtonText(GameObject buttonGO)
    {
        var tmp = buttonGO.GetComponentInChildren<TextMeshProUGUI>(true);
        if (tmp == null) return;

        var outline = tmp.GetComponent<Outline>();
        bool alreadyStyled = outline != null;

        Undo.RecordObject(tmp, "Apply Main Menu Background");
        tmp.color = ButtonTextColor;
        tmp.fontStyle = FontStyles.Bold;
        if (!alreadyStyled)
        {
            tmp.fontSize *= ButtonFontSizeMultiplier;
        }

        if (outline == null)
        {
            outline = Undo.AddComponent<Outline>(tmp.gameObject);
        }
        outline.effectColor = ButtonOutlineColor;
        outline.effectDistance = new Vector2(1.5f, -1.5f);
    }
}
