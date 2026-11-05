using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

[RequireComponent(typeof(Collider2D))]
public class Inventario : MonoBehaviour
{
    [SerializeField]
    private InterfazInventario interfaz;

    [SerializeField]
    private GameObject prefabObjetoCaido;


    private List<Objeto> inventarioList = new();

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (PauseMenu.IsPaused) return;
        if (!collision.CompareTag("Item")) return;

        var ObjetoCaido = collision.GetComponent<ObjetoCaido>();
        if (ObjetoCaido == null || ObjetoCaido.Recogido || !collision.enabled) return;

        
        if (AddObjeto(ObjetoCaido.Objeto))
        {
            ObjetoCaido.Recogido = true;
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
        GameAsserts.IsValidReference(objeto, $"objeto ({name}.AddObjeto())");
        if (objeto != null && inventarioList.Count < Constantes.OBJETOSMAXIMOS)
        {
            inventarioList.Add(objeto);
            interfaz.ActualizarInterfaz();
            GameAsserts.ValueBetweenIncluded(inventarioList.Count, Constantes.OBJETOSMAXIMOS, 0, $"inventarioCount ({name})");
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
        if (objeto == null || !inventarioList.Contains(objeto))
        {
            Debug.LogWarning($"Intento de soltar objeto null o que no se encuentra en el inventario ({name}.SoltarObjeto)");
            return;
        }

        var objetoInstanciado = Instantiate(prefabObjetoCaido, transform.position, Quaternion.identity);
        var objetoCaido = objetoInstanciado.GetComponent<ObjetoCaido>();
        
        if (objetoCaido != null)
        {
            objetoCaido.Initialize(objeto);
            EliminarObjeto(objeto);
        }
        else
        {
            Debug.LogError($"Varibale objetoCaido ({name}.SoltarObjeto()) es null");
            Destroy(objetoInstanciado);
        }
    }

    /// <summary>
    /// Se consume el objeto y se elimina para siempre
    /// </summary>
    /// 
    /// <param name="objeto">Objeto a consumir</param>
    public void ConsumirObjeto(Objeto objeto)
    {
        if (PauseMenu.IsPaused) return;
        if (objeto == null || !inventarioList.Contains(objeto))
        {
            Debug.LogWarning($"Intento de consumir objeto null o que no se encuentra en el inventario ({name}.ConsumirObjeto())");
            return;
        }
        
        EliminarObjeto(objeto);
    }

    /// <summary>
    /// Elimina el objeto del inventario y actualiza la interfaz de inventario.
    /// </summary>
    /// 
    /// <param name="objeto">Objeto a eliminar</param>
    void EliminarObjeto(Objeto objeto)
    {
        inventarioList.Remove(objeto);
        if (interfaz != null) interfaz.ActualizarInterfaz();
    }

    /// <summary>
    /// get: Obtiene si el inventario está vacio
    /// </summary>
    public bool IsNotEmpty
    {
        get { return inventarioList.Count > 0; }
    }

    /// <summary>
    /// get: Obtiene la lista del inventario en modo solo lectura.
    /// </summary>
    public IReadOnlyList<Objeto> listaInventario
    {
        get { return inventarioList.AsReadOnly(); }
    }

    /// <summary>
    /// get: Obtiene el script de InterfazInventario.
    /// </summary>
    public InterfazInventario Interfaz
    {
        get { return interfaz; }
    }
}
