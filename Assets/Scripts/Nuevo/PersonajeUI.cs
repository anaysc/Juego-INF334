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
    private float inputsTime[8] // lista de tiempos en los que se han apretados (o no) los últimos 8 beats
    private int cont = 0; //lleva la cuenta de en qué parte de la lista de inputs time vamos

    void Start()
    {
        inputsTime = [0,0,0,0,0,0,0,0]
        cont = 0;
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
    void OnBeat(double tiempoBeat, bool firstBeat)
    {
//en un compás ir llevando registro de cada vez que se apreta el input: tendre una lista de floats de los tiempos en los que se apretó 
//ocurre el beat, esperamos un poco y después actualizamos
//cada beat, le mandas el registro a una funcion (por ej detectar patron). si el grado de exito de detectar patron es 0, el track para
//detectar patron eventualmente, siempre te entregará el mejor patrón, cuando el grado de éxito no es 0. como tengo este patrón,
//puedo comparar el beat que sigue, para mostrarle en pantalla si lo está haciendo bien
//cada habilidad sólo se puede realizar al comienzo de (dos compases) y cada 2 compases etc. al final de los dos compases, se evalua
//que tan bien lo hizo con el historial (lista de floats) que en teoría 
        if (firstBeat == true && habilidadActivada == false) //si estamos al comienzo de un compás
        {
            habilidadActivada = true;
            inputsTime[cont] = tiempoInput; //si no se apretó ningún otro input después de este, el prox tiempo input quedaría igual ??? 
            cont++; 
            var (habilidadDetectadaPrimero, gradoExito) = personaje.DetectarPatron(inputsTime);
            if (gradoExito >= 1) //si 
            {
                habilidadDetectadaPrimero.track.Play() //la clase Habilidad deberia tener el atributo track que guarde las respectivas canciones
            }
        }
        if (habilidadActivada == true && firstBeat == false)
        {
            inputsTime[cont] = tiempoInput; //si no se apretó ningún otro input después de este, el prox tiempo input quedaría igual ??? 
            cont++; 
            var (habilidadDetectada, gradoExito) = personaje.DetectarPatron(inputsTime);
            //eventualmente agregar código que muestre que hiciste bien el beat que se supone que iba en ese lugar
        }
        if (cont == 7) //último beat de la habilidad, se muestra "puntaje obtenido"
        {
            var (habilidadDetectada, gradoExito) = personaje.DetectarPatron(inputsTime);
            habilidadDetectadaPrimero.track.Stop();  
            Debug.Log("Obtuviste un puntaje de " + gradoExito)
            cont = 0;
            habilidadActivada = false;
            inputsTime = [0,0,0,0,0,0,0,0]

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
 