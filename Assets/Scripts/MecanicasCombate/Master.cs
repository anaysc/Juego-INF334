using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combate
{
    public class Master : MonoBehaviour
    {
        private Personaje[] personajes = new Personaje[4]; //Los 4 slots de personajes
        private Enemigo[] enemigos = new Enemigo[4]; //Los 4 slots de enemigos

        public Personaje GetPersonaje(int indice)
        {
            return personajes[indice];
        }
        public Enemigo GetEnemigo(int indice)
        {
            return enemigos[indice];
        }

    }
}
