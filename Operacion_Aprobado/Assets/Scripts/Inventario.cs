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
            int indice = inventarioCount() - 1;
            interfaz.ActualizarInterfaz();
            GameAsserts.IsGreaterThanZero(inventarioCount(), "inventarioCount (Inventario)");
            return true;
        }
        return false;
    }

    /// <summary>
    /// Elimina el objeto en la posición indice del inventario.
    /// </summary>
    /// 
    /// <param name="indice">Indice del objeto a eliminar</param>
    public void SoltarObjeto(int indice)
    {
        if (PauseMenu.IsPaused)
            return;
            
        if (indice >= 0 && indice < inventarioCount())
        {
            Debug.Log("Inventario: " + indice);
            
            var objeto = inventarioList[indice];
                
            var objetoCaido = Instantiate(prefabObjetoCaido, transform.position, Quaternion.identity).GetComponent<ObjetoCaido>();
            objetoCaido.Initialize(objeto);

            EliminarObjeto(indice);
        }
    }

    /// <summary>
    /// Se consume el objeto y se elimina para siempre
    /// </summary>
    /// 
    /// <param name="indice">Indice del objeto</param>
    public void ConsumirObjeto(int indice)
    {
        if (PauseMenu.IsPaused) return;
        
        EliminarObjeto(indice);
    }

    /// <summary>
    /// Elimina el objeto dado su indice.
    /// </summary>
    /// 
    /// <param name="indice">Indice del objeto. 0 <= indice < inventarioCount</param>
    public void EliminarObjeto(int indice)
    {
        if (indice >= 0 && indice < inventarioCount())
        {
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
