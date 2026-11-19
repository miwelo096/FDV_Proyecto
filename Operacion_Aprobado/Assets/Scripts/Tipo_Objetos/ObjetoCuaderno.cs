using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cuaderno", menuName = "Scriptable Objects/Cuaderno")]
public class ObjetoCuaderno : Objeto
{
    [SerializeField] private List<string> textos = new List<string>();


    void OnEnable()
    {
        if (opcionesDisponibles == null)
        {
            opcionesDisponibles = new List<string>();
        }

        if (!opcionesDisponibles.Contains("Leer"))
        {
            opcionesDisponibles.Insert(0, "Leer");
        }

        if (!opcionesDisponibles.Contains("Soltar"))
        {
            opcionesDisponibles.Add("Soltar");
        }
    }

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
            case "Leer":
                for (int i = 0; i<textos.Count; i++)
                {
                    ObtenerTexto(i);
                    // TODO: Hacer que se escriban en pantalla y
                    // que se pueda pasar de página pulsando algun boton o una tecla o algo
                }
                break;
            default:
                base.EjecutarAccionObjeto(opcion, inventario);
                break;
        }
    }

    /// <summary>
    /// Obtiene el texto que hay en cada página.
    /// </summary>
    /// 
    /// <param name="pagina">Página de la que obtener el texto</param>
    /// <returns></returns>
    public string ObtenerTexto(int pagina)
    {
        if (pagina >= 0 && pagina < textos.Count)
        {
            return textos[pagina];
        }

        return null;
    }
}
