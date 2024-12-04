using System.Collections;
using UnityEngine;

public class BPMVisibilityController : MonoBehaviour
{
    public GameObject targetObject; // El objeto que se agrandará.
    public AudioMaster audioMaster; // Referencia al AudioMaster que se debe asignar en el inspector
    public float escalaMaxima = 1.5f; // Tamaño máximo cuando se agranda
    public float duracionAgrandar = 0.2f; // Duración del efecto de agrandar
    public float duracionReducir = 0.2f; // Duración del efecto de reducir
    public string patron = "cx nx xn xx cx nx xn xx"; // Patrón rítmico
    private float segundosPorCorchea; // Duración de una corchea
    private float tiempoInicioBeat; // Tiempo del inicio del beat actual
    private int indicePatron = 0; // Índice del patrón actual
    private string[] elementosPatron; // Array de elementos del patrón
    private float offsetInicial; // Ajuste del tiempo para sincronización
    private Vector3 escalaOriginal;
    public float offsetVisual = +0.02f; // Ajuste para sincronizar con el beat

    private void Start()
    {
        if (audioMaster == null)
        {
            Debug.LogError("AudioMaster no asignado.");
            return;
        }

        if (targetObject != null)
        {
            escalaOriginal = targetObject.transform.localScale; // Guardar la escala original del objeto
        }

        // Dividir el patrón en partes
        elementosPatron = patron.Split(' ');

        // Calcular la duración de una corchea
        segundosPorCorchea = audioMaster.SecondsPerBeat / 2f;

        // Calcular el offset inicial basado en el tiempo actual del audio
        offsetInicial = audioMaster.TimeInBeats * audioMaster.SecondsPerBeat;
        tiempoInicioBeat = offsetInicial; // Asegurar que el primer beat esté sincronizado
    }

    void Update()
    {
        // Tiempo actual en segundos ajustado con el offset inicial
        float tiempoActual = ((audioMaster.TimeInBeats/2 + offsetVisual) * audioMaster.SecondsPerBeat) - offsetInicial;

        // Verificar si es tiempo de procesar el próximo elemento del patrón
        if (tiempoActual >= tiempoInicioBeat + segundosPorCorchea)
        {
            tiempoInicioBeat += segundosPorCorchea; // Mover al siguiente segmento de tiempo

            // Procesar el elemento actual del patrón
            ProcesarElementoPatron();

            // Avanzar al siguiente elemento
            indicePatron = (indicePatron + 1) % elementosPatron.Length;
        }
    }

    private void ProcesarElementoPatron()
    {
        string elemento = elementosPatron[indicePatron];

        switch (elemento)
        {
            case "cx": // Corchea
                StartCoroutine(AgrandarYReducir());
                break;

            case "nx": // Primera mitad del beat
                Invoke(nameof(IniciarAgrandar), 0f); // Sin retraso
                break;

            case "xn": // Segunda mitad del beat
                Invoke(nameof(IniciarAgrandar), segundosPorCorchea); // Retraso de media corchea
                break;

            case "xx": // Silencio
                // No hacer nada en caso de silencio
                break;

            default:
                Debug.LogWarning($"Elemento desconocido en el patrón: {elemento}");
                break;
        }
    }

    private void IniciarAgrandar()
    {
        StartCoroutine(AgrandarYReducir());
    }

    // Corutina para agrandar y reducir el objeto
    private IEnumerator AgrandarYReducir()
    {
        if (targetObject == null) yield break;

        // Agrandar
        float tiempo = 0f;
        while (tiempo < duracionAgrandar)
        {
            tiempo += Time.deltaTime;
            float escala = Mathf.Lerp(escalaOriginal.x, escalaMaxima, tiempo / duracionAgrandar);
            targetObject.transform.localScale = new Vector3(escala, escala, escala);
            yield return null;
        }

        // Reducir
        tiempo = 0f;
        while (tiempo < duracionReducir)
        {
            tiempo += Time.deltaTime;
            float escala = Mathf.Lerp(escalaMaxima, escalaOriginal.x, tiempo / duracionReducir);
            targetObject.transform.localScale = new Vector3(escala, escala, escala);
            yield return null;
        }

        // Asegurarse de que la escala regrese exactamente a la original
        targetObject.transform.localScale = escalaOriginal;
    }
}
