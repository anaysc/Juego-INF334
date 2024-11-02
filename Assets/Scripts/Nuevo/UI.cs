using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Para TextMeshPro
using UnityEngine.SceneManagement; // Para cargar escenas

public class UI : MonoBehaviour
{
    public AudioMaster audioMaster;   // Referencia al AudioMaster
    public GameObject panelPerdiste;  // El panel que muestra "Perdiste"
    public TMP_Text textoTiempo;      // Texto que muestra el tiempo total
    public Button botonSalir;         // Botón para salir al menú principal

    private float tiempoInicio;       // Tiempo de inicio del juego
    private bool juegoTerminado = false; // Para evitar que se llame múltiples veces

    // Nombre de la escena del menú principal
    public string nombreEscenaMenuPrincipal = "MenuPrincipal"; // Cambia esto por el nombre de tu escena del menú principal

    void Start()
    {
        // Iniciar el juego, ocultar el panel y reiniciar el contador de tiempo
        panelPerdiste.SetActive(false);
        tiempoInicio = Time.time;
        juegoTerminado = false;

        // Asignar el evento al botón de salir
        botonSalir.onClick.AddListener(IrAlMenuPrincipal);
    }

    void Update()
    {
        // Verificar si el audio principal ha terminado
        if (!juegoTerminado && audioMaster.HaTerminado)
        {
            // Terminar el juego si el audio ha terminado y no se ha llamado antes
            TerminarJuego();
        }
    }

    void TerminarJuego()
    {
        juegoTerminado = true;

        // Calcular el tiempo final y mostrarlo en el panel
        float tiempoFinal = Time.time - tiempoInicio;
        textoTiempo.text = "Tiempo: " + tiempoFinal.ToString("F2") + " segundos";

        // Mostrar el panel de derrota
        panelPerdiste.SetActive(true);

        // Detener el tiempo de juego (opcional)
        Time.timeScale = 0f;
    }

    void IrAlMenuPrincipal()
    {
        // Restablecer el tiempo de juego en caso de que lo hayas pausado
        Time.timeScale = 1f;

        // Cargar la escena del menú principal
        SceneManager.LoadScene(nombreEscenaMenuPrincipal);
    }
} 

