using UnityEngine;
using UnityEngine.EventSystems;

public class LanzarObjeto : MonoBehaviour
{
    [SerializeField]
    // Posición desde la que se lanza un objeto
    private Transform puntoDeLanzamiento;
    // Inventario del jugador
    private Inventario inventarioScript;
    private Camera camara;

    private ObjetoLanzable objeto;
    private bool paraLanzar;



    void Start()
    {
        paraLanzar = false;
        camara = Camera.main;
        inventarioScript = GetComponent<Inventario>();
        
        GameAsserts.IsValidReference(puntoDeLanzamiento, $"puntoDeLanzamiento ({name})");
        GameAsserts.IsValidReference(inventarioScript, $"inventarioScript ({name})");
        GameAsserts.IsValidReference(camara, $"camara ({name})");
    }

    /// <summary>
    /// Suscribe el metodo al evento de entrada
    /// </summary>
    void OnEnable()
    {
        InputManager.OnQPressed += PrepararLanzamiento;
    }

    /// <summary>
    /// Desuscribe el metodo del evento de entrada
    /// </summary>
    void OnDisable()
    {
        InputManager.OnQPressed -= PrepararLanzamiento;
    }


    void Update()
    {
        if (PauseMenu.IsPaused) return;

        if (paraLanzar)
        {
            if (!VerificarDisponibilidadObjeto())
            {
                Debug.Log("El objeto preparado ya no está disponible en el inventario. Cancelando preparación.");
                CancelarLanzamiento();
                return;
            }

            // Ignorar clics si el puntero está sobre un elemento de la interfaz
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return; 
            }

            if (Input.GetMouseButtonDown(0))
            {
                EjecutarLanzamiento();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                CancelarLanzamiento();
            }
        }
    }

    /// <summary>
    /// Prepara el lanzamiento asignando el primer objeto lanzable del inventario si lo hay.
    /// </summary>
    void PrepararLanzamiento()
    {
        if (PauseMenu.IsPaused) return;
        if (paraLanzar) CancelarLanzamiento();

        // No se comprueba inventarioScript != null ya que hubiese saltado en Start()
        if (inventarioScript != null && inventarioScript.IsNotEmpty)
        {
            objeto = obtenerPrimerLanzable();

            if (objeto != null)
            {
                if (objeto.prefabObjetoLanzable != null)
                {
                    paraLanzar = true;
                    if (inventarioScript.Interfaz != null)
                    {
                        inventarioScript.Interfaz.SeleccionarObjeto(objeto);
                    }
                }
                else
                {
                    Debug.LogError("Prefab de objeto lanzable no asignado (LanzarObjeto.PreparaLanzamiento)");
                    ResetearEstado();
                }
            }
            else
            {
                Debug.Log("No encontrado objeto lanzable en el inventario");
            }
        }
        else
        {
            Debug.Log("Inventario vacio");
        }
    }

    /// <summary>
    /// Confirma que el objeto preparado sigue disponible para lanzarse
    /// </summary>
    /// 
    /// <returns>True si sigue disponible</returns>
    bool VerificarDisponibilidadObjeto()
    {
        if (inventarioScript == null || objeto == null) return false;

        foreach (Objeto item in inventarioScript.listaInventario)
        {
            if (item == objeto)
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Ejecuta el lanzamiento del objeto asignado y lo elimina del inventario.
    /// </summary>
    void EjecutarLanzamiento()
    {
        if (puntoDeLanzamiento == null || objeto == null || objeto.prefabObjetoLanzable == null || camara == null)
        {
            Debug.LogError("Faltan referencias para ejecutar el lanzamiento.");
            CancelarLanzamiento();
            return;
        }

        GameObject objetoInstanciado = Instantiate(objeto.prefabObjetoLanzable, puntoDeLanzamiento.position, Quaternion.identity);
                
        var rbObjeto = objetoInstanciado.GetComponent<Rigidbody2D>();

        // No se comprueba puntoDeLanzamiento != null ya que hubiese saltado en Start()
        if (rbObjeto != null)
        {
            // Obtener la posición del ratón
            Vector2 posicionRaton = camara.ScreenToWorldPoint(Input.mousePosition);

            Vector2 direccion = (posicionRaton - (Vector2)puntoDeLanzamiento.position).normalized;
            GameAsserts.IsGreaterThanZero(Constantes.FUERZALANZAMIENTO, $"Constantes.FUERZALANZAMIENTO ({name})");
            rbObjeto.linearVelocity = direccion * Constantes.FUERZALANZAMIENTO;
            
            inventarioScript.ConsumirObjeto(objeto);

            Debug.Log("Lanzamiento completado");
        }
        else
        {
            Debug.LogError("No hay Rigidbody2D en el objeto instanciado (LanzarObjeto.lanzar())");
            Destroy(objetoInstanciado);
        }

        ResetearEstado();
    }

    /// <summary>
    /// Cancela el lanzamiento del objeto y quita la asignación del objeto.
    /// </summary>
    void CancelarLanzamiento()
    {
        Debug.Log("Lanzamiento cancelado");

        ResetearEstado();
    }



    /// <summary>
    /// Obtiene el primer objeto lanzable del inventario.
    /// </summary>
    /// 
    /// <returns>Primer objeto lanzable del inventario</returns>
    ObjetoLanzable obtenerPrimerLanzable()
    {
        foreach(Objeto item in inventarioScript.listaInventario)
        {
            if (item is ObjetoLanzable lanzable)
            {
                return lanzable;
            }
        }
        return null;
    }

    /// <summary>
    /// Resetea los estados de objeto y paraLanzar.
    /// </summary>
    void ResetearEstado()
    {
        if (inventarioScript != null && inventarioScript.Interfaz != null)
        {
            inventarioScript.Interfaz.LimpiarSeleccion();
        }

        objeto = null;
        paraLanzar = false;
    }

    /// <summary>
    /// get: Obtiene el objeto que está preparado para lanzar en caso de que haya alguno.
    /// </summary>
    public ObjetoLanzable ObjetoPreparado
    {
        get{ return paraLanzar ? objeto : null; }
    }
}
