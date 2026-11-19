using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Movil", menuName = "Scriptable Objects/Movil")]
public class ObjetoMovil : Objeto
{
    void OnEnable()
    {
        if (opcionesDisponibles == null)
        {
            opcionesDisponibles = new List<string>();
        }

        if (!opcionesDisponibles.Contains("Usar"))
        {
            opcionesDisponibles.Insert(0, "Usar");
        }

        if (opcionesDisponibles.Contains("Soltar"))
        {
            opcionesDisponibles.Remove("Soltar");
        }
        // if (!opcionesDisponibles.Contains("Soltar"))
        // {
        //     opcionesDisponibles.Add("Soltar");
        // }
    }

    /// <summary>
    /// Ejecuta la acción en el objeto obtenida por opción.
    /// </summary>
    /// 
    /// <param name="opcion">Nombre de la acción a ejecutar</param>
    /// <param name="inventario">Inventario del jugador</param>
    public override void EjecutarAccionObjeto(string opcion, Inventario inventario)
    {
        switch (opcion)
        {
            case "Usar":
                GameManager.EntrarPuzzle(Constantes.ESCENAEDUROAM);
                break;
            default:
                // base.EjecutarAccionObjeto(opcion, inventario);
                break;
        }
    }
}