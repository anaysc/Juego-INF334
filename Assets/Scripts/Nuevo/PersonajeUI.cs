using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combate;

public class PersonajeUI : MonoBehaviour
{
    public Master master; // Referencia al master

    public string nombrePersonaje;       // Nombre del personaje
    public Personaje personaje;          // Referencia al objeto Personaje
    public KeyCode teclaControl;         // Tecla asignada para controlar este personaje
    public float tiempoMaxDesviacion = 0.7f; // Margen de desviación permitido en beats
    public AudioSource audioSource;      // El AudioSource preexistente en el GameObject

    private Cronometro cronometro;       // Referencia al Cronometro en la escena
    private double tiempoInput = -1;     // Tiempo en el que el jugador ha presionado la tecla
    private bool habilidadActivada;      // Indica si la habilidad está activada
    private double beatInterval = 0f;    // Intervalo de tiempo entre beats
    private double previousBeatTime = 0f; // Tiempo del beat anterior
    private double previousFirstBeatTime = 0f; //Tiempo en que empezó el ciclo de habilidad actual (del primer beat)
    private Habilidad habilidadDetectadaActual;
    private AudioManager audioManager;
    private double ultimoTiempoBeat = 0f; // Último tiempoBeat recibido
    private Habilidad habilidadEspecifica; // La habilidad que se activará (BajoHabilidad1)
    private int beatsSinceAbilityActivated = 0; // Beats desde que la habilidad fue activada
    private bool inputCorrectoEnBeat = false; // Indica si el input fue correcto en el beat

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        previousFirstBeatTime = AudioSettings.dspTime;
        beatInterval = 60.0 / Cronometro.bpm;

        if (nombrePersonaje != "")
        {
            SeleccionarPersonaje(nombrePersonaje); // Temporalmente esto funciona así
        }

        // Obtener referencia al Cronometro en la escena
        cronometro = FindObjectOfType<Cronometro>();  // Encuentra el Cronometro en la escena
        if (cronometro != null)
        {
            // Suscribirse al evento OnBeat del cronómetro
            cronometro.OnBeat += OnBeat;
        }

        habilidadActivada = false;
        habilidadDetectadaActual = null;

        // Asignar la habilidad específica (habilidad1)
        if (personaje != null && personaje.Habilidades.Count > 1)
        {
            habilidadEspecifica = personaje.Habilidades[1]; // Suponiendo que es la segunda en la lista
        }
        else
        {
            Debug.LogWarning("No se encontró la habilidad específica.");
        }

