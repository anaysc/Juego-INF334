using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Para cargar escenas


namespace Combate
{
    public class MasterTutorial : Master
    {
        [SerializeField] private TextMeshProUGUI mensajeTutorial; // Texto que mostrará "¡Bien hecho!"
        public Button jugar; 

        private int contadorHabilidad1 = 0;
        private int contadorHabilidad2 = 0;
        private bool habilidad1Completada = false;
        private bool habilidad2Completada = false;
        public PersonajeUI personajeObjetivo; // Personaje al que se le aplica la verificación
        public string nombreHabilidad1; // Nombre de la primera habilidad
        public string nombreHabilidad2; // Nombre de la segunda habilidad
        public string Nivel;
        void Start(){
            jugar.onClick.AddListener(() => CambiarEscena(Nivel));
            jugar.gameObject.SetActive(false);

        }
        public override void Awake()
        {
            // Llama al método Awake del Master para inicializar habilidades y personajes
            base.Awake();

            // No inicializamos enemigos en el tutorial
            Debug.Log("Tutorial iniciado: Solo los jugadores están activos.");
        }

        private void Update()
        {
            turnoActual = TurnType.personajes;
            int ciclo = Mathf.FloorToInt(audioMaster.TimeInBeats / duracionCiclo);
            if (ciclo > cicloActual) // true cuando comienza un nuevo ciclo
            {
                cicloActual = ciclo;

                foreach (PersonajeUI personajeUI in slotsPersonajesUI)
                {
                    (string nombre, int grado)  = personajeUI.OnCiclo(ciclo);
                    VerificarHabilidad(nombre,grado);

                }
                
                OnCiclo();
            }
        }

        public override void InitEnemigos()
        {
            // Sobrescribimos InitEnemigos para no inicializar enemigos
            Debug.Log("No se inicializan enemigos en el tutorial.");
        }

        public override void OnCiclo()
        {
            // En el tutorial no necesitamos manejar ciclos ni turnos
            Debug.Log("Ciclo ignorado en el tutorial. Los jugadores tienen control continuo.");
        }

        private void VerificarHabilidad(string nombre, int grado)
        {
            if (personajeObjetivo == null)
            {
                Debug.LogWarning("No se ha asignado un personaje objetivo para el tutorial.");
                return;
            }


            if (!habilidad1Completada)
            {
                if (nombre == nombreHabilidad1 && grado > 2)
                {
                    contadorHabilidad1++;
                    Debug.Log($"Habilidad {nombreHabilidad1} ejecutada correctamente {contadorHabilidad1} veces.");
                    mensajeTutorial.text = "Realiza la habilidad base de " + personajeObjetivo.personaje.Nombre + " (" +(3-contadorHabilidad1).ToString() +")";

                    if (contadorHabilidad1 >= 3)
                    {
                        habilidad1Completada = true;
                        MostrarMensaje($"¡Bien hecho con {nombreHabilidad1}!");
                    }
                }
            }
            else if (!habilidad2Completada)
            {
                if (nombre == nombreHabilidad2 && grado > 2)
                {
                    contadorHabilidad2++;
                    Debug.Log($"Habilidad {nombreHabilidad2} ejecutada correctamente {contadorHabilidad2} veces.");
                    mensajeTutorial.text = "Realiza la habilidad especial de " + personajeObjetivo.personaje.Nombre + " (" +(3-contadorHabilidad2).ToString() +")";

                    if (contadorHabilidad2 >= 3)
                    {
                        habilidad2Completada = true;
                        MostrarMensaje($"¡Bien hecho con {nombreHabilidad2}!");
                    }
                }
            }
        }

        private void MostrarMensaje(string mensaje)
        {
            if (mensajeTutorial != null)
            {
                mensajeTutorial.text = mensaje;
                mensajeTutorial.gameObject.SetActive(true);

                // Ocultar el mensaje después de 2 segundos
                Invoke(nameof(ActualizarMensaje), 2f);

            }
        }

        private void ActualizarMensaje()
        {
            if (mensajeTutorial != null)
            {
                // Caso 1: Finalizó Habilidad 1, pero aún no comienza Habilidad 2
                if (contadorHabilidad2 == 0 && contadorHabilidad1 == 3)
                {
                    mensajeTutorial.text = "Realiza la habilidad especial de " + personajeObjetivo.personaje.Nombre + " (3)";
                }
                // Caso 2: Finalizó Habilidad 2, activamos el botón "Jugar"
                else if (contadorHabilidad2 == 3)
                {
                    mensajeTutorial.gameObject.SetActive(false); // Ocultar mensaje
                    if (jugar != null)
                    {
                        jugar.gameObject.SetActive(true); // Activar el botón "Jugar"
                    }
                }
            }
        }


        public void AsignarPersonajeObjetivo(PersonajeUI personajeUI)
        {
            personajeObjetivo = personajeUI;

            // Asignar los nombres de las habilidades a verificar
            if (personajeObjetivo != null && personajeObjetivo.personaje != null)
            {
                var habilidades = personajeObjetivo.personaje.Habilidades;
                if (habilidades.Count >= 2)
                {
                    nombreHabilidad1 = habilidades[0].Nombre; // Primera habilidad
                    nombreHabilidad2 = habilidades[1].Nombre; // Segunda habilidad
                    Debug.Log($"Habilidades asignadas: {nombreHabilidad1}, {nombreHabilidad2}");
                }
                else
                {
                    Debug.LogWarning("El personaje objetivo no tiene suficientes habilidades asignadas.");
                }
            }
        }
        void CambiarEscena(string nombreEscena)
        {
            // Restablecer el tiempo de juego antes de recargar

            // Cambiar a la escena especificada
            SceneManager.LoadScene(nombreEscena);
        }
    }
    
}
