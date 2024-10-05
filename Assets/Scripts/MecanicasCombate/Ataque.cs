using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combate
{
    public class Ataque : Habilidad
    {
        private float[] damageMulti = { 0, 0.5f, 0.75f, 1 }; //Factor del daño base que hace el ataque segun el grado de exito

        protected override void Activar(Master master, Creatura creatura, int gradoDeExito)
        {
            float damage = damageMulti[gradoDeExito] * creatura.BaseDamage;
            Creatura objetivo = ElegirObjetivo(master, creatura);
            if(objetivo != null )//Esto no deberia ser necesario igual porque ya se deberia estar checkeando a traves de SePuedeActivar
            {
                objetivo.Hp -= damage;
            }

        }
        public override bool SePuedeActivar(Master master, Creatura creatura, int gradoDeExito)
        {
            if (base.SePuedeActivar(master, creatura, gradoDeExito))//Recordemos que la base checkea el mana
            {
                //Hay que checkear que no este muerto, o que no se pueda atacar por alguna otra razon
                return ElegirObjetivo(master, creatura) == null;
            }
            return false;
        }
        protected override void SetParametros(string[] parametros)
        {
            foreach(string parametro in parametros)
            {
                if (parametro.Split(':',2)[0] == "damageMulti")
                {
                    int i = 0;
                    foreach(string valor in parametro.Split(":", 2)[1].Split('|'))
                    {
                        damageMulti[i] = float.Parse(valor);
                        i++;
                    }
                }
            }
        }

        protected virtual Creatura ElegirObjetivo(Master master, Creatura creatura)
        {
            //Esta implementación por defecto elige al oponente en la misma pocisión, y si no puede pasa al siguiente y así.
            List<Creatura> oponentes = new List<Creatura>();
            if (creatura.EsPersonaje())
            {
                oponentes.AddRange(master.Enemigos);
            }
            else
            {
                oponentes.AddRange(master.Personajes);
            }
            for (int i = creatura.Posicion; i < creatura.Posicion+4; i++)
            {
                int j = i;
                if (j >= 4) j -= 4;
                if (oponentes[j] != null)
                {
                    //Hay que checkear que no este muerto, o que no se pueda atacar por alguna otra razon
                    return oponentes[j];
                }
            }
            return null; //Esto solo ocurre si no hay oponentes
        }
    }
}
