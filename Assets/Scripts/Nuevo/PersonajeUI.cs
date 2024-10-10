using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combate;

public class PersonajeUI : MonoBehaviour
{
    [SerializeField] private Master master; //Referencia al master
    [SerializeField] private AudioMaster audioMaster; //Referencia al AudioMaster de la escena

    private int duracionCiclo = 8; //En Beats

    private int cicloActual = 0;
    private float proximoCheckeo = 0;

    public string nombreTrackBase;
    public string nombrePersonaje;       // Nombre del personaje
    public Personaje personaje;          // Referencia al objeto Personaje
    public KeyCode teclaControl;         // Tecla asignada para controlar este personaje
    // public AudioSource pistaBase;        // AudioSource para la pista base (track 1)
    // public AudioSource pistaOffbeat;     // AudioSource para la pista offbeat (track 2)
    public float tiempoMaxDesviacion = 0.2f; // Margen de desviación permitido respecto al beat
    public List<AudioSource> audioSourceHabilidades;  // El AudioSource preexistente en el GameObject
    public AudioSource audioSourceBase; //El AudioSource que esta siempre tocando la base y se mute según sea necesario

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
    private AudioManager audioManager;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        if(nombrePersonaje != "")
        {
            SeleccionarPersonaje(nombrePersonaje); //Temporalmente esto funciona así
            if (personaje == null)
            {
                Debug.LogWarning("El personaje no está asignado");
            }
        }

        /*
        // Aquí buscamos el track base del personaje y reproducimos su track
        if (personaje != null)
        {
            // Usar el AudioManager para obtener el AudioClip correspondiente
            AudioClip audioBase = audioManager.ObtenerAudioPorNombre(nombreTrackBase);

            if (audioBase != null && audioSourceBase != null)
            {
                audioSourceBase.clip = audioBase;  // Asignar el AudioClip al AudioSource
                audioSourceBase.Play();            // Reproducir el audio
                Debug.Log("Reproduciendo el track de la habilidad base: " + nombreTrackBase);
            }
            else
            {
                Debug.LogWarning("No se pudo reproducir el audio de la habilidad base.");
            }
        }
        */

    }

    void Update()
    {
        DetectarInput();  // Detecta la pulsación del jugador en cada frame
        int ciclo = Mathf.FloorToInt(audioMaster.TimeInBeats / duracionCiclo);
        float tiempoCiclo = audioMaster.TimeInBeats % duracionCiclo;
        if(ciclo > cicloActual)
        {
            cicloActual = ciclo;
            FinalizarCiclo();
            proximoCheckeo = 0.5f;
            Debug.Log("Ciclo: " + ciclo);
        }
        if(tiempoCiclo >= proximoCheckeo)
        {
            CheckearHabilidad(Mathf.FloorToInt(proximoCheckeo*2 + 0.0001f)); //el 0.0001f es por si el float es muy ligeramente mas pequeño, no creo que termine siendo necesario
            proximoCheckeo += 1;
        }

        
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
            float offset = 0;
            float tiempoCiclo = audioMaster.TimeInBeats % duracionCiclo;
            inputsTime.Add(tiempoCiclo*2 - offset);
            Debug.Log("Input detectado en tiempo: " + (tiempoCiclo*2-offset));
        }
    }
    void FinalizarCiclo() 
    {
        (Habilidad habilidadDetectada, int gradoExito) = personaje.DetectarPatron(inputsTime);
        Debug.Log("Obtuviste un puntaje de " + gradoExito);
        ActivarTrackBase();
    }

    void CheckearHabilidad(int largo) //Debería llamarse medio beat despues de comenzar el ciclo, y opcionalmente despues
    {
        //tiempoCiclo esta medido en corcheas (antes eran semicorcheas pero es muy rapido), y son la cantidad de elementos del patron que deberían compararse
        (Habilidad habilidadDetectada, int gradoExito) = personaje.DetectarPatron(inputsTime, largo);
        if (gradoExito > 0)
        {
            ActivarTrackHabilidad(habilidadDetectada);
        }
        else
        {
            ActivarTrackBase();
        }
    }

    
    //Esto se encuentra ahora deprecado. No seguirá usandose pero sigue aqui para no tirar errores de compilación hasta que se eliminen las referencias.
    void OnBeat(double tiempoBeat, bool firstBeat)
    {
        if(cont == 7) //Esto debería indicar que terminó el último beat del ciclo anterior, y empezó el primero del nuevo
        {
            (Habilidad habilidadDetectada, int gradoExito) = personaje.DetectarPatron(inputsTime);
            //Debug.Log("Acá3");

            DetenerTrackHabilidad();
            ActivarTrackBase();
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
                //Debug.Log("Acá1");
                DetenerTrackHabilidad();
                if (gradoExito >= 1)
                {
                    habilidadDetectadaActual = habilidadDetectada;
                    ActivarTrackHabilidad(habilidadDetectada);
                }
            }
            else if(gradoExito==0)
            {
                //Debug.Log("Acá2");
                DetenerTrackHabilidad();
            }
            cont++;

            //eventualmente agregar código que muestre que hiciste bien el beat que se supone que iba en ese lugar
        }

    }
    void ActivarTrackHabilidad(Habilidad habilidad)
    {
        //Primero muteamos la habilidad base y cualquier otra habilidad que podría estar sonando
        DetenerTrackHabilidad();
        audioSourceBase.mute = true;

        int indice = personaje.Habilidades.IndexOf(habilidad);
        if(indice != -1)
        {
            //Aqui desmuteamos el track de la habilidad
            audioSourceHabilidades[indice].mute = false;
        }
        else
        {
            Debug.LogWarning("habilidad: " + habilidad.Nombre + " no encontrada");
        }
    }

    void DetenerTrackHabilidad()
    {
        foreach(AudioSource audioSource in audioSourceHabilidades)
        {
            audioSource.mute = true;
        }
    }

    void ActivarTrackBase()
    {
        DetenerTrackHabilidad();   
        audioSourceBase.mute = false;
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
 