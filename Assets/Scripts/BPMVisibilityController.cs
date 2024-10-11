using System.Collections;
using UnityEngine;

public class BPMVisibilityController : MonoBehaviour
{
    public GameObject targetObject; // El objeto que aparecerá y desaparecerá.
    public AudioMaster audioMaster; //Referencia al AudioMaster que se debe asignar en el inspector

    private bool isVisible = true;
    private int beatActual = 0;

    private void Start()
    {
        if(audioMaster == null)
        {
            Debug.LogWarning("Audio Master no asignado");
        }
    }

    void Update()
    {
        int beat = Mathf.FloorToInt(audioMaster.TimeInBeats);
        if(beat > beatActual)
        {
            //Deberiamos llegar aqui dentro una vez por beat
            beatActual = beat;
            ToggleVisibility();
        }
    }

    // Alterna la visibilidad del objeto.
    private void ToggleVisibility()
    {
        isVisible = !isVisible;
        targetObject.SetActive(isVisible); // Activa o desactiva el objeto.
    }
}
