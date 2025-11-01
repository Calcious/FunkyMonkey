using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [Header("Cursor Textures")]
    public Texture2D defaultCursor;
    public Texture2D hoverCursor;

    [Header("Cursor Settings")]
    public Vector2 defaultHotspot = Vector2.zero;
    public Vector2 hoverHotspot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SetDefaultCursor();
    }

    public void SetDefaultCursor()
    {
        if (defaultCursor != null)
        {
            Cursor.SetCursor(defaultCursor, defaultHotspot, cursorMode);
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, cursorMode);
        }
    }

    public void SetHoverCursor()
    {
        if (hoverCursor != null)
        {
            Cursor.SetCursor(hoverCursor, hoverHotspot, cursorMode);
        }
    }
}
