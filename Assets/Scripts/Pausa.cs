using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Para cargar escenas

public class Pausa : MonoBehaviour
{
    public GameObject panelPausa; // Panel que contiene el filtro gris y el texto de pausa
    public Button botonPausa; // Botón que activará y desactivará la pausa
    private bool juegoPausado = false;
    public Button botonQuit; // Botón que activará y desactivará la pausa
    public Button botonRetry; // Botón que activará y desactivará la pausa
    [SerializeField] public List<PersonajeUI> personajes; // Cambié a PersonajeUI porque parece que manejas personajes desde esta clase
    [SerializeField] public EnemigoUI enemigo; // Referencia al objeto enemigo
    public string nombreEscenaMenuPrincipal = "MenuPrincipal"; // Cambia esto por el nombre de tu escena del menú principal
    public string escenaRetry = "Nivel 1"; // Nombre de la escena para "Retry"

    private List<AudioSource> fuentesAudio; // Lista para todas las fuentes de audio en la escena

    void Start()
    {
        botonQuit.onClick.AddListener(() => CambiarEscena(nombreEscenaMenuPrincipal));
        botonRetry.onClick.AddListener(() => CambiarEscena(escenaRetry));
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
    void ReiniciarEstados()
    {
        foreach (var personaje in personajes)
        {
            if (personaje != null)
            {
                personaje.personaje.Reiniciar(); // Llama al método Reiniciar de cada personaje
            }
        }


        if (enemigo != null)
        {
            enemigo.enemigo.Reiniciar(); // Llama al método Reiniciar de cada enemigo
        }

    }
    void CambiarEscena(string nombreEscena)
    {
        // Restablecer el tiempo de juego antes de recargar
        Time.timeScale = 1f;
        ReiniciarEstados();

        // Cambiar a la escena especificada
        SceneManager.LoadScene(nombreEscena);
    }
}
