using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combate
{
    //Esta clase representa los efectos de estado que pueden obtener las creaturas, como bonus de daño, resistencia a tipos de daño, o el estado "muerto", entre otros.
    public class Estado
    {
        public Creatura creatura; //la creatura a la que afecta
        public string nombre; //nombre del estado, generalmente en formato "tipo-especifico" (si aplica), por ejemplo: "resistencia-viento"
        public float valor;
        public int? duracion; //En ciclos, aka, 8 compases. Tiene el signo ? porque podria tomar el valor null, representando que dura para siempre

        public virtual void Actualizar() //Debería llamarse al empezar un nuevo ciclo
        {
            CheckIfEnded();
        }
        protected void CheckIfEnded()
        {
            if (duracion != null)
            {
                duracion--;
                if (duracion <= 0)
                {
                    creatura.RemoveEstado(this);
                    creatura = null;
                }
            }
        }

        public Estado(Creatura creatura, string nombre, float valor = 1, int? duracion = null)
        {
            this.creatura = creatura;
            this.nombre = nombre;
            this.valor = valor;
            this.duracion = duracion;

            creatura.AddEstado(this);
        }
    }
}
