using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMaster : MonoBehaviour
{
    public AudioSource masterAudioSource; //La AudioSource con la pista principal, todas las demás le hacen caso
    public List<AudioSource> subAudioSources = new List<AudioSource>(); //Todas los demás AudioSource que deben sincronizarse. Por Ahora Hay que asignarlos en el inspector

    [SerializeField] private float bpm = 129f;
    [SerializeField] private int sampleRate = 44100; 
    public int TimeSamples { get => masterAudioSource.timeSamples; }
    public float SecondsPerBeat { get => 60f / bpm; }
    public float TimeInBeats { get => TimeSamples / (sampleRate * SecondsPerBeat); }

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
        SyncSources();
    }
    private void SyncSources()
    {
        foreach (AudioSource source in subAudioSources)
        {
            source.timeSamples = masterAudioSource.timeSamples;
        }
    }


}
