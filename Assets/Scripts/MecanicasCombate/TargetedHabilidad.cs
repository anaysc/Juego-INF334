using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combate {
    //Una clase que engloba a todas las habilidades que actuan sobre objetivos, o sea practicamente todas las habilidades 
    public abstract class TargetedHabilidad : Habilidad
    {
        protected abstract List<Creatura> ElegirObjetivos(Master master, Creatura creatura);

        protected static List<Creatura> ElegirPrimerObjetivo(int posicion, List<Creatura> oponentes)
        {
            //Esta implementación por defecto elige al oponente en la misma pocisión, y si no puede pasa al siguiente y así.
            //nota: oponentes podra en realidad ser aliados si es una habilidad que actua sobre aliados

            List<Creatura> objetivos = new List<Creatura>();
            
            for (int i = posicion; i < posicion + oponentes.Count; i++)
            {
                int j = i;
                while (j >= oponentes.Count) j -= oponentes.Count;
                if (oponentes[j] != null)
                {
                    objetivos.Add(oponentes[j]);
                    return objetivos;
                }
            }
            return null; //Esto solo ocurre si no hay oponentes
        }
        protected abstract void AplicarEfecto(Creatura creatura, Creatura objetivo, int gradoDeExito);
        protected override void Activar(Master master, Creatura creatura, int gradoDeExito)
        {
            foreach(Creatura objetivo in  ElegirObjetivos(master, creatura))
            {
                AplicarEfecto(creatura, objetivo, gradoDeExito);
            }
        }
        public override bool SePuedeActivar(Master master, Creatura creatura, int gradoDeExito)
        {
            if (base.SePuedeActivar(master, creatura, gradoDeExito))//Recordemos que la base checkea el mana
            {
                //Hay que haya objetivos
                List<Creatura> objetivos = ElegirObjetivos(master, creatura);
                if (objetivos != null)
                {
                    return objetivos.Count > 0;
                }
            }
            return false;
        }
    }
}
