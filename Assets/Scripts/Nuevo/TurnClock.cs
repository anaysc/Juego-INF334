using Combate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnClock : MonoBehaviour
{
    public Master master;
    public AudioMaster audioMaster;
    public Image clock;

    public Sprite relojTurnoPersonajes;
    public Sprite relojTurnoEnemigos;

    private void Update()
    {
        if (master.turnoActual == TurnType.personajes)
        {
            clock.sprite = relojTurnoPersonajes;
        }
        else if(master.turnoActual == TurnType.enemigos)
        {
            clock.sprite = relojTurnoEnemigos;
        }

        float tiempoEnCiclos = audioMaster.TimeInBeats / master.DuracionCiclo;
        float tiempoTurno = (tiempoEnCiclos - master.CicloInicioTurno) / master.CiclosPorTurno;
        clock.fillAmount = tiempoTurno;
    }
}
