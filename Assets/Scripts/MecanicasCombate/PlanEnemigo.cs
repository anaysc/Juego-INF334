using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combate
{
    public class PlanEnemigo
    {
        public List<(int, Habilidad)> movimientos;

        public PlanEnemigo(List<(int, Habilidad)> movimientos)
        {
            this.movimientos = movimientos;
        }
    }
}
