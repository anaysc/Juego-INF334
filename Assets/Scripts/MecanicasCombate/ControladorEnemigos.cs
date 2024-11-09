using Combate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorEnemigos : MonoBehaviour
{
    [SerializeField] private Master master;

    public PlanEnemigo plan;
    private int index = 0; //El elemento del plan que se usará siguiente

    private Enemigo enemigo;
    private Habilidad habilidad;
    private EnemigoUI enemigoUI;

    public void EmpezarCiclo(TurnType turno) 
    {
        if (turno == TurnType.enemigos)
        {
            int posicion;
            (posicion, habilidad) = plan.movimientos[index];
            enemigo = master.Enemigos[posicion];
            enemigoUI = master.EnemigosUI[posicion];
            enemigoUI.habilidadActivada = true;
            enemigoUI.ActivarTrackHabilidad(habilidad);
        }
    }
    public void FinalizarCiclo(TurnType turno) 
    {
        if (turno == TurnType.enemigos && enemigo!=null)
        {
            habilidad.TryActivar(master, enemigo, 2);
            enemigoUI.habilidadActivada = false;
            enemigoUI.DetenerTrackHabilidad();

            index++;
            if (index >= plan.movimientos.Count)
            {
                index = 0;
            }
        }
    }

}
