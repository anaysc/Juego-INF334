using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Diccionario que mapea el nombre de la habilidad al AudioClip
    public Dictionary<string, AudioClip> habilidadAudios;

    // Carpeta donde estarán los archivos de audio (dentro de Assets/Resources)
    public string audioFolder = "HabilidadesAudio"; // "Resources/HabilidadesAudio"

    // Inicializa el diccionario y carga los audios
    void Start()
    {
        habilidadAudios = new Dictionary<string, AudioClip>();
        CargarAudios();
    }

    // Método para cargar los audios desde la carpeta de Resources
    void CargarAudios()
    {
        // Cargar todos los archivos de audio en la carpeta especificada
        AudioClip[] clips = Resources.LoadAll<AudioClip>(audioFolder);

        // Iterar sobre cada clip cargado
        foreach (AudioClip clip in clips)
        {
            // Agregar el clip al diccionario, utilizando el nombre del clip como clave
            habilidadAudios.Add(clip.name, clip);
        }

        Debug.Log("Se han cargado " + habilidadAudios.Count + " audios.");
    }

    // Método para obtener un AudioClip por el nombre de la habilidad
    public AudioClip ObtenerAudioPorNombre(string nombreHabilidad)
    {
        if (habilidadAudios.ContainsKey(nombreHabilidad))
        {
            return habilidadAudios[nombreHabilidad];
        }
        else
        {
            Debug.LogWarning("No se encontró audio para la habilidad: " + nombreHabilidad);
            return null;
        }
    }
}
