using UnityEngine;

public class ButtonPulseEffect : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float pulseSpeed = 2f;
    public float pulseScale = 1.15f;

    private Vector3 originalScale;
    private float pulseTimer;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void Update()
    {
        pulseTimer += Time.deltaTime * pulseSpeed;
        float scaleFactor = 1f + (Mathf.Sin(pulseTimer) * 0.5f + 0.5f) * (pulseScale - 1f);
        transform.localScale = originalScale * scaleFactor;
    }

    private void OnDisable()
    {
        transform.localScale = originalScale;
    }
}
