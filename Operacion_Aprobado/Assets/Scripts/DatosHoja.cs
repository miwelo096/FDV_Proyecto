using UnityEngine;
using UnityEngine.Serialization;

public class DatosHoja : MonoBehaviour, IInteractuable
{
    [SerializeField, FormerlySerializedAs("canvasInteraccion")]
    private GameObject _canvasInteraccion;
    public string nombreEscenaMinijuego;
    
    
    /// <summary>
    /// Al interactuar con el objeto, entra en el puzzle que se encuentra en la escena nombreEscenaMinijuego.
    /// </summary>
    public void Interactuar()
    {
        GameManager.EntrarPuzzle(nombreEscenaMinijuego);
    }
    
    /// <summary>
    /// get: Obtiene el canvas que aparece para interactuar.
    /// </summary>
    public GameObject canvasInteraccion
    {
        get { return _canvasInteraccion; }
    }
}
