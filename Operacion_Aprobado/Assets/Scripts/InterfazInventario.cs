using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
public class InterfazInventario : MonoBehaviour
{

    public GameObject prefabInterfazObjeto;

    public Inventario inventario;

    public Transform padre;

    public SerializedDictionary<string, GameObject> interfazInventatio = new();
    public void AddObjetoInterfaz(string id, Objeto objeto)
    {
        var interfazObjeto = Instantiate(prefabInterfazObjeto, padre, false).GetComponent<InterfazObjeto>();
        interfazObjeto.Initialize(id, objeto, inventario.RemoveObjeto);
        interfazInventatio.Add(id, interfazObjeto.gameObject);
    }

    public void RemoveObjetoInterfaz(string id)
    {
        var interfazObjeto = interfazInventatio.GetValueOrDefault(id);
        interfazInventatio.Remove(id);
        Destroy(interfazObjeto);
    }
}
