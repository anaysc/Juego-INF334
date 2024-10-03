using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    public Slider healthBar;  // Asigna la barra de vida en el Inspector
    public Slider manaBar;    // Asigna la barra de magia en el Inspector

    private float maxHealth = 100f;   // Vida m�xima
    private float currentHealth;      // Vida actual
    private float maxMana = 100f;     // Magia m�xima
    private float currentMana;        // Magia actual

    void Start()
    {
        // Inicializa las estad�sticas
        currentHealth = maxHealth;
        currentMana = maxMana;

        UpdateHealthBar();
        UpdateManaBar();
    }

    // M�todo para reducir vida
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        UpdateHealthBar();
    }

    // M�todo para gastar magia
    public void UseMana(float amount)
    {
        currentMana -= amount;
        if (currentMana < 0) currentMana = 0;
        UpdateManaBar();
    }

    // Actualiza la barra de vida
    void UpdateHealthBar()
    {
        healthBar.value = currentHealth / maxHealth;
    }

    // Actualiza la barra de magia
    void UpdateManaBar()
    {
        manaBar.value = currentMana / maxMana;
    }

    // M�todos para regenerar vida y magia
    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void RegenerateMana(float amount)
    {
        currentMana += amount;
        if (currentMana > maxMana) currentMana = maxMana;
        UpdateManaBar();
    }
}
