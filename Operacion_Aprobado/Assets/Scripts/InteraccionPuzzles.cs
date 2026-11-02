using UnityEngine;
using UnityEngine.SceneManagement;

public class DetectorInteracciones : MonoBehaviour
{
    // Guardamos la referencia de la hoja/mesa con la que podemos interactuar en este momento
    private DatosHoja objetoActualEnRango;
    private Player playerScript;


    void Start()
    {
        objetoActualEnRango = null;

        playerScript = GetComponent<Player>();
        if (playerScript == null)
        {
            Debug.LogError($"No se ha obtenido script de Player ({name})");
        }
    }



    /// <summary>
    /// Llama a ActivarCanvas con collision si el estado del Player no es escondido.
    /// </summary>
    /// 
    /// <param name="collision">Collider2D que ha traspasado</param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerScript.getEstado() != Player.Estados.Escondido)
        {
            ActivarCanvas(collision);
        }
    }

    /// <summary>
    /// Llama a ActivarCanvas con collision si el estado del Player no es escondido.
    /// Llama a DesactivarCanvas con collision si el estado del Player es escondido.
    /// </summary>
    /// 
    /// <param name="collision">Collider2D que está traspasando</param>
    void OnTriggerStay2D(Collider2D collision)
    {
        if (playerScript.getEstado() != Player.Estados.Escondido)
        {
            ActivarCanvas(collision);
        }
        else
        {
            DesactivarCanvas(collision);
        }
    }

    /// <summary>
    /// Llama a DesactivarCanvas con collision.
    /// </summary>
    /// 
    /// <param name="collision">Collider2D que ha dejado de traspasar</param>
    void OnTriggerExit2D(Collider2D collision)
    {
        DesactivarCanvas(collision);
    }

    /// <summary>
    /// Activa el canvas del objeto de collision.
    /// </summary>
    /// 
    /// <param name="collision">Collider2D a activar el canvas de su objeto</param>
    void ActivarCanvas(Collider2D collision)
    {
        DatosHoja hoja = collision.GetComponent<DatosHoja>();

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
    /// Desactiva el canvas del objeto de collision.
    /// </summary>
    /// 
    /// <param name="collision">Collider2D a desactivar el canvas de su objeto</param>
    void DesactivarCanvas(Collider2D collision)
    {
        DatosHoja hoja = collision.GetComponent<DatosHoja>();

        if (hoja != null)
        {
            if (hoja.canvasInteraccion != null)
            {
                hoja.canvasInteraccion.SetActive(false);
            }

            if (hoja == objetoActualEnRango)
            {
                objetoActualEnRango = null;
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
            GameManager.EntrarPuzzle(); 
            SceneManager.LoadScene(nombreEscena, LoadSceneMode.Additive);
        }
        else
        {
            Debug.LogError("La hoja seleccionada no tiene un nombre de escena válido configurado.");
        }
    }
}