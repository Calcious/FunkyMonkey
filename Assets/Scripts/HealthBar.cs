using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Image fillImage;
    public Image backgroundImage;

    [Header("Settings")]
    public Color fullColor = Color.red;
    public Color emptyColor = Color.red;
    public bool smoothTransition = true;
    public float transitionSpeed = 5f;
    public bool useColorGradient = false;

    private float maxHealth;
    private float targetFillAmount;

    public void SetMaxHealth(float max)
    {
        maxHealth = max;
    }

    public void SetHealth(float current)
    {
        if (maxHealth <= 0f) return;

        targetFillAmount = current / maxHealth;

        if (!smoothTransition && fillImage != null)
        {
            fillImage.fillAmount = targetFillAmount;
            UpdateColor();
        }
    }

    public void SetFillAmount(float normalizedValue)
    {
        targetFillAmount = Mathf.Clamp01(normalizedValue);

        if (!smoothTransition && fillImage != null)
        {
            fillImage.fillAmount = targetFillAmount;
            UpdateColor();
        }
    }

    public void SetColor(Color color)
    {
        fullColor = color;
        emptyColor = color;

        if (fillImage != null)
        {
            fillImage.color = color;
        }
    }

    public void SetColorGradient(Color empty, Color full)
    {
        emptyColor = empty;
        fullColor = full;
        useColorGradient = true;
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (useColorGradient && fillImage != null)
        {
            fillImage.color = Color.Lerp(emptyColor, fullColor, targetFillAmount);
        }
    }

    private void Update()
    {
        if (smoothTransition && fillImage != null)
        {
            fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, targetFillAmount, Time.deltaTime * transitionSpeed);
        }

        if (useColorGradient)
        {
            UpdateColor();
        }
    }
}
