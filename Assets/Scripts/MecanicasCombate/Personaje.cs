using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combate
{
    public class Personaje : Creatura
    {
        const float ERRORMAX = 2.5f;

        public override bool EsPersonaje() { return true; }

        public (Habilidad, int) DetectarPatron(List<float> inputs, int largo = 32) //Entrega la habilidad del personaje que más concuerde con el patrón recibido, y el grado de éxito
        {
            //inputs deberia ser los tiempos en los que el usuario apretó el botón. 0 es al comienzo, 16 al final de un compás. Pueden pasarse un poquito de los bordes si es necesario (para considerar si el usuario apreto un poco muy pronto)

            
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
            return (habilidades[mejorIndice], ErrorAGradoDeExito(mejorError, habilidades[mejorIndice].Patron.Substring(0,largo)));
        }

        private float CalcularError(string patronObjetivo, List<float> inputs)
        {
            //patronObjetivo deberia ser ajustado al largo apropiado
            float error = 0;

            //Calcular error tipo 1
            for(int j = 0; j < patronObjetivo.Length; j++)
            {
                if(patronObjetivo[j] != 'x') //Si debería ir un pulso
                {
                    //Se encuentra el pulso mas cercano (maximo error 2.5 corcheas de diferencia)
                    float minDist = ERRORMAX; 
                    foreach(float f in inputs)
                    {
                        if (Mathf.Abs(f - j) < minDist)
                        {
                            minDist = f;
                        }
                    }
                    error += minDist;
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
                            minDist = f;
                        }
                    }
                }
            }

            return error;
        }
        private int ErrorAGradoDeExito(float error, string patronObjetivo)
        {
            //Primero calculamos el error que se obtendría si el ususario no hace nada
            float errorBase = 0;
            for(int j = 0; j < patronObjetivo.Length; j++)
            {
                if(patronObjetivo[j] != 'x')
                {
                    errorBase += ERRORMAX;
                }
            }

            //Estos numeros claramente no serán finales, están por mientras
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
    }
}
