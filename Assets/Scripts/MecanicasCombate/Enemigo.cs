using Combate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combate
{
    public class Enemigo : Creatura
    {
        public override bool EsPersonaje() { return false; }
    }
}
