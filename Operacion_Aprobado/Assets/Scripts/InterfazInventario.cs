using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
public class InterfazInventario : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    GameObject prefabInterfazObjeto;
    [Header("Atributos")]
    [SerializeField]
    Inventario inventario;
    [SerializeField]
    Transform padre;
    [SerializeField]
    SerializedDictionary<string, GameObject> interfazInventatio = new();
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
