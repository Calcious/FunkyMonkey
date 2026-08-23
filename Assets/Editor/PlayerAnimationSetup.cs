using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

// One-time tool: slices the Idle/Run/Jump sprite sheets, builds an
// AnimationClip for each, and wires them into a shared Animator Controller
// for the player character (Speed float + Grounded bool drive the states).
public static class PlayerAnimationSetup
{
    private const string SpriteDir = "Assets/Sprites/Player";
    private const string AnimDir = "Assets/Animations/Player";
    private const int FrameSize = 32;
    private const float FrameDuration = 0.1f; // 100ms/frame, matches the source .aseprite timing

    private class AnimJob
    {
        public string SheetName;
        public int FrameCount;
        public string ClipName;
        public bool Loop;
    }

    private static readonly AnimJob[] Jobs =
    {
        new AnimJob { SheetName = "FunkyMonkeyIdle_sheet", FrameCount = 10, ClipName = "Funky_Idle", Loop = true },
        new AnimJob { SheetName = "FunkyMonkeyRun_sheet", FrameCount = 4, ClipName = "Funky_Run", Loop = true },
        new AnimJob { SheetName = "FunkyMonkeyJump_sheet", FrameCount = 8, ClipName = "Funky_Jump", Loop = false },
        new AnimJob { SheetName = "FunkyMonkeyAttack_sheet", FrameCount = 5, ClipName = "Funky_Attack", Loop = false },
    };

    [MenuItem("Funky Monkey/Build Player Animations")]
    private static void Build()
    {
        Directory.CreateDirectory(AnimDir);

        var clips = new Dictionary<string, AnimationClip>();

        foreach (var job in Jobs)
        {
            string pngPath = $"{SpriteDir}/{job.SheetName}.png";
            if (!File.Exists(pngPath))
            {
                Debug.LogError($"[PlayerAnimationSetup] Missing {pngPath}");
                continue;
            }

            var sprites = SliceSheet(pngPath, job.FrameCount);
            var clip = BuildClip(job.ClipName, sprites, job.Loop);
            clips[job.ClipName] = clip;
        }

        BuildController(clips);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Player Animations", "Built Idle/Run/Jump animation clips and Player.controller in " + AnimDir + ".\n\nNext: run 'Funky Monkey > Wire Player Animator In Scenes'.", "OK");
    }

    private static Sprite[] SliceSheet(string pngPath, int frameCount)
    {
        var importer = (TextureImporter)AssetImporter.GetAtPath(pngPath);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.spritePixelsPerUnit = 32;

        var metas = new List<SpriteMetaData>();
        for (int i = 0; i < frameCount; i++)
        {
            metas.Add(new SpriteMetaData
            {
                name = $"{Path.GetFileNameWithoutExtension(pngPath)}_{i}",
                rect = new Rect(i * FrameSize, 0, FrameSize, FrameSize),
                pivot = new Vector2(0.5f, 0.5f),
                alignment = (int)SpriteAlignment.Center,
            });
        }
        importer.spritesheet = metas.ToArray();
        importer.SaveAndReimport();

        var assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(pngPath);
        var sprites = new Sprite[frameCount];
        foreach (var asset in assets)
        {
            if (asset is Sprite sprite)
            {
                string suffix = sprite.name.Substring(sprite.name.LastIndexOf('_') + 1);
                if (int.TryParse(suffix, out int index) && index < frameCount)
                {
                    sprites[index] = sprite;
                }
            }
        }
        return sprites;
    }

    private static AnimationClip BuildClip(string clipName, Sprite[] sprites, bool loop)
    {
        string path = $"{AnimDir}/{clipName}.anim";
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        if (clip == null)
        {
            clip = new AnimationClip();
            AssetDatabase.CreateAsset(clip, path);
        }
        else
        {
            clip.ClearCurves();
        }

        clip.frameRate = 1f / FrameDuration;

        var keyframes = new ObjectReferenceKeyframe[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            keyframes[i] = new ObjectReferenceKeyframe
            {
                time = i * FrameDuration,
                value = sprites[i],
            };
        }

        var binding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite",
        };
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);

        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = loop;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        EditorUtility.SetDirty(clip);
        return clip;
    }

    private static void BuildController(Dictionary<string, AnimationClip> clips)
    {
        string path = $"{AnimDir}/Player.controller";
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
        if (controller == null)
        {
            controller = AnimatorController.CreateAnimatorControllerAtPath(path);
        }

        if (System.Array.FindIndex(controller.parameters, p => p.name == "Speed") < 0)
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        if (System.Array.FindIndex(controller.parameters, p => p.name == "Grounded") < 0)
            controller.AddParameter("Grounded", AnimatorControllerParameterType.Bool);
        if (System.Array.FindIndex(controller.parameters, p => p.name == "Attack") < 0)
            controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);

        var rootStateMachine = controller.layers[0].stateMachine;

        AnimatorState GetOrCreateState(string name, AnimationClip clip)
        {
            foreach (var s in rootStateMachine.states)
            {
                if (s.state.name == name)
                {
                    s.state.motion = clip;
                    return s.state;
                }
            }
            var state = rootStateMachine.AddState(name);
            state.motion = clip;
            return state;
        }

        var idleState = GetOrCreateState("Idle", clips["Funky_Idle"]);
        var runState = GetOrCreateState("Run", clips["Funky_Run"]);
        var jumpState = GetOrCreateState("Jump", clips["Funky_Jump"]);
        var attackState = GetOrCreateState("Attack", clips["Funky_Attack"]);

        rootStateMachine.defaultState = idleState;

        void EnsureTransition(AnimatorState from, AnimatorState to, System.Action<AnimatorStateTransition> configure)
        {
            foreach (var t in from.transitions)
            {
                if (t.destinationState == to) return; // already wired
            }
            var transition = from.AddTransition(to);
            transition.hasExitTime = false;
            transition.duration = 0.05f;
            configure(transition);
        }

        EnsureTransition(idleState, runState, t => t.AddCondition(AnimatorConditionMode.Greater, 0.05f, "Speed"));
        EnsureTransition(runState, idleState, t => t.AddCondition(AnimatorConditionMode.Less, 0.05f, "Speed"));

        EnsureTransition(idleState, jumpState, t => t.AddCondition(AnimatorConditionMode.IfNot, 0, "Grounded"));
        EnsureTransition(runState, jumpState, t => t.AddCondition(AnimatorConditionMode.IfNot, 0, "Grounded"));
        EnsureTransition(jumpState, idleState, t => t.AddCondition(AnimatorConditionMode.If, 0, "Grounded"));

        // Attack interrupts whatever's playing (including a repeat mid-swing while
        // the mouse is held, since PlayerMovement re-fires the trigger), then
        // returns to Idle/Run once the swing finishes playing out.
        bool hasAnyStateToAttack = false;
        foreach (var t in rootStateMachine.anyStateTransitions)
        {
            if (t.destinationState == attackState) hasAnyStateToAttack = true;
        }
        if (!hasAnyStateToAttack)
        {
            var anyToAttack = rootStateMachine.AddAnyStateTransition(attackState);
            anyToAttack.hasExitTime = false;
            anyToAttack.duration = 0.02f;
            anyToAttack.canTransitionToSelf = true;
            anyToAttack.AddCondition(AnimatorConditionMode.If, 0, "Attack");
        }

        EnsureTransition(attackState, idleState, t =>
        {
            t.hasExitTime = true;
            t.exitTime = 0.9f;
        });

        EditorUtility.SetDirty(controller);
    }
}
