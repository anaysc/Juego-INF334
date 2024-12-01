using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Para TextMeshPro
using UnityEngine.SceneManagement; // Para cargar escenas
using Combate;
public class UI : MonoBehaviour
{
    public AudioMaster audioMaster;   // Referencia al AudioMaster
    public GameObject panelPerdiste;  // El panel que muestra "Perdiste"
    public TMP_Text textoTiempo;      // Texto que muestra el tiempo total
    public Button botonQuitDerrota;         // Botón para salir al menú principal
    public Button botonRetryDerrota; // Botón que activará y desactivará la pausa

    private List<AudioSource> fuentesAudio; // Lista para todas las fuentes de audio en la escena

    [SerializeField] private Master master; //Referencia al master
    [SerializeField] public List<PersonajeUI> personajes; // Cambié a PersonajeUI porque parece que manejas personajes desde esta clase
    [SerializeField] public EnemigoUI enemigo; // Referencia al objeto enemigo

    private float tiempoInicio;       // Tiempo de inicio del juego
    private bool juegoTerminado = false; // Para evitar que se llame múltiples veces
    public Button botonQuitVictoria; // Botón que activará y desactivará la pausa
    public Button botonRetryVictoria; // Botón que activará y desactivará la pausa

    // Nombre de la escena del menú principal
    public string nombreEscenaMenuPrincipal = "MenuPrincipal"; // Cambia esto por el nombre de tu escena del menú principal
    public string escenaRetry = "Nivel 1"; // Nombre de la escena para "Retry"
    public AudioSource audioDaño; 

    void Start()
    {
        foreach (var personaje in personajes)
        {
            if (personaje != null && personaje.personaje != null)
            {
                personaje.lastHp = personaje.personaje.MaxHp;
            }
        }

        // Inicializar lastHp para enemigo
        if (enemigo != null && enemigo.enemigo != null)
        {
            enemigo.lastHp = enemigo.enemigo.MaxHp;
        }

        fuentesAudio = new List<AudioSource>(FindObjectsOfType<AudioSource>());
        // Iniciar el juego, ocultar el panel y reiniciar el contador de tiempo
        panelPerdiste.SetActive(false);
        tiempoInicio = Time.time;
        juegoTerminado = false;
        botonQuitVictoria.onClick.AddListener(() => CambiarEscena(nombreEscenaMenuPrincipal));
        botonRetryVictoria.onClick.AddListener(() => CambiarEscena(escenaRetry));

        // Asignar el evento al botón de salir
        botonQuitDerrota.onClick.AddListener(() => CambiarEscena(nombreEscenaMenuPrincipal));
        botonRetryDerrota.onClick.AddListener(() => CambiarEscena(escenaRetry));

    }

