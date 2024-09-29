using System.Collections;
using UnityEngine;

public class MetronomeSync : MonoBehaviour
{
    public GameObject targetObject; // El objeto que aparecerá y desaparecerá.
    public GameObject targetObject1; // Objeto que marca correcto y malo.
    public AudioSource metronomeAudio; // AudioSource que reproduce el metrónomo.
    public KeyCode keyToPressBeat = KeyCode.Q; // Tecla para detectar presiones en el beat.
    public KeyCode keyToPressOffBeat = KeyCode.W; // Tecla para detectar presiones en el beat y offbeat.
    public float bpm = 129f; // BPM del metrónomo.
    public float tolerance = 0.1f; // Tolerancia en segundos para detectar el "Bien!".

    private double intervalBetweenBeats; // Intervalo entre beats basado en los BPM.
    private double nextBeatTime; // Tiempo exacto del próximo beat.
    private double nextOffBeatTime; // Tiempo exacto del próximo offbeat (contratiempo).
    private bool isVisible = false; // Estado actual de visibilidad del objeto.

    void Start()
    {
        if (metronomeAudio == null)
        {
            Debug.LogError("AudioSource del metrónomo no asignado.");
            return;
        }

        // Calcula el intervalo entre beats usando BPM.
        intervalBetweenBeats = 60.0 / bpm;
        nextBeatTime = AudioSettings.dspTime + intervalBetweenBeats; // Establece el tiempo del primer beat.
        nextOffBeatTime = nextBeatTime + intervalBetweenBeats / 2.0; // Establece el tiempo del primer offbeat.

        // Inicia la reproducción del audio del metrónomo en el tiempo DSP calculado.
        metronomeAudio.PlayScheduled(nextBeatTime);
    }

    void Update()
    {
        double currentDspTime = AudioSettings.dspTime; // Tiempo actual DSP para sincronización.

        // Verifica si el tiempo DSP ha alcanzado el tiempo del siguiente beat.
        if (currentDspTime >= nextBeatTime)
        {
            ToggleVisibility(); // Alterna la visibilidad del objeto.
            nextBeatTime += intervalBetweenBeats; // Calcula el tiempo para el siguiente beat.
            nextOffBeatTime = nextBeatTime - intervalBetweenBeats / 2.0; // Ajusta el tiempo del siguiente offbeat.
        }

        // Detección de la tecla para el beat exacto con la tecla Q.
        if (Input.GetKeyDown(keyToPressBeat))
        {
            if (Mathf.Abs((float)(currentDspTime - nextBeatTime + intervalBetweenBeats)) <= tolerance)
            {
                Debug.Log("Beat Bien!"); // Apretaste en sincronía con el beat.
                targetObject1.SetActive(true); // Marca como correcto.
            }
            else
            {
                Debug.Log("Fuera de tiempo."); // No apretaste en el momento correcto.
                targetObject1.SetActive(false); // Marca como incorrecto.
            }
        }

        // Detección de la tecla para el beat o offbeat con la tecla W.
        if (Input.GetKeyDown(keyToPressOffBeat))
        {
            // Detección del beat principal.
            if (Mathf.Abs((float)(currentDspTime - nextBeatTime + intervalBetweenBeats)) <= tolerance)
            {
                Debug.Log("Beat Bien!"); // Apretaste en sincronía con el beat.
                targetObject1.SetActive(true); // Marca como correcto.
            }
            // Detección del contratiempo (offbeat).
            else if (Mathf.Abs((float)(currentDspTime - nextOffBeatTime)) <= tolerance)
            {
                Debug.Log("Offbeat Bien!"); // Apretaste en sincronía con el contratiempo.
                targetObject1.SetActive(true); // Marca como correcto.
            }
            else
            {
                Debug.Log("Fuera de tiempo."); // No apretaste en el momento correcto.
                targetObject1.SetActive(false); // Marca como incorrecto.
            }
        }
    }

    // Alterna la visibilidad del objeto.
    private void ToggleVisibility()
    {
        isVisible = !isVisible;
        targetObject.SetActive(isVisible); // Activa o desactiva el objeto.
    }
}
