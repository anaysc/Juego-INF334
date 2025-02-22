using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combate.Habilidades
{
    public class Ataque : TargetedHabilidad
    {
        private float[] damageMulti = { 0, 0.5f, 0.75f, 1 }; //Factor del da�o base que hace el ataque segun el grado de exito
        private string damageType = "normal";
        private float damage;

        protected override void AplicarEfecto(Creatura creatura, Creatura objetivo, int gradoDeExito)
        {
            damage = damageMulti[gradoDeExito] * creatura.BaseDamage;
            if (objetivo != null)//Esto no deberia ser necesario igual porque ya se deberia estar checkeando a traves de SePuedeActivar
            {
                Estado resistencia = objetivo.FindEstado("resistencia-" + damageType);
                if(resistencia != null)
                {
                    damage /= resistencia.valor;
                }
                Estado bonusDamage = creatura.FindEstado("bonusDamage");
                if(bonusDamage != null)
                {
                    damage *= bonusDamage.valor;
                }
                objetivo.Hp -= damage;
            }
        }
        public float Damage
        {
            get{ return damage; }
        }
        protected override List<Creatura> ElegirObjetivos(Master master, Creatura creatura)
        {
            List<Creatura> oponentes = new List<Creatura>();
            if (creatura.EsPersonaje())
            {
                oponentes.AddRange(master.Enemigos);
            }
            else
            {
                oponentes.AddRange(master.Personajes);
            }

            if (targetRule == TargetRule.simple)
            {
                return ElegirPrimerObjetivo(creatura.Posicion, oponentes);
            }
            else if(targetRule == TargetRule.random)
            {
                return ElegirObjetivoAleatorio(oponentes);
            }
            else if(targetRule == TargetRule.fixedPosition)
            {
                
            }
            else if (targetRule == TargetRule.todos)
            {
                return ElegirTodosLosObjetivos(oponentes);
            }
            return null;
        }

        protected override void SetParametros(string[] parametros)
        {
            base.SetParametros(parametros);
            foreach (string parametro in parametros)
            {
                string nombreParam = parametro.Split(':', 2)[0];
                string valorParam = parametro.Split(":", 2)[1];
                if (nombreParam == "damageMulti")
                {
                    int i = 0;
                    foreach (string valor in valorParam.Split('|'))
                    {
                        damageMulti[i] = float.Parse(valor);
                        i++;
                    }
                }
                else if(nombreParam == "damageType")
                {
                    damageType = valorParam;
                }
                else if(nombreParam == "targetRule")
                {
                    if(valorParam == "simple")
                    {
                        targetRule = TargetRule.simple;
                    }
                    else if(valorParam == "random")
                    {
                        targetRule = TargetRule.random;
                    }
                    else if (valorParam == "todos")
                    {
                        targetRule = TargetRule.todos;
                    }
                }
            }
        }
    }
}
