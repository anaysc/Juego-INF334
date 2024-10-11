using Combate;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoUI : MonoBehaviour
{
    public Enemigo enemigo;
    internal void SeleccionarEnemigo(Enemigo e)
    {
        enemigo = e;
    }
}
