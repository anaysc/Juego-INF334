using UnityEngine;
using UnityEngine.UI;

public class ButtonPressEffect : MonoBehaviour
{
    [SerializeField] private Button targetButton; // El botón que se "presionará".
    [SerializeField] private KeyCode activationKey = KeyCode.F; // Tecla configurable para activar el botón.
    [SerializeField] private Color customPressedColor = new Color(1f, 0.3f, 0.3f); // Color personalizado (rojo).

    private bool isKeyPressed = false; // Para rastrear el estado de la tecla.

    void Update()
    {
        // Detectar si la tecla configurada está siendo presionada.
        if (Input.GetKeyDown(activationKey))
        {
            SimulateButtonPress();
            isKeyPressed = true;
        }

        // Detectar si la tecla configurada ha sido liberada.
        if (Input.GetKeyUp(activationKey))
        {
            SimulateButtonRelease();
            isKeyPressed = false;
        }
    }

    // Simula el estado de "presionado" del botón.
    private void SimulateButtonPress()
    {
        // Cambiar el color o estilo del botón para que se vea presionado.
        var colors = targetButton.colors;
        colors.normalColor = customPressedColor; // Cambia al color de presionado.
        targetButton.colors = colors;

        Debug.Log($"Botón presionado con la tecla {activationKey}");
    }

    // Simula el estado de "liberado" del botón.
    private void SimulateButtonRelease()
    {
        // Restaurar el color o estilo del botón.
        var colors = targetButton.colors;
        colors.normalColor = Color.white; // Cambia al color normal.
        targetButton.colors = colors;

        Debug.Log($"Botón liberado con la tecla {activationKey}");
    }
}
