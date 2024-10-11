using System.Collections;
using UnityEngine;

public class BPMVisibilityController : MonoBehaviour
{
    public GameObject targetObject; // El objeto que aparecerá y desaparecerá.
    // public AudioSource metronomeAudio; // AudioSource que reproduce el metrónomo.
    public float bpm = 129f; // BPM del metrónomo.

    private double intervalBetweenBeats; // Intervalo entre beats en segundos.
    private double nextBeatTime; // Tiempo exacto del próximo beat.
    private bool isVisible = false; // Estado actual de visibilidad del objeto.

    void Start()
    {
        // if (metronomeAudio == null)
        // {
        //     Debug.LogError("AudioSource del metrónomo no asignado.");
        //     return;
        // }

        // Calcula el intervalo entre beats usando BPM y DSP Time.
        intervalBetweenBeats = 60.0 / bpm;
        nextBeatTime = AudioSettings.dspTime + intervalBetweenBeats; // Establece el tiempo del primer beat.
        // metronomeAudio.Play(); // Reproduce el audio al iniciar.
    }

    void Update()
    {
        // Verifica si el tiempo DSP ha alcanzado el tiempo del siguiente beat.
        if (AudioSettings.dspTime >= nextBeatTime)
        {
            ToggleVisibility(); // Alterna la visibilidad del objeto.
            nextBeatTime += intervalBetweenBeats; // Calcula el tiempo para el siguiente beat.
        }
    }

    // Alterna la visibilidad del objeto.
    private void ToggleVisibility()
    {
        isVisible = !isVisible;
        targetObject.SetActive(isVisible); // Activa o desactiva el objeto.
    }
}
