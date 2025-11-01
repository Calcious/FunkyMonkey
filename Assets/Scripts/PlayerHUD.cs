using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [Header("References")]
    public HealthBar healthBar;
    public HealthBar funkyMeterBar;
    public PlayerHealth playerHealth;

    private void Start()
    {
        if (playerHealth == null)
        {
            playerHealth = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            playerHealth.onHealthChanged.AddListener(UpdateHealthBar);
            playerHealth.onFunkyMeterChanged.AddListener(UpdateFunkyMeter);
        }

        if (healthBar != null)
        {
            healthBar.SetColor(Color.red);
        }

        if (funkyMeterBar != null)
        {
            Color gray = new Color(0.5f, 0.5f, 0.5f);
            Color gold = new Color(1f, 0.84f, 0f);
            funkyMeterBar.SetColorGradient(gray, gold);
        }
    }

    private void UpdateHealthBar(float normalizedHealth)
    {
        if (healthBar != null)
        {
            healthBar.SetFillAmount(normalizedHealth);
        }
    }

    private void UpdateFunkyMeter(float normalizedFunky)
    {
        if (funkyMeterBar != null)
        {
            funkyMeterBar.SetFillAmount(normalizedFunky);
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.onHealthChanged.RemoveListener(UpdateHealthBar);
            playerHealth.onFunkyMeterChanged.RemoveListener(UpdateFunkyMeter);
        }
    }
}
