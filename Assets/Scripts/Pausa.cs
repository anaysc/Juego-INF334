using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Pausa : MonoBehaviour
{
    public GameObject panelPausa; // Panel que contiene el filtro gris y el texto de pausa
    public Button botonPausa; // Botón que activará y desactivará la pausa
    private bool juegoPausado = false;

    private List<AudioSource> fuentesAudio; // Lista para todas las fuentes de audio en la escena

    void Start()
    {
        // Asegurarse de que el juego comience sin pausa
        Time.timeScale = 1f;
        panelPausa.SetActive(false);

        // Configurar el botón para llamar a AlternarPausa cuando se presione
        if (botonPausa != null)
        {
            botonPausa.onClick.AddListener(AlternarPausa);
        }
        else
        {
            Debug.LogWarning("El botón de pausa no está asignado en el Inspector");
        }

        // Obtener todas las fuentes de audio en la escena
        fuentesAudio = new List<AudioSource>(FindObjectsOfType<AudioSource>());
    }

    // Función para pausar y reanudar el juego
    public void AlternarPausa()
    {
        juegoPausado = !juegoPausado;

        if (juegoPausado)
        {
            PausarTodo();
        }
        else
        {
            ReanudarTodo();
        }
    }

    private void PausarTodo()
    {
        // Pausar el tiempo
        Time.timeScale = 0f;
        panelPausa.SetActive(true); // Muestra el panel de pausa

        // Pausar todas las fuentes de audio
        foreach (AudioSource fuente in fuentesAudio)
        {
            if (fuente.isPlaying)
            {
                fuente.Pause();
            }
        }
    }

    private void ReanudarTodo()
    {
        // Reanudar el tiempo
        Time.timeScale = 1f;
        panelPausa.SetActive(false); // Oculta el panel de pausa

        // Reanudar todas las fuentes de audio
        foreach (AudioSource fuente in fuentesAudio)
        {
            fuente.UnPause();
        }
    }
}
