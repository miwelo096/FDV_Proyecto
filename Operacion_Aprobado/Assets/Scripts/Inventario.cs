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
        if (!isActiveAndEnabled ||
            PauseMenu.IsPaused ||
            GameManager.InPuzzle)
        {
            return;
        }

        if (!collision.CompareTag(Constantes.ITEMTAG))
        {
            return;
        }

        ObjetoCaido objetoCaido = collision.GetComponent<ObjetoCaido>();

        if (objetoCaido == null ||
            objetoCaido.Recogido ||
            !collision.enabled)
        {
            return;
        }

        if (AddObjeto(objetoCaido.Objeto))
        {
            objetoCaido.Recogido = true;
            Destroy(collision.gameObject);
        }
    }

    /// <summary>
    /// Añade objeto al inventario
    /// </summary>
    /// 
    /// <param name="objeto">Objeto a añadir</param>
    /// <returns>True si se ha guardado en el inventario</returns>
    private bool AddObjeto(Objeto objeto)
    {
        if (objeto == null)
        {
            Debug.LogError(
                "Inventario: el recogible no tiene un Objeto asignado.",
                this);
            return false;
        }

        if (interfaz == null)
        {
            Debug.LogError(
                "Inventario: falta asignar la interfaz.",
                this);
            return false;
        }

        if (!interfaz.ValidarReferencias(this))
        {
            return false;
        }

        if (inventarioList.Count >= Constantes.OBJETOSMAXIMOS)
        {
            return false;
        }

        inventarioList.Add(objeto);
        interfaz.ActualizarInterfaz();

        return true;
    }


    /// <summary>
    /// Suelta el objeto del inventario.
    /// </summary>
    /// 
    /// <param name="objeto">Objeto a soltar del inventario</param>
    public bool SoltarObjeto(Objeto objeto)
    {
        if (!isActiveAndEnabled ||
            PauseMenu.IsPaused ||
            GameManager.InPuzzle)
        {
            return false;
        }

        if (objeto == null || !inventarioList.Contains(objeto))
        {
            Debug.LogWarning(
                "Inventario: no se puede soltar un objeto nulo o ausente.",
                this);
            return false;
        }

        if (interfaz == null)
        {
            Debug.LogError(
                "Inventario: falta asignar la interfaz.",
                this);
            return false;
        }

        if (!interfaz.ValidarReferencias(this))
        {
            return false;
        }

        if (prefabObjetoCaido == null ||
            prefabObjetoCaido.GetComponent<ObjetoCaido>() == null ||
            prefabObjetoCaido.GetComponent<Collider2D>() == null)
        {
            Debug.LogError(
                "Inventario: asigna un Prefab Objeto Caido con ObjetoCaido y Collider2D.",
                this);
            return false;
        }

        GameObject objetoInstanciado = Instantiate(
            prefabObjetoCaido,
            transform.position,
            Quaternion.identity,
            transform.parent);

        ObjetoCaido objetoCaido =
            objetoInstanciado.GetComponent<ObjetoCaido>();

        if (objetoCaido == null || !objetoCaido.Initialize(objeto))
        {
            Destroy(objetoInstanciado);
            return false;
        }

        if (!EliminarObjeto(objeto))
        {
            Destroy(objetoInstanciado);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Se consume el objeto y se elimina para siempre
    /// </summary>
    /// 
    /// <param name="objeto">Objeto a consumir</param>
    public bool ConsumirObjeto(Objeto objeto)
    {
        if (!isActiveAndEnabled ||
            PauseMenu.IsPaused ||
            GameManager.InPuzzle)
        {
            return false;
        }

        if (objeto == null || !inventarioList.Contains(objeto))
        {
            Debug.LogWarning(
                "Inventario: no se puede consumir un objeto nulo o ausente.",
                this);
            return false;
        }

        if (interfaz == null)
        {
            Debug.LogError(
                "Inventario: falta asignar la interfaz.",
                this);
            return false;
        }

        if (!interfaz.ValidarReferencias(this))
        {
            return false;
        }

        return EliminarObjeto(objeto);
    }

    /// <summary>
    /// Elimina el objeto del inventario y actualiza la interfaz de inventario.
    /// </summary>
    /// 
    /// <param name="objeto">Objeto a eliminar</param>
    private bool EliminarObjeto(Objeto objeto)
    {
        if (!inventarioList.Remove(objeto))
        {
            return false;
        }

        interfaz.ActualizarInterfaz();
        return true;
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
