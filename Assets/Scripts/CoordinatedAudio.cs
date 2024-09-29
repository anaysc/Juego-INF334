using UnityEngine;

public class CoordinatedAudio : MonoBehaviour
{
    public AudioSource audioSource1; // Primer AudioSource
    public AudioSource audioSource2; // Segundo AudioSource
    public float delayBetweenAudios = 0f; // Tiempo de retraso entre los audios en segundos.

    void Start()
    {
        if (audioSource1 == null || audioSource2 == null)
        {
            Debug.LogError("Asegúrate de asignar ambos AudioSources en el Inspector.");
            return;
        }

        // Obtener el tiempo DSP actual para sincronizar ambos audios.
        double startTime = AudioSettings.dspTime + 0.1; // Añade un pequeño margen para garantizar que se programen correctamente.

        // Programar el primer audio para que se reproduzca en el tiempo especificado.
        audioSource1.PlayScheduled(startTime);

        // Programar el segundo audio con el delay especificado respecto al primer audio.
        audioSource2.PlayScheduled(startTime + delayBetweenAudios);
    }
}
