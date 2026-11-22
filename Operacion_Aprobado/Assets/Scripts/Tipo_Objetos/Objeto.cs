using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Objeto", menuName = "Scriptable Objects/Objeto")]
public class Objeto : ScriptableObject
{
    public string id;
    [TextArea] public string descripcion;
    public GameObject prefabSuelo;
    public Sprite spriteInventario;

    [Header("Opciones del objeto")]
    public List<string> opcionesDisponibles = new List<string> {"Soltar"};


    public virtual void EjecutarAccionObjeto(string opcion, Inventario inventario)
    {
        switch (opcion)
        {
            case "Soltar":
                inventario.SoltarObjeto(this);
                break;
            default:
                Debug.LogWarning(
                    $"Accion no definida para: '{opcion}' ({name})");
                break;
        }
    }
}