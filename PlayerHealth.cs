using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public Slider lifeBar;

    private bool isDead = false;

    public bool shieldActive = false;
    void Start()
    {
        currentHealth = maxHealth;

        lifeBar.maxValue = maxHealth;
        lifeBar.value = currentHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        if (shieldActive)
        {
            Debug.Log("Escudo bloqueou o dano!");
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        lifeBar.value = currentHealth;

#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif

        if (currentHealth <= 0)
        {
            isDead = true;
            Debug.Log("Player morreu");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerDied();
            }
        }
    }

    public void ActivateShield()
    {
        shieldActive = true;

        Debug.Log("ESCUDO ATIVADO!");
    }

    public void Heal(int amount)
    {
        if (isDead)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        lifeBar.value = currentHealth;

        Debug.Log("Player recuperou " + amount + " de vida!");
    }
}