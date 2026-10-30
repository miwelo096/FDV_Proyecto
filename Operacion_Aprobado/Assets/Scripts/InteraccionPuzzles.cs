using UnityEngine;
using UnityEngine.SceneManagement;

public class DetectorInteracciones : MonoBehaviour
{
    // Guardamos la referencia de la hoja/mesa con la que podemos interactuar en este momento
    private DatosHoja objetoActualEnRango = null;


    /// <summary>
    /// Obtiene el componente DatosHoja del collider que ha traspasado y activa su canvas
    /// </summary>
    /// 
    /// <param name="other">Collider2D que ha traspasado</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        DatosHoja hoja = other.GetComponent<DatosHoja>();

        if (hoja != null)
        {
            objetoActualEnRango = hoja;

            if (hoja.canvasInteraccion != null)
            {
                hoja.canvasInteraccion.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Obtiene el componente DatosHoja del collider que ha dejado de traspasar y desactiva su canvas
    /// </summary>
    /// 
    /// <param name="other">Collider2D que ha dejado de traspasar</param>
    void OnTriggerExit2D(Collider2D other)
    {
        DatosHoja hoja = other.GetComponent<DatosHoja>();

        if (hoja != null && hoja == objetoActualEnRango)
        {
            if (hoja.canvasInteraccion != null)
            {
                hoja.canvasInteraccion.SetActive(false);
            }

            objetoActualEnRango = null;
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
    /// Desuscribe el metodo del evento de entrada
    /// </summary>
    void OnDisable()
    {
        InputManager.OnEPressed -= ComprobacionHoja;
    }


    /// <summary>
    /// Comprueba que haya algo con lo que interactuar cerca
    /// </summary>
    void ComprobacionHoja()
    {
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
        if (!string.IsNullOrEmpty(nombreEscena))
        {
            SceneManager.LoadScene(nombreEscena, LoadSceneMode.Additive);
        }
        else
        {
            Debug.LogError("La hoja seleccionada no tiene un nombre de escena válido configurado.");
        }
    }
}