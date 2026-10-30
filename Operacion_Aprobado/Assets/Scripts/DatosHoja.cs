using UnityEngine;

public class DatosHoja : MonoBehaviour
{
    [Header("Configuración del Minijuego")]
    [Tooltip("Nombre de la escena del minijuego/puzzle de esta hoja")]
    public string nombreEscenaMinijuego;

    [Tooltip("Referencia al Canvas de interacción ('Pulsa E') de esta hoja")]
    public GameObject canvasInteraccion;
}