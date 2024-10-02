using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cronometro : MonoBehaviour
{
    public AudioSource tickAudioSource; // AudioSource que reproduce el sonido de "tick".
    public float bpm = 129f; // Beats por minuto.
    public double lastBeat { get; private set; }  // Almacena el tiempo del último beat en dspTime
    private double nextTickTime; // Tiempo del siguiente tick en DSP.

    // Modificamos el evento OnBeat para que emita el tiempo del beat
    public event System.Action<double> OnBeat; // Evento que notifica a otras clases sobre el "beat".

    void Start()
    {
        if (tickAudioSource == null)
        {
            Debug.LogError("AudioSource para el tick no asignado.");
            return;
        }

        // Calcula el intervalo entre beats (ticks) en segundos.
        double intervalBetweenTicks = 60.0 / bpm;

        // Calcula el tiempo del primer tick (justo después de que inicie el DSP).
        nextTickTime = AudioSettings.dspTime + 0.1f; // Un pequeño margen inicial.

        // Programa el primer tick.
        tickAudioSource.PlayScheduled(nextTickTime);

        // Calcula el tiempo para el siguiente tick.
        nextTickTime += intervalBetweenTicks;

        lastBeat = nextTickTime - intervalBetweenTicks;  // Inicializa el tiempo del primer beat
    }

    void Update()
    {
        // Revisa si el tiempo DSP ha alcanzado el tiempo programado para el siguiente tick.
        if (AudioSettings.dspTime >= nextTickTime)
        {
            // Reproduce el siguiente tick.
            tickAudioSource.PlayScheduled(nextTickTime);

            lastBeat = nextTickTime; 
            // Dispara el evento OnBeat para sincronizar con otras clases.
            OnBeat?.Invoke(lastBeat);  // Pasa el tiempo del beat como parámetro

            // Calcula el tiempo para el siguiente tick.
            double intervalBetweenTicks = 60.0 / bpm;
            nextTickTime += intervalBetweenTicks;
        }
    }
}
