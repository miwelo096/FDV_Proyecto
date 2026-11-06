using System.Collections.Generic;
using UnityEngine;

public class DetectorInteracciones : MonoBehaviour
{
    // Guardamos la referencia de la hoja/mesa con la que podemos interactuar en este momento
    private IInteractuable objetoActualEnRango;
    private Player playerScript;
    private List<IInteractuable> objetosInteractivosEnRango;


    void Start()
    {
        objetoActualEnRango = null;
        playerScript = GetComponent<Player>();
        objetosInteractivosEnRango = new List<IInteractuable>();

        GameAsserts.IsValidReference(playerScript, $"playerScript ({name})");
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
        IInteractuable objeto = collision.GetComponent<IInteractuable>();

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
        IInteractuable objeto = collision.GetComponent<IInteractuable>();

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
        objetosInteractivosEnRango.RemoveAll(
                h => h == null ||
                (h is MonoBehaviour mh && mh == null)
                );

        if (objetosInteractivosEnRango.Count == 0)
        {
            objetoActualEnRango = null;
            return;
        }

        IInteractuable masCercana = null;
        float menorDistanciaSq = Mathf.Infinity;
        Vector2 posicionJugador = transform.position;

        foreach (var interactuable in objetosInteractivosEnRango)
        {
            MonoBehaviour mh = interactuable as MonoBehaviour;
            if (mh != null)
            {
                float distanciaSq = ((Vector2)mh.transform.position - posicionJugador).sqrMagnitude;
                if (distanciaSq < menorDistanciaSq)
                {
                    menorDistanciaSq = distanciaSq;
                    masCercana = interactuable;
                }
            }
        }

        objetoActualEnRango = masCercana;

        foreach (var interactuable in objetosInteractivosEnRango)
        {
            if (interactuable.canvasInteraccion != null)
            {
                interactuable.canvasInteraccion.SetActive(interactuable == objetoActualEnRango);
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
    void OnDisable()
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
            objetoActualEnRango.Interactuar();
        }
    }
}