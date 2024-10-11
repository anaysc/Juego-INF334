using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Combate;
using Combate.Habilidades;
using System;

namespace Lector
{
    public static class Lector
    {
        public static Dictionary<string, Personaje> LeerPersonajes(string[] lineas, Dictionary<string,Habilidad> habilidades)
        {
            Dictionary<string, Personaje> personajes = new Dictionary<string, Personaje>();

            foreach (string linea in lineas)
            {
                if (linea != "")
                {
                    string[] parametros = linea.Split(';');
                    string nombre = parametros[0];
                    float maxHp = float.Parse(parametros[1]);
                    float maxMana = float.Parse(parametros[2]);
                    float baseDamage = float.Parse(parametros[3]);
                    string[] nombresHabilidades = parametros[4].Split(',');

                    Personaje personaje = new Personaje(nombre, maxHp, maxMana, baseDamage);
                    foreach (string h in nombresHabilidades)
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
                    personajes.Add(nombre, personaje);
                }
            }

            return personajes;
        }
        public static Dictionary<string, Enemigo> LeerEnemigos(string[] lineas, Dictionary<string, Habilidad> habilidades)
        {
            Dictionary<string, Enemigo> enemigos = new Dictionary<string, Enemigo>();

            foreach (string linea in lineas)
            {
                if (linea != "")
                {
                    string[] parametros = linea.Split(';');
                    string nombre = parametros[0];
                    float maxHp = float.Parse(parametros[1]);
                    float maxMana = float.Parse(parametros[2]);
                    float baseDamage = float.Parse(parametros[3]);
                    string[] nombresHabilidades = parametros[4].Split(',');

                    Enemigo enemigo = new Enemigo(nombre, maxHp, maxMana, baseDamage);
                    foreach (string h in nombresHabilidades)
                    {
                        if (habilidades.ContainsKey(h))
                        {
                            enemigo.Habilidades.Add(habilidades[h]);
                        }
                        else
                        {
                            Debug.Log("Habilidad " + h + " no encontrada");
                        }
                    }
                    enemigos.Add(nombre, enemigo);
                }
            }

            return enemigos;
        }
        public static Dictionary<string,Habilidad> LeerHabilidades(string[] lineas)
        {
            Dictionary<string, Habilidad> habilidades = new Dictionary<string, Habilidad>();

            foreach (string linea in lineas)
            {
                if (linea != "")
                {
                    string[] parametros = linea.Split(';');
                    string nombre = parametros[0];
                    string tipo = parametros[1];
                    string patron = parametros[2].Replace(" ", ""); //Remueve los espacios
                    float costoMana = float.Parse(parametros[3]);
                    string[] otrosParametros = parametros[4].Split(",");

                    Habilidad habilidad = CrearHabilidadPorTipo(tipo);
                    habilidad.Inicializar(nombre, patron, costoMana, otrosParametros);
                    habilidades.Add(nombre, habilidad);

                    Debug.Log("Habilidad " + nombre + " cargada");
                }
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
                Debug.Log("No se encontró el tipo de habilidad: " + tipo);
                return new Ataque();
            }
        }
    }
}
