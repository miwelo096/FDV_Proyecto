using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class Inventario : MonoBehaviour
{

    const int OBJETOSMAXIMOS = 10;


    public InterfazInventario interfaz;


    public GameObject prefabObjetoCaido;


    public List<Objeto> inventarioList = new();

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (PauseMenu.IsPaused)
            return;

        if (!collision.CompareTag("Item"))
        {
            return;
        }

        var ObjetoCaido = collision.GetComponent<ObjetoCaido>();
        if (ObjetoCaido == null || ObjetoCaido.recogido || !collision.enabled)
        {
            return;
        }

        
        if (AddObjeto(ObjetoCaido.objeto))
        {
            ObjetoCaido.recogido = true;
            Destroy(collision.gameObject);
        }
    }

    /// <summary>
    /// Añade objeto al inventario
    /// </summary>
    /// 
    /// <param name="objeto">Objeto a añadir</param>
    /// <returns>True si se ha guardado en el inventario</returns>
    bool AddObjeto(Objeto objeto)
    {
        GameAsserts.IsValidReference(objeto, "objeto (Inventario.addObjeto())");
        if (inventarioCount() < OBJETOSMAXIMOS)
        {
            inventarioList.Add(objeto);
            interfaz.ActualizarInterfaz();
            GameAsserts.ValueBetweenIncluded(inventarioCount(), 10, 0, "inventarioCount (Inventario)");
            return true;
        }
        return false;
    }

    /// <summary>
    /// Suelta el objeto del inventario.
    /// </summary>
    /// 
    /// <param name="objeto">Objeto a soltar del inventario</param>
    public void SoltarObjeto(Objeto objeto)
    {
        if (PauseMenu.IsPaused) return;

        var objetoCaido = Instantiate(prefabObjetoCaido, transform.position, Quaternion.identity).GetComponent<ObjetoCaido>();
        objetoCaido.Initialize(objeto);

        EliminarObjeto(objeto);
    }

    /// <summary>
    /// Se consume el objeto y se elimina para siempre
    /// </summary>
    /// 
    /// <param name="objeto">Objeto a consumir</param>
    public void ConsumirObjeto(Objeto objeto)
    {
        if (PauseMenu.IsPaused) return;
        
        EliminarObjeto(objeto);
    }

    /// <summary>
    /// Elimina el objeto del inventario y actualiza la interfaz de inventario.
    /// </summary>
    /// 
    /// <param name="objeto">Objeto a eliminar</param>
    public void EliminarObjeto(Objeto objeto)
    {
        inventarioList.Remove(objeto);
        interfaz.ActualizarInterfaz();
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

    /// <summary>
    /// get: Devuelve la lista del inventario
    /// </summary>
    public List<Objeto> listaInventario
    {
        get { return inventarioList; }
    }
}