    void Update()
    {
        // Verificar si todos los personajes están muertos
        if (!juegoTerminado && TodosLosPersonajesMuertos())
        {
            MostrarDerrota();
        }

        // Verificar si el audio principal ha terminado
        if (!juegoTerminado && audioMaster.HaTerminado)
        {
            MostrarDerrota();
        }
        foreach (var personaje in personajes)
        {
            if (personaje != null && personaje.personaje != null)
            {
                if (personaje.personaje.Hp < personaje.lastHp && !personaje.estaMuerto) // Si la vida bajó
                {
                    MostrarEfectoDaño(personaje);
                }
                // Actualizar lastHp
                personaje.lastHp = personaje.personaje.Hp;

            }
        }

        // Detectar daño en el enemigo
        if (enemigo != null && enemigo.enemigo != null)
        {
            if (enemigo.enemigo.Hp < enemigo.lastHp && !enemigo.estaMuerto) // Si la vida bajó
            {
                MostrarEfectoDaño(enemigo);
                Debug.Log("daño al enemigo");
            }
            // Actualizar lastHp
            enemigo.lastHp = enemigo.enemigo.Hp;
        }
    }
    private void CambiarABlancoYNegro(MonoBehaviour criatura)
    {
        SpriteRenderer spriteRenderer = criatura.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Cambia el color del sprite a una tonalidad de gris
            spriteRenderer.color = new Color(0.3f, 0.3f, 0.3f, 1f); // Gris sin alterar la transparencia
            Debug.Log($"Cambiado a blanco y negro (color) en: {criatura.name}");
        }
        else
        {
            Debug.LogWarning($"No se encontró SpriteRenderer en {criatura.name}");
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


    bool TodosLosPersonajesMuertos()
    {
        foreach (var personaje in personajes)
        {
            if (personaje.personaje.Hp > 0) // Verificar si algún personaje sigue vivo
            {
                return false;
            }
        }
        return true; 
        
    }

    void MostrarDerrota()
    {
        juegoTerminado = true;

        // Calcular el tiempo final y mostrarlo en el panel
        float tiempoFinal = Time.time - tiempoInicio;
        textoTiempo.text = "Tiempo: " + tiempoFinal.ToString("F2") + " segundos";

        // Mostrar el panel de derrota
        panelPerdiste.SetActive(true);

        // Detener el tiempo de juego (opcional)
        Time.timeScale = 0f;
        foreach (AudioSource fuente in fuentesAudio)
        {
            if (fuente.isPlaying)
            {
                fuente.Pause();
            }
        }
    }

    void IrAlMenuPrincipal()
    {
        // Restablecer el tiempo de juego en caso de que lo hayas pausado
        Time.timeScale = 1f;
        ReiniciarEstados();
        // Cargar la escena del menú principal
        SceneManager.LoadScene(nombreEscenaMenuPrincipal);
    }
    void CambiarEscena(string nombreEscena)
    {
        // Restablecer el tiempo de juego antes de recargar
        Time.timeScale = 1f;
        ReiniciarEstados();

        // Cambiar a la escena especificada
        SceneManager.LoadScene(nombreEscena);
    }
    private void MostrarEfectoDaño(PersonajeUI personaje)
    {
        if (!personaje.estaRecibiendoDaño) // Solo si el efecto no está activo
        {
            StartCoroutine(EfectoDaño(personaje.spriteRenderer, personaje));
        }
        
    }

    private void MostrarEfectoDaño(EnemigoUI enemigo)
    {
        if (!enemigo.estaRecibiendoDaño) // Solo si el efecto no está activo
        {
            StartCoroutine(EfectoDaño(enemigo.spriteRenderer, enemigo));
        }
    }
    private IEnumerator EfectoDaño(SpriteRenderer spriteRenderer, MonoBehaviour criatura)
    {

        if (criatura is PersonajeUI personaje)
        {
            personaje.estaRecibiendoDaño = true;
        }
        else if (criatura is EnemigoUI enemigo)
        {
            enemigo.estaRecibiendoDaño = true;
        }
        // Reproducir sonido de daño
        if (audioDaño != null)
        {
            audioDaño.Play();
        }


        Color colorOriginal = spriteRenderer.color;
        spriteRenderer.color = Color.red;

        try
        {
            for (int i = 0; i < 4; i++)
            {
                spriteRenderer.enabled = false;
                yield return new WaitForSeconds(0.2f);
                spriteRenderer.enabled = true;
                yield return new WaitForSeconds(0.2f);
            }
        }
        finally
        {
            spriteRenderer.color = colorOriginal;

            if (criatura is PersonajeUI p)
            {
                p.estaRecibiendoDaño = false;
            }
            else if (criatura is EnemigoUI e)
            {
                e.estaRecibiendoDaño = false;
            }
        }
        if (criatura is PersonajeUI pe)
        {
            if (pe.personaje.Hp <= 0 && !pe.estaMuerto) // Si el personaje murió
            {
                CambiarABlancoYNegro(pe);
                pe.estaMuerto = true; // Marcarlo como muerto para no repetir el cambio
            }
        }
        else if (criatura is EnemigoUI en)
        {
           if (en.enemigo.Hp <= 0 && !en.estaMuerto) // Si el personaje murió
            {
                CambiarABlancoYNegro(en);
                en.estaMuerto = true; // Marcarlo como muerto para no repetir el cambio
            }
        }


    }


}

