using System.Collections.Generic;
using UnityEngine;

public class BeatBarController : MonoBehaviour
{
    public RectTransform barra; // Barra horizontal
    public RectTransform marcadorPrefab; // Prefab de los marcadores
    public AudioMaster audioMaster; // Referencia al AudioMaster
    public int cantidadMarcadores = 4; // Número de marcadores a generar

    private List<RectTransform> marcadores = new List<RectTransform>();
    private float anchoBarra; // Ancho de la barra para calcular posiciones
    public float offsetVisual = -0.05f; // Ajuste para sincronizar con el beat

    void Start()
    {
        if (audioMaster == null)
        {
            Debug.LogError("AudioMaster no asignado. Por favor, asigna el AudioMaster en el Inspector.");
            return;
        }

        // Obtener el ancho de la barra horizontal
        if (barra != null)
        {
            anchoBarra = barra.rect.width;
        }

        // Crear los marcadores
        for (int i = 0; i < cantidadMarcadores; i++)
        {
            RectTransform nuevoMarcador = Instantiate(marcadorPrefab, barra);
            marcadores.Add(nuevoMarcador);

            // Inicializar los marcadores fuera de la barra
            nuevoMarcador.anchoredPosition = new Vector2(-anchoBarra / 2, 0);
        }
    }

    void Update()
    {
        if (audioMaster == null) return;

        // Obtener el tiempo actual en beats desde el AudioMaster
        float tiempoEnBeats = audioMaster.TimeInBeats/4 + offsetVisual;

        // Mover los marcadores
        for (int i = 0; i < marcadores.Count; i++)
        {
            // Calcula la posición normalizada del marcador en la barra
            float posicionNormalizada = (tiempoEnBeats + i * (1f / cantidadMarcadores)) % 1f;
            float xPos = Mathf.Lerp(-anchoBarra / 2, anchoBarra / 2, posicionNormalizada);

            // Actualiza la posición del marcador
            RectTransform marcadorRect = marcadores[i];
            marcadorRect.anchoredPosition = new Vector2(xPos, marcadorRect.anchoredPosition.y);
        }
    }
}
