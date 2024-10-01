using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combate
{
    public class Master : MonoBehaviour
    {
        private List<Personaje> personajes = new List<Personaje>(); //Los 4 slots de personajes
        private List<Enemigo> enemigos = new List<Enemigo>(); //Los 4 slots de enemigos

        //Encapsulamos las listas y ahora son readonly
        public List<Personaje> Personajes { get => personajes; }
        public List<Enemigo> Enemigos { get => enemigos; }
    }
}
