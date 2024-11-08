using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combate
{
    public class Personaje : Creatura
    {
        //Constantes para el calculo del grado de exito
        const float ERRORMAX = 1.5f;
        public const float TOLERANCIA = 0.2f;

        public bool seleccionado = false; //variable controlada por el master, que se asegura que solo un personaje pueda activar su habilidad por ciclo (los combos seran una excepcion)
        public int gradoDeExitoReciente = 0; //Podria ser util llevar la cuenta del ultimo grado de exito que se calculo

        public Personaje(string nombre, float maxHp, float maxMana, float baseDamage) : base(nombre, maxHp, maxMana, baseDamage)
        {
            //Por ahora solo hace lo mismo que el constructor de Creatura
        }

        public override bool EsPersonaje() { return true; }

        public (Habilidad, int) DetectarPatron(List<float> inputs, int largo = 16) //Entrega la habilidad del personaje que m�s concuerde con el patr�n recibido, y el grado de �xito
        {
            //inputs deberia ser los tiempos en los que el usuario apret� el bot�n. 0 es al comienzo, 16 al final de un comp�s. Pueden pasarse un poquito de los bordes si es necesario (para considerar si el usuario apreto un poco muy pronto)

            
            //largo es en que parte del ciclo de habilidad (dos compases por ahora) vamos, para considerar hasta ahi nomas el string del patron

            int mejorIndice = 0;
            float mejorError = Mathf.Infinity;
            for(int i = 0; i<habilidades.Count; i++)
            {
                string patronObjetivo = habilidades[i].Patron.Substring(0, largo);
                float error = CalcularError(patronObjetivo, inputs);
                if (error < mejorError)
                {
                    mejorIndice = i;
                    mejorError = error;
                }
            }
            int gradoDeExito = ErrorAGradoDeExito(mejorError, habilidades[mejorIndice].Patron.Substring(0, largo));
            gradoDeExitoReciente = gradoDeExito;
            return (habilidades[mejorIndice], gradoDeExito);
        }

        public float CalcularError(string patronObjetivo, List<float> inputs)
        {
            //patronObjetivo deberia ser ajustado al largo apropiado
            float error = 0;

            //Calcular error tipo 1
            for(int j = 0; j < patronObjetivo.Length; j++)
            {
                if(patronObjetivo[j] != 'x') //Si deber�a ir un pulso
                {
                    //Se encuentra el pulso mas cercano (maximo error 2.5 corcheas de diferencia)
                    float minDist = ERRORMAX; 
                    foreach(float f in inputs)
                    {
                        if (Mathf.Abs(f - j) < minDist)
                        {
                            minDist = Mathf.Abs(f - j);
                        }
                    }
                    if (minDist > TOLERANCIA)
                    {
                        error += minDist - TOLERANCIA;
                    }
                }
            }

            //Calcular Error tipo 2
            foreach(float f in inputs)
            {
                float minDist = ERRORMAX;
                for (int j = 0; j < patronObjetivo.Length; j++)
                {
                    if(patronObjetivo[j] != 'x')
                    {
                        if (Mathf.Abs(f - j) < minDist)
                        {
                            minDist = Mathf.Abs(f - j);
                        }
                    }
                }
                if(minDist > TOLERANCIA)
                {
                    error += minDist - TOLERANCIA;
                }
            }

            return error;
        }
        private int ErrorAGradoDeExito(float error, string patronObjetivo)
        {
            //Primero calculamos el error que se obtendr�a si el ususario no hace nada
            float errorBase = 0;
            for(int j = 0; j < patronObjetivo.Length; j++)
            {
                if(patronObjetivo[j] != 'x')
                {
                    errorBase += ERRORMAX-TOLERANCIA;
                }
            }

            //Estos numeros claramente no ser�n finales, est�n por mientras
            if(error < 0.1f*errorBase)
            {
                return 3;
            }
            else if(error < 0.25f*errorBase)
            {
                return 2;
            }
            else if(error < 0.5f * errorBase)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        public override void OnCiclo()
        {
            base.OnCiclo();
            seleccionado = false;
        }
    }
}
