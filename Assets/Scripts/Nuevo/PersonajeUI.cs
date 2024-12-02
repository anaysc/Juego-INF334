using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combate;
using TMPro;
using Combate.Habilidades;

public class PersonajeUI : MonoBehaviour
{
    [SerializeField] private Master master; //Referencia al master
    [SerializeField] private AudioMaster audioMaster; //Referencia al AudioMaster de la escena
    [SerializeField] private AudioSource efectoTeclaSource; // AudioSource instanciado para el sonido de tecla
    [SerializeField] private AudioSource efectoTeclaSourceBad; // AudioSource instanciado para el sonido de tecla
    [SerializeField] private TextMeshProUGUI bienTexto; // Referencia al texto que muestra "¡Bien!" usando TextMeshPro
    private float volumenDefault = 0.5f;  // Volumen estándar cuando el personaje está en base
    public float volumenReducido = 0.05f; // Volumen reducido para personajes que no están en habilidad
    private float volumenAudioMasterDefault = 0.5f; // Volumen por defecto para el AudioMaster
    private float volumenAudioMasterReducido = 0.05f;// Volumen reducido para el AudioMaster cuando hay una habilidad activa
    public Sprite spriteNormal;
    public Sprite spriteAtaque;
    public SpriteRenderer spriteRenderer;
    public bool estaRecibiendoDaño = false; // Indica si el personaje está recibiendo daño
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
    public AudioSource desactivar; //El AudioSource que esta siempre tocando la base y se mute según sea necesario
    public GameObject borde;
    private List<float> inputsTime = new List<float>(); // lista de tiempos en los que se han apretados (o no) los últimos 8 beats
    private AudioManager audioManager;
    // [SerializeField] private Animator animador; // Referencia al Animator del personaje
    public AudioSource sonidoInput; // AudioSource que se reproducirá al detectar el input
    public TextMeshProUGUI textoDaño;
    public TextMeshProUGUI textoCura;
    public TextMeshProUGUI textoBuff;
    public TextMeshProUGUI nombreHabilidad;
    public float lastHp;
    public bool estaMuerto = false; // Indica si el personaje/enemigo ya murió
    [SerializeField] private Color colorHabilidadEspecial = Color.white; // Color único para la habilidad especial

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer no encontrado en el personaje.");
        }

        audioManager = FindObjectOfType<AudioManager>();
        // borde.SetActive(false);
        // if (efectoTeclaSource == null)
        // {
        //     Debug.LogWarning("efectoTeclaSource no está asignado en el Inspector");
        // }
        // if (efectoTeclaSource != null)
        // {
        //     efectoTeclaSource.volume = 0.2f; // Ajusta el valor según prefieras
        // }

    }

    void Update()
    {
        DetectarInput();  // Detecta la pulsación del jugador en cada frame
        int ciclo = Mathf.FloorToInt(audioMaster.TimeInBeats / duracionCiclo);
        float tiempoCiclo = audioMaster.TimeInBeats % duracionCiclo;
        if(ciclo > cicloActual) //true cuando comienza un nuevo ciclo
        {
            //Aqui iba OnCiclo() pero ahora se encargara de eso el master, para asegurarse de que se ejecute antes que el master
        }
        if(tiempoCiclo >= proximoCheckeo)
        {
            CheckearHabilidad(Mathf.FloorToInt(proximoCheckeo*2 + 0.0001f)); //el 0.0001f es por si el float es muy ligeramente mas pequeño, no creo que termine siendo necesario
            proximoCheckeo += 1;
        }

        
    }
    public void OnCiclo(int ciclo)
    {
        habilidadActivada = false;
        //achicar al mono
        // borde.SetActive(false);
        cicloActual = ciclo;
        FinalizarCiclo();
        proximoCheckeo = 0.5f;
        Debug.Log("Ciclo: " + ciclo);
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
            // Reproducir el sonido de input
            if (sonidoInput != null)
            {
                sonidoInput.Play();
            }
            else
            {
                Debug.LogWarning("sonidoInput no está asignado en el Inspector");
            }
            float offset = 0.1f;
            float tiempoCiclo = audioMaster.TimeInBeats % duracionCiclo;
            inputsTime.Add(tiempoCiclo*2 - offset);
            Debug.Log("Input detectado en tiempo: " + (tiempoCiclo*2-offset));

            master.TrySeleccionar(personaje);
            // Validar si el input fue en el tiempo correcto utilizando CalcularError
        }
    }
    bool isPersonajeSeleccionado()
    {
        return master.PersonajeSeleccionado == personaje;
    }
    void FinalizarCiclo() 
    {
        if (isPersonajeSeleccionado() && master.turnoActual==TurnType.personajes) //Esto es para checkear que no se activen multiples personajes a la vez
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
            // Actualizar el texto de daño y mostrarlo en pantalla
            if (textoDaño != null && gradoExito > 0 && habilidadDetectada is Ataque atack)
            {
                textoDaño.text = "-" + atack.Damage.ToString("F1");
                textoDaño.gameObject.SetActive(true);
                StartCoroutine(OcultarTextoDaño()); // Ocultar el texto después de un tiempo
            }
            else if (habilidadDetectada != null && gradoExito > 0 && habilidadDetectada is Curacion cura)
            {
                textoCura.text = "+" + cura.Heal.ToString("F1");
                textoCura.gameObject.SetActive(true);
                StartCoroutine(OcultarTextoDaño()); // Ocultar el texto después de un tiempo
            }
            else if (habilidadDetectada != null && gradoExito > 0 && habilidadDetectada is BuffHabilidad buffHabilidad)
            {
                textoBuff.text = buffHabilidad.NombreEstado + " +"+ gradoExito;
                textoBuff.gameObject.SetActive(true);
                StartCoroutine(OcultarTextoDaño()); // Ocultar el texto después de un tiempo
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
    // Corrutina para ocultar el texto de daño después de un breve tiempo
    private IEnumerator OcultarTextoDaño()
    {
        yield return new WaitForSeconds(1.5f); // Espera 1.5 segundos
        textoDaño.gameObject.SetActive(false);
        textoCura.gameObject.SetActive(false);
        textoBuff.gameObject.SetActive(false);
    }
    void CheckearHabilidad(int largo) //Debería llamarse medio beat despues de comenzar el ciclo, y opcionalmente despues
    {
        //tiempoCiclo esta medido en corcheas (antes eran semicorcheas pero es muy rapido), y son la cantidad de elementos del patron que deberían compararse
        (Habilidad habilidadDetectada, int gradoExito) = personaje.DetectarPatron(inputsTime, largo);
        if (gradoExito > 0 && isPersonajeSeleccionado() && master.turnoActual==TurnType.personajes)
        {
            ActivarTrackHabilidad(habilidadDetectada);
            // Reproducir el sonido de tecla cada vez que se detecta una pulsación
            // if (efectoTeclaSource != null)
            // {
            //     efectoTeclaSource.Play();
            // }
            if (bienTexto != null)
            {
                bienTexto.gameObject.SetActive(true);
            }
            if (nombreHabilidad != null)
            {
                if (EsHabilidadEspecial(habilidadDetectada))
                {
                    nombreHabilidad.color = colorHabilidadEspecial; // Usa el color especial asociado
                }
                else
                {
                    nombreHabilidad.color = Color.white; // Usa un color por defecto si no es especial
                }
                nombreHabilidad.text = habilidadDetectada.nombreDisplay;
                nombreHabilidad.gameObject.SetActive(true);
            }
        }
        else
        {
            ActivarTrackBase();
            if (habilidadActivada == true)
            {
                // if (efectoTeclaSourceBad != null)
                // {
                //     efectoTeclaSourceBad.Play();
                // }
            }
            if(master.turnoActual == TurnType.enemigos)
            {
                audioSourceBase.volume = volumenAudioMasterReducido;
            }
        }
    }

    void ActivarTrackHabilidad(Habilidad habilidad)
    {
        if (habilidadActivada == false)
        {
            //hacer sonido y agrandar al mono
            activar.Play();
            // borde.SetActive(true);
            habilidadActivada = true;
            // Cambia al sprite de ataque
            if (spriteRenderer != null && spriteAtaque != null)
            {
                spriteRenderer.sprite = spriteAtaque;
            }
        }
        else
        {
            //desactivar.Play();

        }
        //Primero muteamos la habilidad base y cualquier otra habilidad que podría estar sonando
        DetenerTrackHabilidad();
        audioSourceBase.mute = true;
        AjustarVolumenOtrosPersonajes(true);
        audioMaster.masterAudioSource.volume = volumenAudioMasterReducido; // Reducir volumen del AudioMaster

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
        bienTexto.gameObject.SetActive(false);
        nombreHabilidad.gameObject.SetActive(false);
        DetenerTrackHabilidad();   
        audioSourceBase.mute = false;
        AjustarVolumenOtrosPersonajes(false);
        audioMaster.masterAudioSource.volume = volumenAudioMasterDefault; // Restaurar volumen del AudioMaster
        // borde.SetActive(false);
        // Restaurar el sprite normal cuando el track base vuelva a sonar
        if (spriteRenderer != null && spriteNormal != null)
        {
            spriteRenderer.sprite = spriteNormal;
        }
        


    }
    void AjustarVolumenOtrosPersonajes(bool reducirVolumen)
    {
        foreach (var personaje in master.ObtenerTodosPersonajes())
        {
            if (personaje != this) // Solo ajustar a otros personajes
            {
                personaje.audioSourceBase.volume = reducirVolumen ? volumenReducido : volumenDefault;
            }
        }
    }

    private bool EsHabilidadEspecial(Habilidad habilidad)
    {
        return habilidad.Nombre.Contains("Habilidad2"); // Verifica si el nombre contiene "Habilidad2"
    }

}
 