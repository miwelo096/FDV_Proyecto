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
        ResetearEstado();
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
        if (!isActiveAndEnabled ||
            PauseMenu.IsPaused ||
            GameManager.InPuzzle)
        {
            return;
        }

        // Una nueva preparación sustituye la selección anterior.
        ResetearEstado();

        if (inventarioScript == null ||
            !inventarioScript.isActiveAndEnabled)
        {
            Debug.LogError(
                "LanzarObjeto: se necesita un Inventario activo en el jugador.",
                this);
            return;
        }

        if (puntoDeLanzamiento == null)
        {
            Debug.LogError(
                "LanzarObjeto: falta asignar el punto de lanzamiento.",
                this);
            return;
        }

        if (camara == null || !camara.isActiveAndEnabled)
        {
            Debug.LogError(
                "LanzarObjeto: se necesita una cámara activa para apuntar.",
                this);
            return;
        }

        InterfazInventario interfaz = inventarioScript.Interfaz;

        if (interfaz == null)
        {
            Debug.LogError(
                "LanzarObjeto: falta la interfaz del inventario.",
                this);
            return;
        }

        if (!interfaz.ValidarReferencias(inventarioScript))
        {
            return;
        }

        if (!inventarioScript.IsNotEmpty)
        {
            Debug.Log("Inventario vacío.", this);
            return;
        }

        ObjetoLanzable candidato = obtenerPrimerLanzable();

        if (candidato == null)
        {
            Debug.Log(
                "No se ha encontrado un objeto lanzable en el inventario.",
                this);
            return;
        }

        if (candidato.prefabObjetoLanzable == null)
        {
            Debug.LogError(
                "LanzarObjeto: el objeto no tiene un prefab de lanzamiento asignado.",
                this);
            return;
        }

        objeto = candidato;
        paraLanzar = true;
        interfaz.SeleccionarObjeto(objeto);
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
    private void EjecutarLanzamiento()
    {
        if (!isActiveAndEnabled ||
            PauseMenu.IsPaused ||
            GameManager.InPuzzle)
        {
            return;
        }

        if (!paraLanzar || !VerificarDisponibilidadObjeto())
        {
            Debug.Log(
                "LanzarObjeto: el objeto preparado ya no está disponible.",
                this);
            CancelarLanzamiento();
            return;
        }

        if (puntoDeLanzamiento == null ||
            camara == null ||
            !camara.isActiveAndEnabled ||
            inventarioScript == null ||
            !inventarioScript.isActiveAndEnabled)
        {
            Debug.LogError(
                "LanzarObjeto: faltan referencias activas para confirmar.",
                this);
            CancelarLanzamiento();
            return;
        }

        GameObject prefab = objeto.prefabObjetoLanzable;

        if (prefab == null || !prefab.activeSelf)
        {
            Debug.LogError(
                "LanzarObjeto: el prefab del proyectil debe estar asignado y activo.",
                this);
            CancelarLanzamiento();
            return;
        }

        Rigidbody2D cuerpoPrefab = prefab.GetComponent<Rigidbody2D>();
        Collider2D colliderPrefab = prefab.GetComponent<Collider2D>();
        ObjetoSonido sonidoPrefab = prefab.GetComponent<ObjetoSonido>();

        if (cuerpoPrefab == null ||
            !cuerpoPrefab.simulated ||
            cuerpoPrefab.bodyType != RigidbodyType2D.Dynamic ||
            colliderPrefab == null ||
            !colliderPrefab.enabled ||
            colliderPrefab.isTrigger ||
            sonidoPrefab == null ||
            !sonidoPrefab.enabled)
        {
            Debug.LogError(
                "LanzarObjeto: el prefab necesita Rigidbody2D dinámico y simulado, " +
                "Collider2D habilitado sin Is Trigger y ObjetoSonido habilitado.",
                this);
            CancelarLanzamiento();
            return;
        }

        Vector2 posicionRaton =
            camara.ScreenToWorldPoint(Input.mousePosition);

        Vector2 direccion =
            (posicionRaton - (Vector2)puntoDeLanzamiento.position).normalized;

        if (direccion == Vector2.zero)
        {
            Debug.Log(
                "LanzarObjeto: dirección nula. Se cancela sin consumir el objeto.",
                this);
            CancelarLanzamiento();
            return;
        }

        GameAsserts.IsGreaterThanZero(
            Constantes.FUERZALANZAMIENTO,
            $"Constantes.FUERZALANZAMIENTO ({name})");

        GameObject objetoInstanciado = Instantiate(
            prefab,
            puntoDeLanzamiento.position,
            Quaternion.identity,
            transform.parent);

        Rigidbody2D rbObjeto =
            objetoInstanciado.GetComponent<Rigidbody2D>();

        if (rbObjeto == null)
        {
            Debug.LogError(
                "LanzarObjeto: la instancia del proyectil no tiene Rigidbody2D.",
                this);
            objetoInstanciado.SetActive(false);
            Destroy(objetoInstanciado);
            CancelarLanzamiento();
            return;
        }

        rbObjeto.linearVelocity =
            direccion * Constantes.FUERZALANZAMIENTO;

        if (!inventarioScript.ConsumirObjeto(objeto))
        {
            objetoInstanciado.SetActive(false);
            Destroy(objetoInstanciado);
            CancelarLanzamiento();
            return;
        }

        Debug.Log("Lanzamiento completado");
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
