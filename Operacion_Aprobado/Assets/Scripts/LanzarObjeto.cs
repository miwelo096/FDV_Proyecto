using Unity.VisualScripting;
using UnityEngine;

public class LanzarObjeto : MonoBehaviour
{
    // Constante de fuerza de lanzamiento
    const float FUERZALANZAMIENTO = 9f;


    [SerializeField]
    // Posición desde la que se lanza un objeto
    private Transform puntoDeLanzamiento;
    // Inventario del jugador
    private Inventario inventarioScript;

    private ObjetoLanzable objeto;
    private bool paraLanzar;



    void Start()
    {
        if (puntoDeLanzamiento == null)
        {
            Debug.LogError("Punto de lanzamiento no asignado a Player (LanzarObjeto)");
        }

        inventarioScript = GetComponent<Inventario>();
        if (inventarioScript == null)
        {
            Debug.LogError("Script del inventario no asignado a Player");
        }

        paraLanzar = false;
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
        if (paraLanzar) return;

        // No se comprueba inventarioScript != null ya que hubiese saltado en Start()
        if (inventarioScript.isNotEmpty())
        {
            objeto = obtenerPrimerLanzable();

            if (objeto != null)
            {
                if (objeto.prefabObjetoLanzable != null)
                {
                    paraLanzar = true;
                }
                else
                {
                    Debug.LogError("Prefab de objeto lanzable no asignado (LanzarObjeto.PreparaLanzamiento)");
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
    /// Ejecuta el lanzamiento del objeto asignado y lo elimina del inventario.
    /// </summary>
    void EjecutarLanzamiento()
    {
        GameObject objetoInstanciado = Instantiate(objeto.prefabObjetoLanzable, puntoDeLanzamiento.position, Quaternion.identity);
                
        var rbObjeto = objetoInstanciado.GetComponent<Rigidbody2D>();

        // No se comprueba puntoDeLanzamiento != null ya que hubiese saltado en Start()
        if (rbObjeto != null)
        {
            // Obtener la posición del ratón
            Vector2 posicionRaton = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 direccion = (posicionRaton - (Vector2)puntoDeLanzamiento.position).normalized;
            rbObjeto.linearVelocity = direccion * FUERZALANZAMIENTO;
            
            inventarioScript.ConsumirObjeto(objeto);

            Debug.Log("Lanzamiento completado");
        }
        else
        {
            Debug.LogError("No hay Rigidbody2D en el objeto instanciado (LanzarObjeto.lanzar())");
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
        objeto = null;
        paraLanzar = false;
    }
}
