using System.Collections;
using UnityEngine;

public class BPMVisibilityController : MonoBehaviour
{
    public GameObject targetObject; // El objeto que se agrandará.
    public AudioMaster audioMaster; // Referencia al AudioMaster que se debe asignar en el inspector
    public float escalaMaxima = 1.5f; // Tamaño máximo cuando se agranda
    public float duracionAgrandar = 0.2f; // Duración del efecto de agrandar
    public float duracionReducir = 0.2f; // Duración del efecto de reducir

    private int beatActual = 0;
    private Vector3 escalaOriginal;
    public float offsetVisual = -0.05f; // Ajuste para sincronizar con el beat


    private void Start()
    {
        if (audioMaster == null)
        {
            Debug.LogWarning("Audio Master no asignado");
        }

        if (targetObject != null)
        {
            escalaOriginal = targetObject.transform.localScale; // Guardar la escala original del objeto
        }
    }

    void Update()
    {
        int beat = Mathf.FloorToInt(audioMaster.TimeInBeats/2 + offsetVisual);
        if (beat > beatActual)
        {
            // Deberíamos llegar aquí dentro una vez por beat
            beatActual = beat;
            StartCoroutine(AgrandarYReducir());
        }
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