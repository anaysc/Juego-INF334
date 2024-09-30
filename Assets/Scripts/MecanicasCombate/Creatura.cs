using System.Collections;
using System.Collections.Generic;

namespace Combate
{
    //Una clase general de la que heredan los personajes (heroes) y los enemigos
    public class Creatura
    {
        //Referencia al master
        protected Master master;

        //Stats
        private float maxHp; //La máxima vida de la creatura
        private float hp;    //La vida actual de la creatura
        private float maxMana;
        private float mana;
        private float baseDamage;

        //Propiedades para encapsular las stats. No se que tan necesario es esto en realidad, pero se supone que es buen práctica.
        public float MaxHp { get => maxHp; set => maxHp = value; }
        public float Hp { get => hp; 
            set {
                hp = value;
                if (hp < 0)
                {
                    Morir();
                }
            } 
        }
        public float MaxMana { get => maxMana; set => maxMana = value; }
        public float Mana { get => mana; set => mana = value; }
        public float BaseDamage { get => baseDamage; set => baseDamage = value; }

        //Habilidades
        protected List<Habilidad> habilidades = new List<Habilidad>();

        private void Morir()
        {
            //Aqui agregaremos funcionalidad en un futuro
        }
    }
}
