using System.Collections.Generic;

namespace Combate.Habilidades
{
    /*
    public class Curacion : Habilidad
    {
        private float[] healMulti = { 0, 0.5f, 0.75f, 1 }; //Factor del da�o base que hace el ataque segun el grado de exito

        protected override void Activar(Master master, Creatura creatura, int gradoDeExito)
        {
            float heal = healMulti[gradoDeExito];
            Creatura objetivo = ElegirObjetivo(master, creatura);
            if (objetivo != null)//Esto no deberia ser necesario igual porque ya se deberia estar checkeando a traves de SePuedeActivar
            {
                objetivo.Hp += heal;
            }

        }
        public override bool SePuedeActivar(Master master, Creatura creatura, int gradoDeExito)
        {
            if (base.SePuedeActivar(master, creatura, gradoDeExito))//Recordemos que la base checkea el mana
            {
                //Hay que checkear que no este muerto, o que no se pueda atacar por alguna otra razon
                return ElegirObjetivo(master, creatura) != null;
            }
            return false;
        }
        protected override void SetParametros(string[] parametros)
        {
            foreach (string parametro in parametros)
            {
                if (parametro.Split(':', 2)[0] == "healMulti")
                {
                    int i = 0;
                    foreach (string valor in parametro.Split(":", 2)[1].Split('|'))
                    {
                        healMulti[i] = float.Parse(valor);
                        i++;
                    }
                }
            }
        }

        protected virtual Creatura ElegirObjetivo(Master master, Creatura creatura)
        {
            //Esta implementaci�n por defecto elige al oponente en la misma pocisi�n, y si no puede pasa al siguiente y as�.
            List<Creatura> aliados = new List<Creatura>();
            if (creatura.EsPersonaje())
            {
                aliados.AddRange(master.Personajes);
            }
            else
            {
                aliados.AddRange(master.Enemigos);
            }
            for (int i = creatura.Posicion+1; i < creatura.Posicion + aliados.Count+1; i++)
            {
                int j = i;
                while (j >= aliados.Count) j -= aliados.Count;
                if (aliados[j] != null)
                {
                    //Hay que checkear que no este muerto, o que no se pueda atacar por alguna otra razon
                    return aliados[j];
                }
            }
            return null; //Esto solo ocurre si no hay oponentes
        }
    }
    */
    public class Curacion : TargetedHabilidad
    {
        private float[] healMulti = { 0, 0.5f, 0.75f, 1 }; //Factor del da�o base que hace el ataque segun el grado de exito

        protected override void SetParametros(string[] parametros)
        {
            foreach (string parametro in parametros)
            {
                if (parametro.Split(':', 2)[0] == "healMulti")
                {
                    int i = 0;
                    foreach (string valor in parametro.Split(":", 2)[1].Split('|'))
                    {
                        healMulti[i] = float.Parse(valor);
                        i++;
                    }
                }
            }
        }

        protected override List<Creatura> ElegirObjetivos(Master master, Creatura creatura)
        {
            List<Creatura> aliados = new List<Creatura>();
            if (creatura.EsPersonaje())
            {
                aliados.AddRange(master.Personajes);
            }
            else
            {
                aliados.AddRange(master.Enemigos);
            }

            return ElegirTodosLosObjetivos(aliados);
        }

        protected override void AplicarEfecto(Creatura creatura, Creatura objetivo, int gradoDeExito)
        {
            float heal = healMulti[gradoDeExito];
            if (objetivo != null)//Esto no deberia ser necesario igual porque ya se deberia estar checkeando a traves de SePuedeActivar
            {
                objetivo.Hp += heal;
            }
        }
    }
}
