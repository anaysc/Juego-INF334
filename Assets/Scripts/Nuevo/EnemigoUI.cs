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
    public Button botonMenuPrincipalVictoria; // Botón de "Menú Principal"
    private List<AudioSource> fuentesAudio; // Lista para todas las fuentes de audio en la escena
    [SerializeField] public List<PersonajeUI> personajes; // Cambié a PersonajeUI porque parece que manejas personajes desde esta clase

    [SerializeField] private Master master; //Referencia al master
    [SerializeField] private AudioMaster audioMaster; //Referencia al AudioMaster de la escena

    public AudioSource audioSourceBase;
    public List<AudioSource> audioSourceHabilidades;

    public bool habilidadActivada = false;
    public Button botonRetryVictoria; // Botón que activará y desactivará la pausa
    public string escenaRetry = "Nivel 1"; // Nombre de la escena para "Retry"

    private float tiempoInicio; // Tiempo cuando comenzó el juego
    public bool estaRecibiendoDaño = false; // Indica si el personaje está recibiendo daño
    public SpriteRenderer spriteRenderer;
    public Sprite spriteNormal;
    public Sprite spriteAtaque;

    public float lastHp;
    public bool estaMuerto = false; // Indica si el personaje/enemigo ya murió

    internal void SeleccionarEnemigo(Enemigo e)
    {
        enemigo = e;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer no encontrado en el enemigo.");
        }
        fuentesAudio = new List<AudioSource>(FindObjectsOfType<AudioSource>());
        tiempoInicio = Time.time; // Inicia el tiempo cuando comienza el juego
        panelVictoria.SetActive(false); // Ocultar el panel de victoria al inicio
        Time.timeScale = 1f; // Asegurarse de que el tiempo fluye normalmente

        // Asignar los eventos a los botones
        botonMenuPrincipalVictoria.onClick.AddListener(IrMenuPrincipal);
        botonRetryVictoria.onClick.AddListener(() => CambiarEscena(escenaRetry));

    }

    void Update()
    {
        audioSourceBase.mute = (master.turnoActual != TurnType.enemigos || habilidadActivada);

        if (master.turnoActual != TurnType.enemigos)
        {
            // Cambiar al sprite normal
            spriteRenderer.sprite = spriteNormal;
        }

        // Revisa constantemente si la vida del enemigo es menor o igual a 0
        if (enemigo.Hp <= 0)
        {
            TerminarJuego();
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
            enemigo.Reiniciar(); // Llama al método Reiniciar de cada enemigo
        }
    }
    public void OnCiclo(int ciclo)
    {
        
    }
    public void ActivarTrackBase()
    {
        audioSourceBase.mute = false;
        DetenerTrackHabilidad();
        if (spriteRenderer != null && spriteNormal != null)
        {
            spriteRenderer.sprite = spriteNormal;
        }
        
    }
    public void ActivarTrackHabilidad(Habilidad habilidad)
    {
        if (spriteRenderer != null && spriteAtaque != null)
        {
            spriteRenderer.sprite = spriteAtaque;
        }
        //Primero muteamos la habilidad base y cualquier otra habilidad que podría estar sonando
        DetenerTrackHabilidad();
        audioSourceBase.mute = true;
        //audioMaster.masterAudioSource.volume = volumenAudioMasterReducido; // Reducir volumen del AudioMaster

        int indice = enemigo.Habilidades.IndexOf(habilidad);
        if (indice != -1)
        {
            //Aqui desmuteamos el track de la habilidad
            audioSourceHabilidades[indice].mute = false;
        }
        else
        {
            Debug.LogWarning("habilidad: " + habilidad.Nombre + " no encontrada");
        }

    }
    public void DetenerTrackHabilidad()
    {

        foreach (AudioSource audioSource in audioSourceHabilidades)
        {
            audioSource.mute = true;
        }
    }
    void TerminarJuego()
    {
        ReiniciarEstados();
        // Pausar el juego
        Time.timeScale = 0f;
        foreach (AudioSource fuente in fuentesAudio)
        {
            if (fuente.isPlaying)
            {
                fuente.Pause();
            }
        }
        // Calcular el tiempo total
        float tiempoFinal = Time.time - tiempoInicio;

        // Mostrar el panel de victoria
        panelVictoria.SetActive(true);

        // Mostrar el tiempo que se demoró en la victoria
        textoTiempo.text = "Tiempo: " + tiempoFinal.ToString("F2") + " segundos";
        
        Debug.Log("Juego terminado. Ganaste en " + tiempoFinal + " segundos.");
    }

    // Método para reiniciar el juego
    void VolverAJugar()
    {
        ReiniciarEstados();
        // Reinicia el tiempo y el estado del enemigo
        tiempoInicio = Time.time; // Restablece el tiempo al actual
        panelVictoria.SetActive(false); // Ocultar el panel de victoria
        Time.timeScale = 1f; // Reanudar el tiempo

        Debug.Log("Juego reiniciado");
    }

    // Método para ir al menú principal
    void IrMenuPrincipal()
    {
        ReiniciarEstados();

        // Cambiar de escena al menú principal
        SceneManager.LoadScene("MenuPrincipal");
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
