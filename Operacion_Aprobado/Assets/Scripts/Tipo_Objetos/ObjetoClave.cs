using UnityEngine;

[CreateAssetMenu(fileName = "Clave", menuName = "Scriptable Objects/Clave")]
public class ObjetoClave : Objeto
{
    public GameObject prefabObjetoClave;


    /// <summary>
    /// Ejecuta la acción en el objeto obtenida por opción.
    /// </summary>
    /// 
    /// <param name="opcion">Nombre de la acción a ejecutar</param>
    /// <param name="inventario">Inventario del jugador</param>
    public override void EjecutarAccionObjeto(string opcion, Inventario inventario)
    {
        switch (opcion)
        {
            default:
                base.EjecutarAccionObjeto(opcion, inventario);
                break;
        }
    }
}