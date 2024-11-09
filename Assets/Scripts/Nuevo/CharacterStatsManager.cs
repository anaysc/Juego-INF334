using System.Collections.Generic;
using Combate;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatsManager : MonoBehaviour
{
    // Lista para almacenar las barras de vida y mana de cada personaje
    public List<Slider> healthBars;  // Asigna una barra de vida para cada personaje en el Inspector
    public List<Slider> manaBars;    // Asigna una barra de magia para cada personaje en el Inspector

    [SerializeField] private List<PersonajeUI> personajesUI; // UI para los personajes
    [SerializeField] private List<EnemigoUI> enemigosUI;     // UI para los enemigos

    private List<Creatura> criaturas;

    private void Start()
    {
        // Inicializa la lista de criaturas basada en los UI de personajes y enemigos
        criaturas = new List<Creatura>();

        foreach (var personajeUI in personajesUI)
        {
            if (personajeUI != null)
            {
                criaturas.Add(personajeUI.personaje);
            }
        }

        foreach (var enemigoUI in enemigosUI)
        {
            if (enemigoUI != null)
            {
                criaturas.Add(enemigoUI.enemigo);
            }
        }

        if (criaturas.Count != healthBars.Count || criaturas.Count != manaBars.Count)
        {
            Debug.LogWarning("El número de criaturas no coincide con el número de barras de vida o magia asignadas");
        }
    }

    private void Update()
    {
        UpdateHealthBars();
        UpdateManaBars();
    }

    // Actualiza todas las barras de vida
    void UpdateHealthBars()
    {
        for (int i = 0; i < criaturas.Count; i++)
        {
            if (criaturas[i] != null && i < healthBars.Count)
            {
                healthBars[i].value = criaturas[i].Hp / criaturas[i].MaxHp;
            }
        }
    }

    // Actualiza todas las barras de magia
    void UpdateManaBars()
    {
        for (int i = 0; i < criaturas.Count; i++)
        {
            if (criaturas[i] != null && i < manaBars.Count)
            {
                manaBars[i].value = criaturas[i].Mana / criaturas[i].MaxMana;
            }
        }
    }
}
