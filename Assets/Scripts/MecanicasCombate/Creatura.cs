using System.Collections;
using System.Collections.Generic;

namespace Combate
{
    //Una clase general de la que heredan los personajes (heroes) y los enemigos
    public abstract class Creatura
    {
        //Referencia al master
        protected Master master;

        //Stats
        private string nombre;
        private float maxHp; //La m�xima vida de la creatura
        private float hp;    //La vida actual de la creatura
        private float maxMana;
        private float mana;
        private float baseDamage;

        //Effectos de Estado
        private List<Estado> efectosDeEstado = new List<Estado>(); //Los efectos de estado funcionan con un nombre y opcionalmente un valor asociado

        private int posicion; //Posicion en el juego (0-3) son los slots de los personajes o enemigos segun corresponda. Debe concordar con el indice en las listas del master

        //Propiedades para encapsular las stats. No se que tan necesario es esto en realidad, pero se supone que es buen pr�ctica.

        public string Nombre { get => nombre; }
        public float MaxHp { get => maxHp; set => maxHp = value; }
        public float Hp { get => hp; 
            set {
                if (!HasEstado("muerto"))
                {
                    hp = value;
                    if (hp < 0)
                    {
                        Morir();
                    }
                }
            } 
        }
        public float MaxMana { get => maxMana; set => maxMana = value; }
        public float Mana { get => mana; set => mana = value; }
        public float BaseDamage { get => baseDamage; set => baseDamage = value; }
        public int Posicion { get => posicion; set => posicion = value; }

        public List<Estado> EfectosDeEstado { get => efectosDeEstado; }

        //Habilidades
        protected List<Habilidad> habilidades = new List<Habilidad>();
        public List<Habilidad> Habilidades { get => habilidades; }

        private void Morir()
        {
            new Estado(this, "muerto"); //Esto automaticamente a�ade el nuevo estado a la lista
        }

        public abstract bool EsPersonaje();

        protected Creatura(string nombre, float maxHp, float maxMana, float baseDamage)
        {
            this.nombre = nombre;
            this.maxHp = maxHp;
            hp = maxHp;
            this.maxMana = maxMana;
            mana = maxMana;
            this.baseDamage = baseDamage;
        }

        public void AddEstado(Estado estado)
        {
            efectosDeEstado.Add(estado);
        }
        public void RemoveEstado(Estado estado)
        {
            efectosDeEstado.Remove(estado);
        }
        public bool HasEstado(string nombre)
        {
            foreach(Estado estado in efectosDeEstado)
            {
                if(estado.nombre == nombre)
                {
                    return true;
                }
            }
            return false;
        }
        public List<Estado> FindAllEstados(string nombre) //Entrega todos los estados con el nombre en cuesti�n
        {
            List<Estado> output = new List<Estado>();
            foreach(Estado estado in efectosDeEstado)
            {
                if( estado.nombre == nombre)
                {
                    output.Add(estado);
                }
            }
            return output;
        }
        public Estado FindEstado(string nombre)
        {
            foreach (Estado estado in efectosDeEstado)
            {
                if (estado.nombre == nombre)
                {
                    return estado;
                }
            }
            return null;
        }

        public virtual void OnCiclo()
        {
            List<Estado> efectoscopia = new List<Estado>(efectosDeEstado);
            foreach(Estado estado in efectoscopia)
            {
                estado.Actualizar();
            }
        }
        public void Reiniciar()
        {
            Hp = MaxHp;           // Restablecer la vida al máximo
            Mana = MaxMana;       // Restablecer el maná al máximo
            efectosDeEstado.Clear(); // Eliminar todos los efectos de estado
            // Otros reinicios según sea necesario
        }

    }

}
