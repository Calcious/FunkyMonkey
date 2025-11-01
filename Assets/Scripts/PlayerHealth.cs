using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("Funky Meter Settings")]
    public float maxFunkyMeter = 100f;
    public float currentFunkyMeter = 0f;

    [Header("Events")]
    public UnityEvent<float> onHealthChanged;
    public UnityEvent<float> onFunkyMeterChanged;
    public UnityEvent onDeath;

    private void Start()
    {
        currentHealth = maxHealth;
        currentFunkyMeter = 0f;

        onHealthChanged?.Invoke(currentHealth / maxHealth);
        onFunkyMeterChanged?.Invoke(currentFunkyMeter / maxFunkyMeter);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        onHealthChanged?.Invoke(currentHealth / maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        onHealthChanged?.Invoke(currentHealth / maxHealth);
    }

    public void AddFunkyMeter(float amount)
    {
        currentFunkyMeter += amount;
        currentFunkyMeter = Mathf.Clamp(currentFunkyMeter, 0f, maxFunkyMeter);

        onFunkyMeterChanged?.Invoke(currentFunkyMeter / maxFunkyMeter);
    }

    public void UseFunkyMeter(float amount)
    {
        currentFunkyMeter -= amount;
        currentFunkyMeter = Mathf.Clamp(currentFunkyMeter, 0f, maxFunkyMeter);

        onFunkyMeterChanged?.Invoke(currentFunkyMeter / maxFunkyMeter);
    }

    private void Die()
    {
        onDeath?.Invoke();
        Debug.Log("Player died!");
    }
}
