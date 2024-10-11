using Lector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Combate
{
    public class Master : MonoBehaviour
    {
        //Data
        [SerializeField] private TextAsset archivoHabilidades;
        [SerializeField] private TextAsset archivoPersonajes;
        [SerializeField] private TextAsset archivoEnemigos;

        private Dictionary<string, Habilidad> dictHabilidades;
        private Dictionary<string, Personaje> dictPersonajes;
        private Dictionary<string, Enemigo> dictEnemigos;

        private List<Personaje> slotsPersonajes = new List<Personaje>(); //Los 4 slots de personajes
        private List<Enemigo> slotsEnemigos = new List<Enemigo>(); //Los 4 slots de enemigos

        [SerializeField] private List<PersonajeUI> slotsPersonajesUI = new List<PersonajeUI>(); //Asignarlos en el inspector
        [SerializeField] private List<EnemigoUI> slotsEnemigosUI = new List<EnemigoUI>();

        [SerializeField] private List<string> nombresPersonajes; //Escribirlos en el inspector
        [SerializeField] private List<string> nombresEnemigos;


        //Encapsulamos las listas y ahora son readonly
        public List<Personaje> Personajes { get => slotsPersonajes; }
        public List<Enemigo> Enemigos { get => slotsEnemigos; }
        public Dictionary<string, Habilidad> DictHabilidades { get => dictHabilidades; }
        public Dictionary<string, Personaje> DictPersonajes { get => dictPersonajes; }

        private void Awake()
        {
            //Lo primero que se hace es que se leen los archivos con los datos sobre habilidades y personajes y se crean las instancias de las clases correspondientes...
            //guardandolas en diccionarios
            string[] lineas = archivoHabilidades.text.Split('\n').Skip(1).ToArray();
            dictHabilidades = Lector.Lector.LeerHabilidades(lineas);
            lineas = archivoPersonajes.text.Split('\n').Skip(1).ToArray();
            dictPersonajes = Lector.Lector.LeerPersonajes(lineas, dictHabilidades);
            lineas = archivoEnemigos.text.Split('\n').Skip(1).ToArray();
            dictEnemigos = Lector.Lector.LeerEnemigos(lineas, dictHabilidades); 

            //Luego se inicializan los personajes activos en los slots
            InitPersonajes();
            InitEnemigos();
        }

        private void InitPersonajes() //Esta funcion inicializa slotsPersonajes y los PersonajeUI en slotsPersonajesUI, asignandoles personaje.
        {
            for (int i = 0; i < nombresPersonajes.Count; i++)
            {
                if (dictPersonajes.TryGetValue(nombresPersonajes[i], out Personaje p))
                {
                    p.Posicion = i; //El personaje tiene que saber donde est� parado
                    slotsPersonajes.Add(p);
                    slotsPersonajesUI[i].SeleccionarPersonaje(p);
                }
                else
                {
                    slotsPersonajes.Add(null);
                    if (nombresPersonajes[i] == "") Debug.Log("Slot personaje vac�o");
                    else Debug.LogWarning("Personaje " + nombresPersonajes[i] + " no se encontr�");
                }
            }
        }
        private void InitEnemigos()
        {
            for (int i = 0; i < nombresEnemigos.Count; i++)
            {
                if (dictEnemigos.TryGetValue(nombresEnemigos[i], out Enemigo e))
                {
                    e.Posicion = i; //El enemigo tiene que saber donde est� parado
                    slotsEnemigos.Add(e);
                }
                else
                {
                    slotsEnemigos.Add(null);
                    if (nombresEnemigos[i] == "") Debug.Log("Slot enemigo vac�o");
                    else Debug.LogWarning("Enemigo " + nombresEnemigos[i] + " no se encontr�");
                }
            }
        }
    }
}
