using Combate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combate
{
    public class Enemigo : Creatura
    {
        public Enemigo(string nombre, float maxHp, float maxMana, float baseDamage) : base(nombre, maxHp, maxMana, baseDamage)
        {
        }

        public override bool EsPersonaje() { return false; }
    }
}
