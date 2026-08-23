using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;

// One-time tool: finds the "Player" GameObject in every gameplay scene and
// attaches the shared Player.controller Animator, so Idle/Run/Jump actually
// play in-game. Run 'Funky Monkey > Build Player Animations' first.
public static class PlayerAnimatorSceneWiring
{
    private const string ControllerPath = "Assets/Animations/Player/Player.controller";

    private static readonly string[] ScenePaths =
    {
        "Assets/Scenes/SampleScene.unity",
        "Assets/Scenes/Hub.unity",
        "Assets/Scenes/Level1.unity",
        "Assets/Scenes/Level2.unity",
        "Assets/Scenes/Synth.unity",
        "Assets/Scenes/Metal.unity",
        "Assets/Scenes/Pop.unity",
        "Assets/Scenes/Punk.unity",
        "Assets/Scenes/Grunge.unity",
        "Assets/Scenes/Emo.unity",
        "Assets/Scenes/Rap.unity",
        "Assets/Scenes/Dub.unity",
        "Assets/Scenes/FunkyFinal.unity",
    };

    [MenuItem("Funky Monkey/Wire Player Animator In Scenes")]
    private static void Wire()
    {
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("Wire Player Animator", "Exit Play Mode before running this tool.", "OK");
            return;
        }

        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
        if (controller == null)
        {
            EditorUtility.DisplayDialog("Wire Player Animator", $"Couldn't find {ControllerPath}. Run 'Build Player Animations' first.", "OK");
            return;
        }

        string activeScenePath = EditorSceneManager.GetActiveScene().path;
        if (!string.IsNullOrEmpty(activeScenePath) && EditorSceneManager.GetActiveScene().isDirty)
        {
            EditorUtility.DisplayDialog("Wire Player Animator", "You have unsaved changes in the current scene. Save or discard them first so nothing gets lost.", "OK");
            return;
        }

        int wired = 0;
        int skipped = 0;

        foreach (string scenePath in ScenePaths)
        {
            if (!File.Exists(scenePath))
            {
                Debug.LogWarning($"[PlayerAnimatorSceneWiring] Scene not found, skipping: {scenePath}");
                skipped++;
                continue;
            }

            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            GameObject player = GameObject.Find("Player");

            if (player == null || player.GetComponent<PlayerMovement>() == null)
            {
                Debug.LogWarning($"[PlayerAnimatorSceneWiring] No Player+PlayerMovement found in {scenePath}, skipping.");
                skipped++;
                continue;
            }

            var animator = player.GetComponent<Animator>();
            if (animator == null)
            {
                animator = player.AddComponent<Animator>();
            }
            animator.runtimeAnimatorController = controller;

            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }

            EditorUtility.SetDirty(player);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            wired++;
        }

        string message = $"Wired the Animator onto Player in {wired} scene(s).";
        if (skipped > 0) message += $" Skipped {skipped} (see Console for details).";
        Debug.Log($"[PlayerAnimatorSceneWiring] {message}");
        EditorUtility.DisplayDialog("Wire Player Animator", message, "OK");
    }
}
