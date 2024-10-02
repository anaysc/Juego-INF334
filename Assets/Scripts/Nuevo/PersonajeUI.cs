using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combate;

public class PersonajeUI : MonoBehaviour
{
    public Personaje personaje;          // Referencia al objeto Personaje
    public KeyCode teclaControl;         // Tecla asignada para controlar este personaje
    public AudioSource pistaBase;        // AudioSource para la pista base (track 1)
    public AudioSource pistaOffbeat;     // AudioSource para la pista offbeat (track 2)
    public float tiempoMaxDesviacion = 0.2f; // Margen de desviación permitido respecto al beat

    private Cronometro cronometro;       // Referencia al Cronometro en la escena
    private double tiempoInput = -1;     // Tiempo en el que el jugador ha presionado la tecla
    private bool habilidadActivada;      // Indica si ya se activó la habilidad en el beat actual
    private bool estaTocandoBase = false; // Indica si el track base está sonando
    private int compasesRestantesOffbeat = 0; // Controla los compases que debe sonar el track 2 (offbeat)
    private double beatInterval = 0f;     // Intervalo de tiempo entre beats
    private double previousBeatTime = 0f; // Tiempo del beat anterior

    void Start()
    {
        // Obtener referencia al Cronometro en la escena
        cronometro = FindObjectOfType<Cronometro>();  // Encuentra el Cronometro en la escena
        if (cronometro != null)
        {
            // Suscribirse al evento OnBeat del cronómetro
            cronometro.OnBeat += OnBeat;
        }

        habilidadActivada = false;
    }

    void Update()
    {
        DetectarInput();  // Detecta la pulsación del jugador en cada frame
    }

    // Detecta la tecla asignada al personaje
    void DetectarInput()
    {
        if (Input.GetKeyDown(teclaControl))  // Si el jugador presiona la tecla asignada
        {
            tiempoInput = AudioSettings.dspTime;  // Captura el tiempo en que se presiona la tecla
            Debug.Log("Input detectado en tiempo: " + tiempoInput);
        }
    }

    // Método que se llama cada vez que ocurre un "OnBeat"
    void OnBeat(double tiempoBeat)
    {
    
        // Calcula el intervalo entre beats
        if (previousBeatTime > 0f)
        {
            beatInterval = tiempoBeat - previousBeatTime;
        }
        previousBeatTime = tiempoBeat;

        if (tiempoInput >= 0f && beatInterval > 0f)
        {
            double halfBeatInterval = beatInterval / 2f;

            // Tiempos del beat y del contratiempo
            double tiempoOffbeat = tiempoBeat + halfBeatInterval;

            // Calcula desviaciones respecto al beat y al contratiempo
            double desviacionBeat = Mathf.Abs((float)(tiempoInput - tiempoBeat));
            double desviacionOffbeat = Mathf.Abs((float)(tiempoInput - tiempoOffbeat));

            // Verifica si la desviación está dentro del margen permitido
            if (desviacionBeat <= tiempoMaxDesviacion)
            {
                // Beat
                if (!estaTocandoBase)
                {
                    ActivarTrackBase();
                    Debug.Log("¡Beat correcto! Activando pista base.");
                }
                else
                {
                    Debug.Log("¡Beat perfecto! Manteniendo pista base.");
                }
            }
            else if (desviacionOffbeat <= tiempoMaxDesviacion)
            {
                // Offbeat
                if (estaTocandoBase)
                {
                    Debug.Log("¡Offbeat detectado! Cambiando a pista offbeat.");
                    ActivarOffbeat();
                }
                else
                {
                    Debug.Log("¡Offbeat correcto! Manteniendo pista offbeat.");
                }
            }
            else
            {
                Debug.Log("Desviación fuera del rango. Beat: " + desviacionBeat + ", Offbeat: " + desviacionOffbeat);
            }

            // Resetea el tiempo del input
            tiempoInput = -1f;
            habilidadActivada = true;
        }

        // Resetea el estado de activación de la habilidad
        habilidadActivada = false;

        // Control del tiempo que debe sonar la pista offbeat
        if (compasesRestantesOffbeat > 0)
        {
            compasesRestantesOffbeat--;

            if (compasesRestantesOffbeat == 0)
            {
                Debug.Log("Finalizan los compases del offbeat, volviendo a pista base.");
                ActivarTrackBase();  // Vuelve a la pista base después del offbeat
            }
        }
    }

    // Activa la pista base (track 1)
    void ActivarTrackBase()
    {
        if (pistaOffbeat.isPlaying)
        {
            pistaOffbeat.Stop();  // Detener el track 2 (offbeat)
        }

        if (!pistaBase.isPlaying)
        {
            pistaBase.Play();  // Reproducir el track 1 (base)
        }

        estaTocandoBase = true;
    }

    // Activa la pista offbeat (track 2)
    void ActivarOffbeat()
    {
        if (pistaBase.isPlaying)
        {
            pistaBase.Stop();  // Detener el track 1 (base)
        }

        if (!pistaOffbeat.isPlaying)
        {
            pistaOffbeat.Play();  // Reproducir el track 2 (offbeat)
        }

        estaTocandoBase = false;
        compasesRestantesOffbeat = 18;  // El track 2 suena por 18 beats
    }

    void OnDestroy()
    {
        // Desuscribirse del evento OnBeat cuando el objeto se destruye
        if (cronometro != null)
        {
            cronometro.OnBeat -= OnBeat;
        }
    }
}
 