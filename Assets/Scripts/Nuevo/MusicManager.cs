using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance; // Singleton instance
    private AudioSource audioSource;

    
    void Awake()
    {
        // Implementar el patrón singleton para evitar duplicados
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Si ya existe una instancia, destruir este objeto
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Evitar que el objeto se destruya al cambiar de escena
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        // Cargar la escena principal
        SceneManager.LoadScene("MenuPrincipal");
    }
    // Método para reproducir una nueva pista
    public void PlayNewTrack(AudioClip newClip)
    {
        if (audioSource.isPlaying && audioSource.clip == newClip)
        {
            return; // No hacer nada si la pista ya está sonando
        }

        audioSource.Stop(); // Detener la música actual
        audioSource.clip = newClip; // Asignar la nueva pista
        audioSource.Play(); // Reproducir la nueva pista
    }

    // Método para detener la música
    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    // Método para mutear la música
    public void MuteMusic()
    {
        audioSource.mute = true; // Mutea la música
    }

    // Método para desmutear la música
    public void UnmuteMusic()
    {
        audioSource.mute = false; // Desmutea la música
    }

    // Método para verificar si la música está muteada
    public bool IsMuted()
    {
        return audioSource.mute;
    }
}
