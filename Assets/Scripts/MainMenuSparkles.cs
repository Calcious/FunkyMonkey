using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuSparkles : MonoBehaviour
{
    [Header("Sparkle Sprite")]
    public Sprite sparkleSprite;

    [Header("Population")]
    public int sparkleCount = 54;
    public Vector2 sizeRange = new Vector2(18f, 60f);

    [Header("Timing")]
    public float fadeInDuration = 0.8f;
    public float holdDuration = 0.5f;
    public float fadeOutDuration = 1.3f;
    public float minDelayBetween = 0.5f;
    public float maxDelayBetween = 4f;

    [Header("Look")]
    public Vector2 peakAlphaRange = new Vector2(0.4f, 0.9f);
    public Color[] sparkleColors =
    {
        new Color(1f, 1f, 1f),
        new Color(1f, 0.85f, 0.4f),
        new Color(1f, 0.6f, 0.85f),
        new Color(0.5f, 0.95f, 0.9f),
    };

    private RectTransform containerRect;

    private void Start()
    {
        containerRect = GetComponent<RectTransform>();
        for (int i = 0; i < sparkleCount; i++)
        {
            GameObject sparkleGO = CreateSparkle();
            StartCoroutine(SparkleLoop(sparkleGO));
        }
    }

    private GameObject CreateSparkle()
    {
        var go = new GameObject("Sparkle", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(transform, false);

        var image = go.GetComponent<Image>();
        image.sprite = sparkleSprite;
        image.raycastTarget = false;
        image.color = new Color(1f, 1f, 1f, 0f);

        return go;
    }

    private IEnumerator SparkleLoop(GameObject sparkleGO)
    {
        var rect = sparkleGO.GetComponent<RectTransform>();
        var image = sparkleGO.GetComponent<Image>();

        // Stagger start times so sparkles don't all sync up on scene load.
        yield return new WaitForSeconds(Random.Range(0f, maxDelayBetween));

        while (true)
        {
            PositionAndStyleSparkle(rect, image);
            float peakAlpha = Random.Range(peakAlphaRange.x, peakAlphaRange.y);

            yield return Fade(image, 0f, peakAlpha, fadeInDuration);
            yield return new WaitForSeconds(holdDuration);
            yield return Fade(image, peakAlpha, 0f, fadeOutDuration);

            yield return new WaitForSeconds(Random.Range(minDelayBetween, maxDelayBetween));
        }
    }

    private void PositionAndStyleSparkle(RectTransform rect, Image image)
    {
        Vector2 halfSize = containerRect.rect.size * 0.5f;
        rect.anchoredPosition = new Vector2(Random.Range(-halfSize.x, halfSize.x), Random.Range(-halfSize.y, halfSize.y));

        float size = Random.Range(sizeRange.x, sizeRange.y);
        rect.sizeDelta = new Vector2(size, size);

        Color baseColor = sparkleColors[Random.Range(0, sparkleColors.Length)];
        image.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
    }

    private IEnumerator Fade(Image image, float from, float to, float duration)
    {
        float elapsed = 0f;
        Color c = image.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / duration);
            image.color = c;
            yield return null;
        }
        c.a = to;
        image.color = c;
    }
}
