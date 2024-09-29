using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedAudio : MonoBehaviour
{
    private AudioSource audioSource;
    public float delay = 2f; // Tiempo en segundos que quieres retrasar el inicio.

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Obtener automáticamente el AudioSource.
        audioSource.PlayDelayed(delay); // Retrasar la reproducción del audio.
    }
}
