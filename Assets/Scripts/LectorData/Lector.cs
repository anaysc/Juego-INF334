using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combate;
using System;

namespace Lector
{
    public static class Lector
    {
        public static List<Personaje> LeerPersonajes(string[] lineas, Dictionary<string,Habilidad> habilidades)
        {
            List<Personaje> personajes = new List<Personaje>();

            foreach (string linea in lineas)
            {
                string[] parametros = linea.Split(';');
                string nombre = parametros[0];
                float maxHp = float.Parse(parametros[1]);
                float maxMana = float.Parse(parametros[2]);
                float baseDamage = float.Parse(parametros[3]);
                string[] nombresHabilidades = parametros[4].Split(','); 
                
                Personaje personaje = new Personaje(nombre, maxHp, maxMana, baseDamage);
                foreach(string h in nombresHabilidades)
                {
                    if (habilidades.ContainsKey(h))
                    {
                        personaje.Habilidades.Add(habilidades[h]);
                    }
                    else
                    {
                        Debug.Log("Habilidad " + h + " no encontrada");
                    }
                }
                personajes.Add(personaje);
            }

            return personajes;
        }
        public static List<Habilidad> LeerHabilidades(string[] lineas)
        {
            List<Habilidad> habilidades = new List<Habilidad>();

            foreach (string linea in lineas)
            {
                string[] parametros = linea.Split(';');
                string nombre = parametros[0];
                string tipo = parametros[1];
                string patron = parametros[2].Replace(" ", ""); //Remueve los espacios
                float costoMana = float.Parse(parametros[3]);
                string[] otrosParametros = parametros[4].Split(",");

                Habilidad habilidad = CrearHabilidadPorTipo(tipo);
                habilidad.Inicializar(nombre, patron, costoMana, otrosParametros);
            }

            return habilidades;
        }
        private static Habilidad CrearHabilidadPorTipo(string tipo)
        {
            if(tipo == "Ataque")
            {
                return new Ataque();
            }
            else
            {
                return new Ataque();
            }
        }
    }
}
