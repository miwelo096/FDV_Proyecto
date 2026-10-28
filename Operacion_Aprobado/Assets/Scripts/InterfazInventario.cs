using UnityEngine;
using System.Collections.Generic;

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
        foreach (Objeto objeto in inventario.listaInventario)
        {
            var interfazObjeto = Instantiate(prefabInterfazObjeto, padre, false).GetComponent<InterfazObjeto>();
            interfazObjeto.Initialize(objeto, inventario.SoltarObjeto);
            interfazInventario.Add(interfazObjeto.gameObject);
        }
    }
}
