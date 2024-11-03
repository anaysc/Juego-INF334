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
    private bool habilidadActivada = false;
    public string nombreTrackBase;
    public string nombrePersonaje;       // Nombre del personaje
    public Personaje personaje;          // Referencia al objeto Personaje
    public KeyCode teclaControl;         // Tecla asignada para controlar este personaje
    public List<AudioSource> audioSourceHabilidades;  // El AudioSource preexistente en el GameObject
    public AudioSource audioSourceBase; //El AudioSource que esta siempre tocando la base y se mute según sea necesario
    public AudioSource activar; //El AudioSource que esta siempre tocando la base y se mute según sea necesario

    public GameObject borde;
    private List<float> inputsTime = new List<float>(); // lista de tiempos en los que se han apretados (o no) los últimos 8 beats
    private AudioManager audioManager;
    // [SerializeField] private Animator animador; // Referencia al Animator del personaje

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        borde.SetActive(false);

        /*
        if(nombrePersonaje != "")
        {
            SeleccionarPersonaje(nombrePersonaje); //Temporalmente esto funciona así
            if (personaje == null)
            {
                Debug.LogWarning("El personaje no está asignado");
            }
        }
        */

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
        if(ciclo > cicloActual) //true cuando comienza un nuevo ciclo
        {
            habilidadActivada = false;
            //achicar al mono
            borde.SetActive(false);
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

    private void SeleccionarPersonaje(string nuevoNombre) //Esta version de la funcion no se va a usar desde que la seleccion de personajes este a cargo del Master (preguntar a GAT por aclaración)
    {
        if(master.DictPersonajes.TryGetValue(nuevoNombre, out Personaje p))
        {
            SeleccionarPersonaje(p);
        }
        else
        {
            Debug.Log("No se encontró el personaje: " + nuevoNombre);
        }
    }
    public void SeleccionarPersonaje(Personaje p)
    {
        nombrePersonaje = p.Nombre;
        personaje = p;
    }

    // Detecta la tecla asignada al personaje
    void DetectarInput()
    {
        if (Input.GetKeyDown(teclaControl))  // Si el jugador presiona la tecla asignada
        {
            float offset = 0.1f;
            float tiempoCiclo = audioMaster.TimeInBeats % duracionCiclo;
            inputsTime.Add(tiempoCiclo*2 - offset);
            Debug.Log("Input detectado en tiempo: " + (tiempoCiclo*2-offset));

            master.TrySeleccionar(personaje);
        }
    }
    void FinalizarCiclo() 
    {
        if (personaje.seleccionado) //Esto es para checkear que no se activen multiples personajes a la vez
        {
            (Habilidad habilidadDetectada, int gradoExito) = personaje.DetectarPatron(inputsTime);
            Debug.Log("Obtuviste un puntaje de " + gradoExito);

            //Aqui se activan los efectos mecanicos de la habilidad
            if (habilidadDetectada != null && gradoExito > 0)
            {
                if (habilidadDetectada.TryActivar(master, personaje, gradoExito))
                {
                    Debug.Log("Habilidad " + habilidadDetectada.Nombre + " Activada Correctamente");
                }
                else
                {
                    Debug.Log("Habilidad " + habilidadDetectada.Nombre + " no se pudo activar");
                }
            }
        }


        ActivarTrackBase();

        float lastInput = -1;
        if (inputsTime.Count > 0) {
            lastInput = inputsTime[inputsTime.Count-1];
        }
        inputsTime.Clear();
        if(lastInput >= 15) 
        {
            inputsTime.Add(lastInput-(duracionCiclo*2));
        }
    }

    void CheckearHabilidad(int largo) //Debería llamarse medio beat despues de comenzar el ciclo, y opcionalmente despues
    {
        //tiempoCiclo esta medido en corcheas (antes eran semicorcheas pero es muy rapido), y son la cantidad de elementos del patron que deberían compararse
        (Habilidad habilidadDetectada, int gradoExito) = personaje.DetectarPatron(inputsTime, largo);
        if (gradoExito > 0 && personaje.seleccionado)
        {
            ActivarTrackHabilidad(habilidadDetectada);
        }
        else
        {
            ActivarTrackBase();
        }
    }

    void ActivarTrackHabilidad(Habilidad habilidad)
    {
        if (habilidadActivada == false)
        {
            //hacer sonido y agrandar al mono
            activar.Play();
            borde.SetActive(true);
            habilidadActivada = true;
        }
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


}
 