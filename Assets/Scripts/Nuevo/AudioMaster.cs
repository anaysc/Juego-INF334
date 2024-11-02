using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMaster : MonoBehaviour
{
    public AudioSource masterAudioSource; //La AudioSource con la pista principal, todas las dem�s le hacen caso
    public List<AudioSource> subAudioSources = new List<AudioSource>(); //Todas los dem�s AudioSource que deben sincronizarse. Por Ahora Hay que asignarlos en el inspector

    [SerializeField] private float bpm = 129f;
    [SerializeField] private int sampleRate = 44100; 
    public int TimeSamples { get => masterAudioSource.timeSamples; }
    public float SecondsPerBeat { get => 60f / bpm; }
    public float TimeInBeats { get => TimeSamples / (sampleRate * SecondsPerBeat); }
    // Propiedad que devuelve si el AudioSource principal ya terminó de reproducirse
    public bool HaTerminado 
    { 
        get => !masterAudioSource.isPlaying && masterAudioSource.time >= masterAudioSource.clip.length; 
    }
    public void Start()
    {
        masterAudioSource.Play();
        foreach (AudioSource source in subAudioSources)
        {
            source.Play();
        }
    }

    private void Update()
    {
        if (!HaTerminado)
        {
            SyncSources();
        }
    }
    private void SyncSources()
    {
        foreach (AudioSource source in subAudioSources)
        {
            source.timeSamples = masterAudioSource.timeSamples;
        }
    }


}
