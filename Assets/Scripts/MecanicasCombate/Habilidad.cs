using System.Collections;
using System.Collections.Generic;

namespace Combate
{
    public abstract class Habilidad
    {
        protected string patron; //una secuencia de 16 caracteres que representa un compás. "nxxx" significa negra, "cx" corchea, "s" semicorchea, "bxxxxxxx" blanca.
        protected float costoMana = 0;

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
                return false;
            }
        }

        public bool TryActivar(Master master, Creatura creatura, int gradoDeExito) //La razon por la que existe este metodo es por si el jugador intenta realizar una habilidad pero por alguna razon no se puede
        {
            if(SePuedeActivar(master, creatura, gradoDeExito))
            {
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
