using System.Collections.Generic;

namespace Combate.Habilidades
{
    
    public class Curacion : TargetedHabilidad
    {
        private float[] healMulti = { 0, 0.5f, 0.75f, 1 }; //Factor del da�o base que hace el ataque segun el grado de exito
        private float heal;

        protected override void SetParametros(string[] parametros)
        {
            base.SetParametros(parametros);
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
            heal = healMulti[gradoDeExito];
            if (objetivo != null)//Esto no deberia ser necesario igual porque ya se deberia estar checkeando a traves de SePuedeActivar
            {
                objetivo.Hp += heal;
            }
        }
        public float Heal
        {
            get { return heal; }
        }
    }
}
