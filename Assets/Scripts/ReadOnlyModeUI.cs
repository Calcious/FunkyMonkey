using UnityEngine;
using UnityEngine.UI;

public class ReadOnlyModeUI : MonoBehaviour
{
    [Header("Visual Settings")]
    public Color readOnlyTint = new Color(0.7f, 0.7f, 0.7f, 1f);

    private Image[] images;
    private Color[] originalColors;
    private bool isReadOnly = false;

    public void SetReadOnlyMode(bool readOnly)
    {
        if (isReadOnly == readOnly) return;

        isReadOnly = readOnly;

        if (images == null)
        {
            images = GetComponentsInChildren<Image>(true);
            originalColors = new Color[images.Length];

            for (int i = 0; i < images.Length; i++)
            {
                originalColors[i] = images[i].color;
            }
        }

        for (int i = 0; i < images.Length; i++)
        {
            if (readOnly)
            {
                images[i].color = originalColors[i] * readOnlyTint;
            }
            else
            {
                images[i].color = originalColors[i];
            }
        }
    }
}