        // Reproducir la habilidad base
        if (personaje != null && personaje.Habilidades.Count > 0)
        {
            Habilidad habilidadBase = personaje.Habilidades[0];  // Obtener la primera habilidad del personaje

            // Usar el AudioManager para obtener el AudioClip correspondiente
            AudioClip audioBase = audioManager.ObtenerAudioPorNombre(habilidadBase.Nombre);

            if (audioBase != null && audioSource != null)
            {
                audioSource.clip = audioBase;  // Asignar el AudioClip al AudioSource
                audioSource.Play();            // Reproducir el audio
                Debug.Log("Reproduciendo el track de la habilidad base: " + habilidadBase.Nombre);
            }
            else
            {
                Debug.LogWarning("No se pudo reproducir el audio de la habilidad base.");
            }
        }
        else
        {
            Debug.LogWarning("El personaje no tiene habilidades o no está asignado.");
        }
    }

    void Update()
    {
        DetectarInput();  // Detecta la pulsación del jugador en cada frame
    }

    private void SeleccionarPersonaje(string nuevoNombre)
    {
        if (master.DictPersonajes.TryGetValue(nuevoNombre, out Personaje p))
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
            tiempoInput = AudioSettings.dspTime;  // Captura el tiempo en que se presiona la tecla

            // Calcular el número de beats desde previousFirstBeatTime
            double beatsSinceStart = (tiempoInput - previousFirstBeatTime) / beatInterval;

            // Encontrar el número de beat más cercano
            int beatNumber = Mathf.RoundToInt((float)beatsSinceStart);

            // Calcular el tiempo del beat más cercano
            double tiempoBeatCercano = previousFirstBeatTime + beatNumber * beatInterval;

            // Calcular la diferencia de tiempo entre el input y el beat más cercano
            double diferenciaTiempo = tiempoInput - tiempoBeatCercano;

            // Calcular el desvío máximo permitido en segundos
            double tiempoDesviacion = tiempoMaxDesviacion * beatInterval;

            if (Mathf.Abs((float)diferenciaTiempo) <= tiempoDesviacion)
            {
                // Input es válido dentro del margen de desviación

                // Verificar si el beat es múltiplo de 4
                if (beatNumber % 4 == 0)
                {
                    // Input correcto en un beat múltiplo de 4
                    inputCorrectoEnBeat = true;

                    if (!habilidadActivada)
                    {
                        ActivarTrackHabilidad(habilidadEspecifica);
                        beatsSinceAbilityActivated = 0;
                        habilidadActivada = true;
                    }
                    else
                    {
                        // Habilidad ya activada, extenderla
                        beatsSinceAbilityActivated = 0;
                    }

                    Debug.Log("Input válido en beat número: " + beatNumber);
                }
                else
                {
                    // Input válido pero no en beat múltiplo de 4
                    Debug.Log("Input válido pero no en beat múltiplo de 4");
                }
            }
            else
            {
                // Input no válido
                if (habilidadActivada)
                {
                    DetenerTrackHabilidad();
                    habilidadActivada = false;
                }
                Debug.Log("Input no válido");
            }
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

        /*if(cont == 7) //Esto debería indicar que terminó el último beat del ciclo anterior, y empezó el primero del nuevo
        {
            (Habilidad habilidadDetectada, int gradoExito) = personaje.DetectarPatron(inputsTime);
            Debug.Log("Acá3");

            DetenerTrackHabilidad();
            Debug.Log("Obtuviste un puntaje de " + gradoExito);
            //Aqui debería ejecutarse la habilidad

            previousFirstBeatTime = tiempoBeat;
            cont = 0;
            habilidadActivada = false;
            if (inputsTime.Count > 0)
            {
                float lastTime = inputsTime[inputsTime.Count - 1];
                inputsTime.Clear();
                if (lastTime >= 31)
                {
                    inputsTime.Add(lastTime - 32); //Añade el ultimo input del ciclo anterior en caso de que pueda entenderse como el primero del nuevo
                }
            }
            habilidadDetectadaActual = null;
        }
        Debug.Log("tiempoBeat: " + (tiempoBeat - previousFirstBeatTime) / beatInterval * 4);
        //Esto se ejecuta inmediatamente despues de lo anterior
        if (firstBeat == true && habilidadActivada == false) //si estamos al comienzo de un compás
        {
            //notar que cont parte en 0
            Debug.Log("entra");
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
                Debug.Log("Acá1");
                DetenerTrackHabilidad();
                if (gradoExito >= 1)
                {
                    habilidadDetectadaActual = habilidadDetectada;
                    ActivarTrackHabilidad(habilidadDetectada);
                }
            }
            else if(gradoExito==0)
            {
                Debug.Log("Acá2");
                DetenerTrackHabilidad();
            }
            cont++;

            //eventualmente agregar código que muestre que hiciste bien el beat que se supone que iba en ese lugar
        }*/

        
        ultimoTiempoBeat = tiempoBeat;

        if (habilidadActivada)
        {
            beatsSinceAbilityActivated++;

            if (beatsSinceAbilityActivated >= 4)
            {
                // Han pasado 4 beats desde que se activó la habilidad
                if (inputCorrectoEnBeat)
                {
                    // El jugador presionó el input correctamente, extender la habilidad
                    beatsSinceAbilityActivated = 0;
                    inputCorrectoEnBeat = false; // Reiniciar para la próxima vez
                }
                else
                {
                    // El jugador no presionó el input correctamente, desactivar la habilidad
                    DetenerTrackHabilidad();
                    habilidadActivada = false;
                    beatsSinceAbilityActivated = 0;
                }
            }
        }
    }

    void ActivarTrackHabilidad(Habilidad habilidad)
    {
        if (habilidad != null && audioManager != null)
        {
            AudioClip trackHabilidad = audioManager.ObtenerAudioPorNombre(habilidad.Nombre);

            if (trackHabilidad != null)
            {
                float currentTime = audioSource.time; // Obtener el tiempo actual de reproducción

                audioSource.Stop(); // Detener la reproducción actual
                audioSource.clip = trackHabilidad; // Asignar el nuevo clip
                audioSource.time = currentTime; // Establecer el tiempo de reproducción
                audioSource.Play(); // Reproducir el nuevo clip

                Debug.Log("Reproduciendo track de la habilidad: " + habilidad.Nombre);
            }
            else
            {
                Debug.LogWarning("No se encontró el audio para la habilidad: " + habilidad.Nombre);
            }
        }
        else
        {
            Debug.LogWarning("Habilidad o AudioManager no están presentes.");
        }
    }

    void DetenerTrackHabilidad()
    {
        if (audioSource != null)
        {
            float currentTime = audioSource.time; // Obtener el tiempo actual de reproducción

            // Obtener la habilidad base (asumiendo que es la primera en la lista)
            if (personaje != null && personaje.Habilidades.Count > 0)
            {
                Habilidad habilidadBase = personaje.Habilidades[0];
                AudioClip baseClip = audioManager.ObtenerAudioPorNombre(habilidadBase.Nombre);

                if (baseClip != null)
                {
                    audioSource.Stop(); // Detener la reproducción actual
                    audioSource.clip = baseClip; // Asignar el clip base
                    audioSource.time = currentTime; // Establecer el tiempo de reproducción
                    audioSource.Play(); // Reproducir el clip base

                    Debug.Log("Reproduciendo el track de la habilidad base: " + habilidadBase.Nombre);
                }
                else
                {
                    Debug.LogWarning("No se pudo encontrar el audio de la habilidad base.");
                }
            }
            else
            {
                Debug.LogWarning("El personaje no tiene habilidades o no está asignado.");
            }
        }
        else
        {
            Debug.LogWarning("No hay ningún AudioSource asignado.");
        }
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
