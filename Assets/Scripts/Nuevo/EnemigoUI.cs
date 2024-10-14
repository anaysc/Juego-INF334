using Combate;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Para cargar escenas
using UnityEngine.UI; // Necesario para usar elementos de la UI como Button

public class EnemigoUI : MonoBehaviour
{
    public Enemigo enemigo; // Referencia al objeto enemigo
    public GameObject panelVictoria; // Panel de victoria
    public TMP_Text textoTiempo; // Texto TMP para mostrar el tiempo
    public Button botonReiniciar; // Botón de "Volver a Jugar"
    public Button botonMenuPrincipal; // Botón de "Menú Principal"

    private float tiempoInicio; // Tiempo cuando comenzó el juego

    internal void SeleccionarEnemigo(Enemigo e)
    {
        enemigo = e;
    }

    void Start()
    {
        tiempoInicio = Time.time; // Inicia el tiempo cuando comienza el juego
        panelVictoria.SetActive(false); // Ocultar el panel de victoria al inicio
        Time.timeScale = 1f; // Asegurarse de que el tiempo fluye normalmente

        // Asignar los eventos a los botones
        botonReiniciar.onClick.AddListener(VolverAJugar);
        botonMenuPrincipal.onClick.AddListener(IrMenuPrincipal);
    }

    void Update()
    {
        // Revisa constantemente si la vida del enemigo es menor o igual a 0
        if (enemigo.Hp <= 0)
        {
            TerminarJuego();
        }
    }

    void TerminarJuego()
    {
        // Pausar el juego
        Time.timeScale = 0f;

        // Calcular el tiempo total
        float tiempoFinal = Time.time - tiempoInicio;

        // Mostrar el panel de victoria
        panelVictoria.SetActive(true);

        // Mostrar el tiempo que se demoró en la victoria
        textoTiempo.text = "¡Ganaste! Tiempo: " + tiempoFinal.ToString("F2") + " segundos";
        
        Debug.Log("Juego terminado. Ganaste en " + tiempoFinal + " segundos.");
    }

    // Método para reiniciar el juego
    void VolverAJugar()
    {
        // Reinicia el tiempo y el estado del enemigo
        tiempoInicio = Time.time; // Restablece el tiempo al actual
        enemigo.Hp = enemigo.MaxHp; // Restablecer la vida del enemigo
        panelVictoria.SetActive(false); // Ocultar el panel de victoria
        Time.timeScale = 1f; // Reanudar el tiempo

        Debug.Log("Juego reiniciado");
    }

    // Método para ir al menú principal
    void IrMenuPrincipal()
    {
        // Cambiar de escena al menú principal
        SceneManager.LoadScene("MenuPrincipal");
    }
}
