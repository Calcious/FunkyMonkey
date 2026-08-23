using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// One-time tool: adds the idle-shimmer sparkle effect to the Main Menu background.
// Generates a soft radial glow sprite (a hard-edged circle would read as a plain
// dot, not a twinkle) and wires up a MainMenuSparkles container sitting above the
// background art but below the menu buttons in the Canvas hierarchy.
public static class MainMenuSparklesSetup
{
    private const string GlowSpritePath = "Assets/Sprites/UI/SparkleGlow.png";
    private const int GlowTextureSize = 64;

    [MenuItem("Funky Monkey/Add Main Menu Sparkles")]
    private static void Apply()
    {
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("Main Menu Sparkles", "Exit Play Mode before running this tool.", "OK");
            return;
        }

        Sprite glowSprite = CreateGlowSprite();
        AssetDatabase.Refresh();
        glowSprite = AssetDatabase.LoadAssetAtPath<Sprite>(GlowSpritePath);

        if (glowSprite == null)
        {
            EditorUtility.DisplayDialog("Main Menu Sparkles", "Glow sprite imported but could not be loaded - try running this again.", "OK");
            return;
        }

        Canvas canvas = FindCanvasInActiveScene();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("Main Menu Sparkles", "No Canvas found among the open scene's own root objects.", "OK");
            return;
        }

        Transform existing = canvas.transform.Find("Sparkles");
        GameObject sparklesGO;
        if (existing != null)
        {
            sparklesGO = existing.gameObject;
        }
        else
        {
            sparklesGO = new GameObject("Sparkles", typeof(RectTransform));
            Undo.RegisterCreatedObjectUndo(sparklesGO, "Add Main Menu Sparkles");
            sparklesGO.transform.SetParent(canvas.transform, false);
        }

        var rect = sparklesGO.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // Sits right after BackgroundArtwork (index 1) if present, so sparkles
        // render over the art but sibling order still puts the buttons on top.
        Transform artwork = canvas.transform.Find("BackgroundArtwork");
        int siblingIndex = artwork != null ? artwork.GetSiblingIndex() + 1 : 0;
        sparklesGO.transform.SetSiblingIndex(siblingIndex);

        var sparkles = sparklesGO.GetComponent<MainMenuSparkles>();
        if (sparkles == null)
        {
            sparkles = Undo.AddComponent<MainMenuSparkles>(sparklesGO);
        }

        Undo.RecordObject(sparkles, "Add Main Menu Sparkles");
        sparkles.sparkleSprite = glowSprite;
        // Re-syncs an already-existing instance to the script's current tuning
        // constants, so re-running this after adjusting count/size in code
        // actually applies the change instead of leaving the old serialized values.
        sparkles.sparkleCount = 54;
        sparkles.sizeRange = new Vector2(18f, 60f);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        string message = "Added the idle sparkle/shimmer effect above the background art.\n\nRemember to save the scene (Ctrl+S), then Play to preview it.";
        Debug.Log($"[MainMenuSparklesSetup] {message}");
        EditorUtility.DisplayDialog("Main Menu Sparkles", message, "OK");
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

    private static Sprite CreateGlowSprite()
    {
        var texture = new Texture2D(GlowTextureSize, GlowTextureSize, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2((GlowTextureSize - 1) * 0.5f, (GlowTextureSize - 1) * 0.5f);
        float maxDist = center.magnitude;

        for (int y = 0; y < GlowTextureSize; y++)
        {
            for (int x = 0; x < GlowTextureSize; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center) / maxDist;
                float alpha = Mathf.SmoothStep(1f, 0f, Mathf.Clamp01(dist));
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }
        texture.Apply();

        byte[] pngData = texture.EncodeToPNG();
        Object.DestroyImmediate(texture);
        Directory.CreateDirectory(Path.GetDirectoryName(GlowSpritePath));
        File.WriteAllBytes(GlowSpritePath, pngData);
        AssetDatabase.ImportAsset(GlowSpritePath);

        var importer = AssetImporter.GetAtPath(GlowSpritePath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(GlowSpritePath);
    }
}
