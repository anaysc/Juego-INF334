using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combate;

public class PersonajeUI : MonoBehaviour
{
    public Master master; //Referencia al master

    public string nombrePersonaje;       // Nombre del personaje
    public Personaje personaje;          // Referencia al objeto Personaje
    public KeyCode teclaControl;         // Tecla asignada para controlar este personaje
    public AudioSource pistaBase;        // AudioSource para la pista base (track 1)
    public AudioSource pistaOffbeat;     // AudioSource para la pista offbeat (track 2)
    public float tiempoMaxDesviacion = 0.2f; // Margen de desviación permitido respecto al beat

    private Cronometro cronometro;       // Referencia al Cronometro en la escena
    private float tiempoInput = -1;     // Tiempo en el que el jugador ha presionado la tecla
    private bool habilidadActivada;      // Indica si ya se activó la habilidad en el beat actual
    private bool estaTocandoBase = false; // Indica si el track base está sonando
    private int compasesRestantesOffbeat = 0; // Controla los compases que debe sonar el track 2 (offbeat)
    private double beatInterval = 0f;     // Intervalo de tiempo entre beats
    private double previousBeatTime = 0f; // Tiempo del beat anterior
    private double previousFirstBeatTime = 0f; //Tiempo en que empezó el ciclo de habilidad actual (del primer beat)
    private List<float> inputsTime = new List<float>(); // lista de tiempos en los que se han apretados (o no) los últimos 8 beats
    private int cont = 0; //lleva la cuenta de en qué parte de la lista de inputs time vamos
    private Habilidad habilidadDetectadaActual;

    void Start()
    {
        previousFirstBeatTime = AudioSettings.dspTime;
        beatInterval = 60.0 / Cronometro.bpm;
        if(nombrePersonaje != "")
        {
            SeleccionarPersonaje(nombrePersonaje); //Temporalmente esto funciona así
        }

        cont = 0;
        // Obtener referencia al Cronometro en la escena
        cronometro = FindObjectOfType<Cronometro>();  // Encuentra el Cronometro en la escena
        if (cronometro != null)
        {
            // Suscribirse al evento OnBeat del cronómetro
            cronometro.OnBeat += OnBeat;
        }

        habilidadActivada = false;
        habilidadDetectadaActual = null;
    }

    void Update()
    {
        DetectarInput();  // Detecta la pulsación del jugador en cada frame
    }

    private void SeleccionarPersonaje(string nuevoNombre)
    {
        if(master.DictPersonajes.TryGetValue(nuevoNombre, out Personaje p))
        {
            nombrePersonaje = nuevoNombre;
            personaje = p;
        }
        else
        {
            Debug.Log("No se encontró el personaje: " + nuevoNombre);
        }
    }

    // Detecta la tecla asignada al personaje
    void DetectarInput()
    {
        if (Input.GetKeyDown(teclaControl))  // Si el jugador presiona la tecla asignada
        {
            double tiempo = (AudioSettings.dspTime - previousFirstBeatTime)/(beatInterval) * 4; //Se normaliza para que esté entre 0 y 32 (semicorcheas de 2 compases)
            inputsTime.Add((float)tiempo);
            //tiempoInput = (float) AudioSettings.dspTime;  // Captura el tiempo en que se presiona la tecla
            Debug.Log("Input detectado en tiempo: " + tiempo);
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

        Debug.Log("tiempoBeat: " + (tiempoBeat-previousFirstBeatTime)/beatInterval * 4);

        if(cont == 7) //Esto debería indicar que terminó el último beat del ciclo anterior, y empezó el primero del nuevo
        {
            (Habilidad habilidadDetectada, int gradoExito) = personaje.DetectarPatron(inputsTime);
            DetenerTrackHabilidad();
            Debug.Log("Obtuviste un puntaje de " + gradoExito);
            //Aqui debería ejecutarse la habilidad

            previousFirstBeatTime = tiempoBeat;
            cont = 0;
            habilidadActivada = false;
            float lastTime = inputsTime[inputsTime.Count - 1];
            inputsTime.Clear();
            if(lastTime >= 31)
            {
                inputsTime.Add(lastTime-32); //Añade el ultimo input del ciclo anterior en caso de que pueda entenderse como el primero del nuevo
            }
            habilidadDetectadaActual = null;
        }
        //Esto se ejecuta inmediatamente despues de lo anterior
        if (firstBeat == true && habilidadActivada == false) //si estamos al comienzo de un compás
        {
            //notar que cont parte en 0
            habilidadActivada = true; 
            (Habilidad habilidadDetectada, int gradoExito) = personaje.DetectarPatron(inputsTime, cont*4 + 1); //cont*4 + 1 porque hay 4 semicorcheas en un beat, y ademas se considera la primera semicorchea del beat actual
            habilidadDetectadaActual = habilidadDetectada;
            if (gradoExito >= 1) //si 
            {
                ActivarTrackHabilidad(habilidadDetectada);
            }
            cont++;
        }
        if (habilidadActivada == true && firstBeat == false)
        {
            (Habilidad habilidadDetectada, int gradoExito) = personaje.DetectarPatron(inputsTime, cont*4+1);
            if(habilidadDetectada != habilidadDetectadaActual)
            {
                DetenerTrackHabilidad();
                if (gradoExito >= 1)
                {
                    habilidadDetectadaActual = habilidadDetectada;
                    ActivarTrackHabilidad(habilidadDetectada);
                }
            }
            else if(gradoExito==0)
            {
                DetenerTrackHabilidad();
            }
            cont++;

            //eventualmente agregar código que muestre que hiciste bien el beat que se supone que iba en ese lugar
        }

    }
    void ActivarTrackHabilidad(Habilidad habilidad)
    {
        //Vacio por ahora pero aqui deberia implementarse que empieze a sonar lo que dice el nombre
    }
    void DetenerTrackHabilidad()
    {
        //Detiene el track de habilidad que esté activo en el momento
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
 