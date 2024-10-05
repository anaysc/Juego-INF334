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
        [SerializeField]
        private TextAsset archivoHabilidades;
        [SerializeField]
        private TextAsset archivoPersonajes;

        private Dictionary<string, Habilidad> dictHabilidades;
        private Dictionary<string, Personaje> dictPersonajes;

        private List<Personaje> slotsPersonajes = new List<Personaje>(); //Los 4 slots de personajes
        private List<Enemigo> slotsEnemigos = new List<Enemigo>(); //Los 4 slots de enemigos

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
        }
    }
}
