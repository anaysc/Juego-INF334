using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combate
{
    public class Personaje : Creatura
    {
        public override bool EsPersonaje() { return true; }

        public (Habilidad, int) DetectarPatron(string patron, float[] desviaciones) //Entrega la habilidad del personaje que más concuerde con el patrón recibido, y el grado de éxito
        {
            //patron es un string de 16 (o menos pasra detección parcial) caracteres 
            //Las desviaciones es que tanto se alejan los input de haber estado perfectamente en la semicorchea (va entre -1 y 1)
            int mejorIndice = 0;
            float mejorError = Mathf.Infinity;
            for(int i = 0; i<habilidades.Count; i++)
            {
                float error = CalcularError(habilidades[i].Patron, patron, desviaciones);
                if (error < mejorError)
                {
                    mejorIndice = i;
                    mejorError = error;
                }
            }
            return (habilidades[mejorIndice], ErrorAGradoDeExito(mejorError));
        }

        private float CalcularError(string patronObjetivo, string patronEjecutado, float[] desviaciones)
        {
            //Borrador, la versión final será un poco más compleja
            float error = 0;
            for (int j = 0; j < patronEjecutado.Length; j++)
            {
                if (patronObjetivo[j] == patronEjecutado[j])
                {
                    error += Mathf.Abs(desviaciones[j]);
                }
                else
                {
                    error += 1;
                }
            }
            return error;
        }
        private int ErrorAGradoDeExito(float error)
        {
            //Estos numeros claramente no serán finales, están por mientras
            if(error < 2)
            {
                return 3;
            }
            else
            {
                return 0;
            }
        }
    }
}
