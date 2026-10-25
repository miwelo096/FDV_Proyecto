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

    const int OBJETOSMAXIMOS = 10;


    public InterfazInventario interfaz;


    public GameObject prefabObjetoCaido;


    public List<Objeto> inventarioList = new();

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

    /// <summary>
    /// Añade objeto al inventario
    /// </summary>
    /// 
    /// <param name="objeto">Objeto a añadir</param>
    void AddObjeto(Objeto objeto)
    {
        GameAsserts.IsValidReference(objeto, "objeto (Inventario.addObjeto())");
        if (inventarioCount() <= OBJETOSMAXIMOS)
        {
            inventarioList.Add(objeto);
            int indice = inventarioCount() - 1;
            interfaz.ActualizarInterfaz();
            GameAsserts.IsGreaterThanZero(inventarioCount(), "inventarioCount (Inventario)");
        }
    }

    /// <summary>
    /// Elimina el objeto en la posición indice del inventario. Lo deja en el suelo si lanzado = false
    /// </summary>
    /// 
    /// <param name="indice">Indice del objeto a eliminar</param>
    /// <param name="lanzado">True elimina el objeto para siempre. False deja el objeto en el suelo</param>
    public void RemoveObjeto(int indice, bool lanzado)
    {
        if (indice >= 0 && indice < inventarioCount())
        {
            Debug.Log("Inventario: " + indice);
            var objeto = inventarioList[indice];
            if (!lanzado)
            {
                var objetoCaido = Instantiate(prefabObjetoCaido, transform.position, Quaternion.identity).GetComponent<ObjetoCaido>();
                objetoCaido.Initialize(objeto);
            }

            inventarioList.RemoveAt(indice);
            interfaz.ActualizarInterfaz();
        }
    }

    /// <summary>
    /// Obtiene el objeto dado su indice
    /// </summary>
    /// 
    /// <param name="indice">Indice del objeto. 0 <= indice < inventarioCount</param>
    /// <returns>Objeto a obtener</returns>
    public Objeto ObtenerObjeto(int indice)
    {
        if (indice >= 0 && indice < inventarioCount())
        {
            return inventarioList[indice];
        }
        return null;
    }

    /// <summary>
    /// Obtiene la cantidad de objetos en el inventario
    /// </summary>
    /// 
    /// <returns>Cantidad de objetos en el inventario</returns>
    public int inventarioCount()
    {
        return inventarioList.Count;
    }

    /// <summary>
    /// Obtiene si el inventario está vacio
    /// </summary>
    /// 
    /// <returns>False si está vacio</returns>
    public bool isNotEmpty()
    {
        return inventarioCount() > 0;
    }
}
