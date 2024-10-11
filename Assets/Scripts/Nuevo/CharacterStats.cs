using Combate;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    public Slider healthBar;  // Asigna la barra de vida en el Inspector
    public Slider manaBar;    // Asigna la barra de magia en el Inspector

    [SerializeField] private PersonajeUI personajeUI; //Solo uno de estos dos deberia tener valor no null, de acuerdo se es enemigo o personaje
    [SerializeField] private EnemigoUI enemigoUI;

    private Creatura creatura;

    protected virtual Creatura GetCreatura()
    {
        if(personajeUI != null)
        {
            return personajeUI.personaje;
        }
        else if(enemigoUI != null)
        {
            return enemigoUI.enemigo;
        }
        else
        {
            Debug.LogWarning("Ni PersonajeUI ni EnemigoUI asignado");
            return null;
        }
    }

    private void Update()
    {
        creatura = GetCreatura();
        UpdateHealthBar();
        UpdateManaBar();
    }

    // Actualiza la barra de vida
    void UpdateHealthBar()
    {
        healthBar.value = creatura.Hp / creatura.MaxHp;
    }

    // Actualiza la barra de magia
    void UpdateManaBar()
    {
        manaBar.value = creatura.Mana / creatura.MaxMana;
    }
}
