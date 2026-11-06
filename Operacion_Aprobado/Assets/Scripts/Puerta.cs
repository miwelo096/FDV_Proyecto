using UnityEngine;

public class Puerta : MonoBehaviour, IInteractuable
{
    [SerializeField]
    private GameObject _canvasInteraccion;
    [Header("Configuración de la puerta")]
    public string idPuertaRequerida;
    public bool estaAbierta = false;


    void Awake()
    {
        if (_canvasInteraccion != false) _canvasInteraccion.SetActive(false);
        if (string.IsNullOrEmpty(idPuertaRequerida))
        {
            Debug.LogError($"No hay idPuertaRequerida ({name})");
        }
    }

    public void Interactuar()
    {
        if (estaAbierta) return;
        Inventario inventarioJugador = FindAnyObjectByType<Inventario>();

        if (inventarioJugador != null)
        {
            bool tieneLlave = false;
            foreach(Objeto item in inventarioJugador.listaInventario)
            {
                if (item is ObjetoClave clave && clave.id == idPuertaRequerida)
                {
                    tieneLlave = true;
                    break;
                }
            }

            if (tieneLlave)
            {
                AbrirPuerta();
            }
            else
            {
                Debug.Log("La puerta está cerrada");
            }
        }
        else
        {
            Debug.LogError($"No se ha encontrado Inventario ({name})");
        }
    }


    void AbrirPuerta()
    {
        estaAbierta = true;
        if (_canvasInteraccion != null) _canvasInteraccion.SetActive(false);

        Debug.Log("Puerta abierta");
        gameObject.SetActive(false);
    }

    /// <summary>
    /// get: Obtiene el canvas que aparece para interactuar.
    /// </summary>
    public GameObject canvasInteraccion
    {
        get { return _canvasInteraccion; }
    }
}
