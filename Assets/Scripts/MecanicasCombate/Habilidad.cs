using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Combate
{
    public abstract class Habilidad
    {
        private string nombre;
        private string patron; //una secuencia de 16 caracteres que representa un compás. "nxxx" significa negra, "cx" corchea, "s" semicorchea, "bxxxxxxx" blanca.
        protected float costoMana = 0;

        public string nombreDisplay;

        public string Patron { get => patron; }
        public string Nombre { get => nombre; }

        public void Inicializar(string nombre, string patron, float costoMana, string[] parametros = null)
        {
            this.nombre = nombre;
            this.patron = patron;
            this.costoMana = costoMana;
            this.nombreDisplay = nombre; //Por defecto en caso que no haya otro
            SetParametros(parametros);
        }
        protected virtual void SetParametros(string[] parametros)
        {
            foreach (string parametro in parametros)
            {
                string nombreParam = parametro.Split(':', 2)[0];
                string valorParam = parametro.Split(":", 2)[1];
                if (nombreParam == "nombreDisplay")
                {
                    nombreDisplay = valorParam;
                }
            }
        }

        protected abstract void Activar(Master master, Creatura creatura, int gradoDeExito); //La creatura es el personaje o enemigo realizando la habilidad
                                                                                             //El grado de exito va de 0 a 3 (?): Fallo, Ok, Bien, Perfecto 
        public virtual bool SePuedeActivar(Master master, Creatura creatura, int gradoDeExito)
        {
            if(creatura.Mana >= costoMana)
            {
                return true;
            }
            else
            {
                Debug.Log("La Habilidad no se puede activar porque no hay mana");
                return false;
            }
        }

        public bool TryActivar(Master master, Creatura creatura, int gradoDeExito) //La razon por la que existe este metodo es por si el jugador intenta realizar una habilidad pero por alguna razon no se puede
        {
            if(SePuedeActivar(master, creatura, gradoDeExito))
            {
                creatura.Mana -= costoMana;
                Activar(master,creatura, gradoDeExito);
                return true;
            }
            else
            {
                return false; 
            }
        }
    }
}
