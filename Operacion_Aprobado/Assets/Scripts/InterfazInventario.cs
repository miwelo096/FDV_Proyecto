using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
public class InterfazInventario : MonoBehaviour
{

    public GameObject prefabInterfazObjeto;
    public Inventario inventario;
    public Transform padre;

    public List<GameObject> interfazInventario = new();

    
    /// <summary>
    /// Refresca toda la interfaz visual del inventario sincronizándola con la lista
    /// </summary>
    public void ActualizarInterfaz()
    {
        // Destruimos todos los objetos de la interfaz de inventario
        foreach (var obj in interfazInventario)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        interfazInventario.Clear();

        // Volvemos a crearlos ordenadamente desde el índice 0 en adelante
        for (int i = 0; i < inventario.inventarioCount(); i++)
        {
            var objeto = inventario.ObtenerObjeto(i);
            var interfazObjeto = Instantiate(prefabInterfazObjeto, padre, false).GetComponent<InterfazObjeto>();
            interfazObjeto.Initialize(i, objeto, inventario.RemoveObjeto);
            interfazInventario.Add(interfazObjeto.gameObject);
        }
    }
}
