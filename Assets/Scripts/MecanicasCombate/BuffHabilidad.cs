using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combate
{
    public class BuffHabilidad : TargetedHabilidad
    {
        private bool debuff = false; //si es true afecta a los enemigos
        private string nombreEstado;
        private float valor;
        private int? duracion;
        protected override void AplicarEfecto(Creatura creatura, Creatura objetivo, int gradoDeExito)
        {
            if (objetivo != null)
            {
                new Estado(objetivo, nombreEstado, valor, duracion);
                //Aqui (o en AddEstado) podría faltar un checkeo para no duplicar estados
            }
        }

        protected override List<Creatura> ElegirObjetivos(Master master, Creatura creatura)
        {
            List<Creatura> oponentes = new List<Creatura>();
            if (creatura.EsPersonaje() == debuff) //si son verdad las dos o ninguna a la vez
            {
                oponentes.AddRange(master.Enemigos);
            }
            else
            {
                oponentes.AddRange(master.Personajes);
            }
            return oponentes;
        }

        protected override void SetParametros(string[] parametros)
        {
            foreach (string parametro in parametros)
            {
                string nombreParam = parametro.Split(':')[0];
                if(nombreParam == "debuff")
                {
                    debuff = true;
                }
                else if(nombreParam == "nombreEstado")
                {
                    nombreEstado = parametro.Split(':')[1];
                }
                else if(nombreParam == "valor")
                {
                    valor = float.Parse(parametro.Split(":")[1]);
                }
                else if(nombreParam == "duracion")
                {
                    duracion = int.Parse(parametro.Split(":")[1]);
                }
            }
        }
    }
}
