using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ClaseSave
{
    // Datos de guardado
    public string saveId;
    public string saveName;
    public string sceneName;
    public string date;

    // Datos del jugador que se guardan
    public Vector3 playerPosicion;
    public int playerVidas;
    public int playerEstado;

    // Narrativa
    // public string narrativa;
    // public List<string> narrativaFlags = new List<string>();

    // Inventario
    public List<string> inventarioItems = new List<string>();

    // Estado del enemigo y objetos recogidos
    public List<string> defeatedEnemigos = new List<string>();
    public List<string> objetosRecogidos = new List<string>();

    // Posición del punto de guardado
    public Vector3 savePointPosition;
}
