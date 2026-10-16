using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using Unity.Hierarchy.Editor;
using JetBrains.Annotations;
[RequireComponent(typeof(Collider2D))]
public class Inventario : MonoBehaviour
{


    public InterfazInventario interfaz;


    public GameObject prefabObjetoCaido;


    public SerializedDictionary<string, Objeto> inventario = new();

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Item"))
        {
            return;
        }

        var ObjetoCaido = collision.GetComponent<ObjetoCaido>();
        if (ObjetoCaido == null || ObjetoCaido.recogido)
        {
            return;
        }

        ObjetoCaido.recogido = true;
        AddObjeto(ObjetoCaido.objeto);
        Destroy(collision.gameObject);
    }

    void AddObjeto(Objeto objeto)
    {
        var id = Guid.NewGuid().ToString();
        inventario.Add(id,objeto);
        interfaz.AddObjetoInterfaz(id,objeto);
    }

    public void RemoveObjeto(string id)
    {
        var objetoCaido = Instantiate(prefabObjetoCaido, transform.position, Quaternion.identity).GetComponent<ObjetoCaido>();
        var objeto = inventario.GetValueOrDefault(id);
        objetoCaido.Initialize(objeto);
        inventario.Remove(id);
        interfaz.RemoveObjetoInterfaz(id);
    }
}
