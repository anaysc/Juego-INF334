using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText; // Texto para diálogos.
    [SerializeField] private GameObject nextButton; // Botón "Siguiente".
    [SerializeField] private GameObject playLevelButton; // Botón "Comenzar nivel".
    [SerializeField] private GameObject bateriaObject; // Objeto batería para activarlo.
    [SerializeField] private VideoPlayer tutorialVideo; // Video de demostración.
    [SerializeField] private int currentLevel; // Nivel actual (1, 2, 3...).
    [SerializeField] private string demoscene;
    private Dictionary<int, string[]> dialoguesByLevel;
    private string[] dialogues;
    private int currentDialogueIndex = 0;

    void Start()
    {
        InitializeDialogues(); // Configura los diálogos por nivel.

        // Validar que el nivel sea correcto y tenga diálogos definidos
        if (currentLevel < 1 || !SetDialoguesForLevel(currentLevel))
        {
            Debug.LogError($"No hay diálogos definidos para el nivel {currentLevel}. Por favor, verifica el valor de currentLevel.");
            dialogueText.text = "No hay diálogos disponibles para este nivel."; // Mostrar mensaje de error en la UI.
            nextButton.SetActive(false); // Desactivar el botón si no hay diálogos.
            return;
        }

        ShowDialogue(); // Mostrar el primer diálogo.
        playLevelButton.SetActive(false); // Ocultar el botón "Comenzar nivel" al inicio.
    }

    private void InitializeDialogues()
    {
        dialoguesByLevel = new Dictionary<int, string[]>
        {
            {
                1, new string[]
                {
                    "¡Hey, hola! Soy la guitarra, ¡pero no cualquier guitarra! Me llaman **la Guitarra del Ritmo Implacable**. Mi misión y la de mis compañeros es derrotar a los malvados enemigos que han tomado control de la **Rythm Tower**...",
                    "¿Qué por qué lo hacemos? Bueno, porque no podemos permitir que el ritmo desaparezca del mundo.",
                    "En esta torre, cada piso está protegido por un enemigo aún más fuerte que el anterior. Pero no te preocupes, ¡no estás solo! Nosotros, los instrumentos, estamos aquí para ayudarte a devolver la armonía...",
                    "Pero antes de hablar de batallas épicas y ritmos frenéticos, necesito enseñarte lo más básico: cómo atacar. Sin un buen ataque, no podremos derrotar a nadie. ¡Y créeme, no queremos quedarnos atrapados en este primer piso para siempre!",
                    "Ah, pero antes de eso... Necesitamos algo importante: una base rítmica sólida. Aquí es donde entra en escena mi compañero inseparable: **la Batería del Compás Constante**. ¡Hey, batería, es tu turno de brillar!",
                    "Aquí está. Aunque en este nivel no tocará mucho más que una base sencilla, su trabajo es vital para que no pierdas el ritmo. Escucha atentamente lo que toca y déjate guiar por su compás. ¿Entendido?",
                    "Ahora, te mostraré una demostración de lo que debes hacer. Observa bien, porque este será tu momento de brillar en unos instantes. ¡Presta atención a cómo seguir el ritmo y cuándo atacar!",
                    "... Presta atención ...",
                    "¡Y eso es todo! Ahora es tu turno. Quiero que te concentres, escuches la base de la batería y uses todo lo que te mostré para vencer a nuestro primer enemigo: **la Flauta Desafinada**.",
                    "Para asegurar que estás listo, deberás demostrar que entendiste las habilidades... ¡Te pondremos a prueba antes de comenzar con el nivel 1!"
                }
            },
            {
                2, new string[]
                {
                    "¡Bienvenido al segundo nivel! Has demostrado ser un defensor excepcional del ritmo. Pero ahora es momento de añadir un nuevo miembro al equipo: **la Batería del Compás Constante**.",
                    "La batería no será solo la base rítmica del equipo. Ahora, tendrás el control completo sobre ella. Este instrumento tiene una fuerza única: mientras mantiene el compás, puede proteger y curar a los aliados con su habilidad especial.",
                    "La habilidad base de la batería es un ataque constante y rítmico, pero su habilidad especial brilla aún más: al ejecutar correctamente su ritmo único, puede sanar a todo el equipo, devolviendo la fuerza necesaria para continuar la batalla.",
                    "Antes de enfrentarte al próximo enemigo, observa cómo usar la batería en acción. ¡Aprecia cómo su habilidad especial devuelve la vida al equipo!",
                    "... Presta atención ...",
                    "Ahora que conoces las habilidades de la batería, es tu turno de actuar. En este nivel, deberás usar tanto a la guitarra como a la batería para derrotar a la desafinada y caótica **Trompeta Estridente**. ¡El destino del ritmo está en tus manos!"

                }
            },
            {
                3, new string[]
                {
                    "¡Felicidades por llegar al tercer nivel de la **Rythm Tower**! Es momento de conocer a dos nuevos aliados que se unirán a nuestro equipo: **el Bajo del Escudo Melódico** y **el Violín de la Armonía Letal**.",
                    "Primero, déjame presentarte al bajo. Este poderoso instrumento tiene una habilidad especial que otorga un buffeo de defensa para todos los aliados. Cuando lo uses correctamente, su melodía protegerá al equipo de los ataques más fuertes.",
                    "Y ahora, el violín. Este elegante pero letal instrumento es capaz de desatar un ataque especial que aumenta la fuerza de todos los aliados. Su habilidad especial otorga un buffeo de ataque, permitiendo que el equipo inflija un daño devastador a los enemigos.",
                    "La clave en este nivel será aprender a usar tanto el bajo como el violín en armonía con la guitarra y la batería. Observa atentamente cómo combinamos sus habilidades base y especiales para aprovechar al máximo sus efectos en el combate.",
                    "... Presta atención ...",
                    "Ahora que conoces a todos los instrumentos, es tu turno de actuar. En este nivel, deberás usar a la guitarra, la batería, el bajo y el violín para enfrentarte al temido **Sintetizador Malvado**. ¡El destino del ritmo está en tus manos!"
                }
            }
        };
    }

    private bool SetDialoguesForLevel(int level)
    {
        if (dialoguesByLevel.ContainsKey(level))
        {
            dialogues = dialoguesByLevel[level];
            return true; // Diálogos encontrados.
        }

        dialogues = new string[0]; // Evita errores si no hay diálogos definidos.
        return false; // Diálogos no encontrados.
    }

    public void OnNextButtonPressed()
    {
        if (currentDialogueIndex < dialogues.Length - 1 && currentLevel == 1)
        {
            currentDialogueIndex++;
            ShowDialogue();

            // Activar la batería en el nivel 1 y en el diálogo correspondiente.
            if (currentLevel == 1 && currentDialogueIndex == 4)
            {
                bateriaObject.SetActive(true);
            }
        }
        if (currentDialogueIndex == 7 && currentLevel == 1) // Video
        {
            ShowVideo();
        }

        if (currentLevel == 2 || currentLevel == 3)
        {
            currentDialogueIndex++;
            ShowDialogue();
        }
        if (currentDialogueIndex == 4 && currentLevel == 2) // Video.
        {
            ShowVideo();
        }
        if (currentDialogueIndex == 4 && currentLevel == 3) // Video.
        {
            ShowVideo();
        }
    }

    private void ShowDialogue()
    {
        if (dialogues.Length > 0)
        {
            dialogueText.text = dialogues[currentDialogueIndex];
        }
        else
        {
            Debug.LogWarning("No hay diálogos para mostrar.");
        }
    }

    private void ShowVideo()
    {
        MuteMusic();
        // nextButton.SetActive(false); // Ocultar el botón "Siguiente".
        tutorialVideo.gameObject.SetActive(true); // Activar el video.
        tutorialVideo.Play();

        // Esperar a que termine el video para continuar.
        Invoke(nameof(EndVideo), (float)tutorialVideo.length);
    }

    private void EndVideo()
    {
        UnmuteMusic();
        tutorialVideo.gameObject.SetActive(false); // Ocultar el video.
        dialogueText.text = "Para asegurar que estás listo, deberás demostrar que entendiste las habilidades... ¡Te pondremos a prueba antes de comenzar con el nivel 1!";
        playLevelButton.SetActive(true); // Mostrar el botón para jugar.
        nextButton.SetActive(false);

    }

    public void OnPlayLevelButtonPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(demoscene);
    }
    void MuteMusic()
    {
        MusicManager musicManager = FindObjectOfType<MusicManager>();
        if (musicManager != null)
        {
            musicManager.MuteMusic();
        }
    }
    void UnmuteMusic()
    {
        MusicManager musicManager = FindObjectOfType<MusicManager>();
        if (musicManager != null)
        {
            musicManager.UnmuteMusic();
        }
    }

}
