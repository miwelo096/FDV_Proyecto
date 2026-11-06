using System.Collections.Generic;
using UnityEngine;

public class DetectorInteracciones : MonoBehaviour
{
    // Guardamos la referencia de la hoja/mesa con la que podemos interactuar en este momento
    private DatosHoja objetoActualEnRango;
    private Player playerScript;
    private List<DatosHoja> objetosInteractivosEnRango;


    void Start()
    {
        objetoActualEnRango = null;
        playerScript = GetComponent<Player>();
        objetosInteractivosEnRango = new List<DatosHoja>();

        if (playerScript == null)
        {
            Debug.LogError($"No se ha obtenido script de Player ({name})");
        }
    }

    void Update()
    {
        if (playerScript != null && playerScript.getEstado() == Player.Estados.Escondido)
        {
            DesactivarTodosLosCanvas();
            objetoActualEnRango = null;
            return;
        }

        ActualizarObjetoMasCercano();
    }



    /// <summary>
    /// Llama a ActivarCanvas con collision si el estado del Player no es escondido.
    /// </summary>
    /// 
    /// <param name="collision">Collider2D que ha traspasado</param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        DatosHoja objeto = collision.GetComponent<DatosHoja>();

        if (objeto != null && !objetosInteractivosEnRango.Contains(objeto))
        {
            objetosInteractivosEnRango.Add(objeto);
        }
    }

    /// <summary>
    /// Llama a DesactivarCanvas con collision.
    /// </summary>
    /// 
    /// <param name="collision">Collider2D que ha dejado de traspasar</param>
    void OnTriggerExit2D(Collider2D collision)
    {
        DatosHoja objeto = collision.GetComponent<DatosHoja>();

        if (objeto != null)
        {
            if (objetosInteractivosEnRango.Contains(objeto))
            {
                objetosInteractivosEnRango.Remove(objeto);
            }

            if (objeto.canvasInteraccion != null)
            {
                objeto.canvasInteraccion.SetActive(false);
            }
        }
    }


    void ActualizarObjetoMasCercano()
    {
        // Eliminamos todos los que sean null
        objetosInteractivosEnRango.RemoveAll(h => h == null);

        if (objetosInteractivosEnRango.Count == 0)
        {
            objetoActualEnRango = null;
            return;
        }

        DatosHoja masCercana = null;
        float menorDistanciaSq = Mathf.Infinity;
        Vector2 posicionJugador = transform.position;

        foreach (var hoja in objetosInteractivosEnRango)
        {
            float distanciaSq = ((Vector2)hoja.transform.position - posicionJugador).sqrMagnitude;
            if (distanciaSq < menorDistanciaSq)
            {
                menorDistanciaSq = distanciaSq;
                masCercana = hoja;
            }
        }

        objetoActualEnRango = masCercana;

        foreach (var hoja in objetosInteractivosEnRango)
        {
            if (hoja.canvasInteraccion != null)
            {
                hoja.canvasInteraccion.SetActive(hoja == objetoActualEnRango);
            }
        }
    }


    void DesactivarTodosLosCanvas()
    {
        foreach(var objeto in objetosInteractivosEnRango)
        {
            if (objeto != null && objeto.canvasInteraccion != null)
            {
                objeto.canvasInteraccion.SetActive(false);
            }
        }
    }


    /// <summary>
    /// Suscribe el metodo al evento de entrada
    /// </summary>
    void OnEnable()
    {
        InputManager.OnEPressed += ComprobacionHoja;
    }

    /// <summary>
    /// Desuscribe la entrada y limpia el aviso de interacción.
    /// </summary>
    private void OnDisable()
    {
        InputManager.OnEPressed -= ComprobacionHoja;

        if (objetosInteractivosEnRango != null)
        {
            DesactivarTodosLosCanvas();
        }

        objetoActualEnRango = null;
    }


    /// <summary>
    /// Comprueba que haya algo con lo que interactuar cerca
    /// </summary>
    void ComprobacionHoja()
    {
        if (playerScript != null && playerScript.getEstado() == Player.Estados.Escondido) return;

        if (objetoActualEnRango != null)
        {
            CargarMinijuego(objetoActualEnRango.nombreEscenaMinijuego);
        }
    }

    /// <summary>
    /// Carga la escena del minijuego/puzzle dado el nombre
    /// </summary>
    /// 
    /// <param name="nombreEscena">Nombre de la escena a cargar</param>
    void CargarMinijuego(string nombreEscena)
    {
        GameManager.EntrarPuzzle(nombreEscena);
    }
}