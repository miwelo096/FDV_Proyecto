using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class InterfazInventario : MonoBehaviour
{
    public GameObject prefabInterfazObjeto;
    public Inventario inventario;
    public Transform padre;

    [Header("Panel de Detalles e Interacción")]
    [SerializeField] private TextMeshProUGUI textoTitulo;
    [SerializeField] private TextMeshProUGUI textoDescripcion;
    [SerializeField] private Transform contenedorBotonesOpciones;
    [SerializeField] private GameObject prefabBotonOpcion;

    public List<GameObject> interfazInventario = new();
    private List<GameObject> botonesOpcionesActuales = new();

    public bool ValidarReferencias(Inventario inventarioEsperado)
    {
        if (inventario == null || inventario != inventarioEsperado)
        {
            Debug.LogError(
                "InterfazInventario: el campo Inventario no corresponde al inventario que recoge.",
                this);
            return false;
        }

        if (padre == null || prefabInterfazObjeto == null)
        {
            Debug.LogError(
                "InterfazInventario: asigna Padre y Prefab Interfaz Objeto.",
                this);
            return false;
        }

        InterfazObjeto casilla =
            prefabInterfazObjeto.GetComponent<InterfazObjeto>();

        if (casilla == null)
        {
            Debug.LogError(
                "InterfazInventario: el prefab necesita el componente InterfazObjeto.",
                this);
            return false;
        }

        if (casilla.boton == null)
        {
            Debug.LogError(
                "InterfazInventario: asigna el Button de la casilla.",
                this);
            return false;
        }

        if (casilla.imagenObjeto == null &&
            casilla.GetComponent<Image>() == null)
        {
            Debug.LogError(
                "InterfazInventario: la casilla necesita una Image.",
                this);
            return false;
        }

        return true;
    }

    private void Start()
    {
        ActualizarInterfaz();
        LimpiarPanelDetalles();
    }

    /// <summary>
    /// Refresca toda la interfaz visual del inventario sincronizándola con la lista
    /// </summary>
    public void ActualizarInterfaz()
    {
        if (!ValidarReferencias(inventario)) return;

        foreach (GameObject casillaAnterior in interfazInventario)
        {
            if (casillaAnterior != null)
            {
                casillaAnterior.SetActive(false);
                Destroy(casillaAnterior);
            }
        }

        interfazInventario.Clear();

        foreach (Objeto objeto in inventario.listaInventario)
        {
            InterfazObjeto interfazObjeto = Instantiate(
                prefabInterfazObjeto,
                padre,
                false).GetComponent<InterfazObjeto>();

            interfazObjeto.Initialize(objeto, item =>
            {
                SeleccionarObjeto(item);
                MostrarDetallesObjeto(item);
            });

            interfazInventario.Add(interfazObjeto.gameObject);
        }
    }

    /// <summary>
    /// Muestra la descripción del objeto y los botones que puede utlilizar.
    /// </summary>
    /// 
    /// <param name="objeto">Objeto</param>
    void MostrarDetallesObjeto(Objeto objeto)
    {
        if (textoTitulo != null)
        {
            textoTitulo.text =
                objeto != null ? objeto.id : string.Empty;
        }
        if (textoDescripcion != null)
        {
            textoDescripcion.text =
                objeto != null ? objeto.descripcion : string.Empty;
        }

        foreach (GameObject botonObjeto in botonesOpcionesActuales)
        {
            Destroy(botonObjeto);
        }
        botonesOpcionesActuales.Clear();

        if (objeto != null &&
            contenedorBotonesOpciones != null &&
            prefabBotonOpcion != null)
        {
            foreach (string opcion in objeto.opcionesDisponibles)
            {
                CrearBotonOpcion(opcion, () =>
                {
                    EjecutarAccionObjeto(objeto, opcion);
                });
            }
        }
    }

    /// <summary>
    /// Creación de los botones del objeto.
    /// </summary>
    /// 
    /// <param name="textoBoton">Texto que aparecera en el botón</param>
    /// <param name="action">Acción a realizar al pulsarse el botón</param>
    void CrearBotonOpcion(string textoBoton, UnityEngine.Events.UnityAction action)
    {
        GameObject nuevoBoton = Instantiate(prefabBotonOpcion, contenedorBotonesOpciones);
        var textoTMP = nuevoBoton.GetComponentInChildren<TextMeshProUGUI>();

        if (textoTMP != null)
        {
            textoTMP.text = textoBoton;
        }

        var botonComponente = nuevoBoton.GetComponent<Button>();
        
        if (botonComponente != null)
        {
            botonComponente.onClick.AddListener(action);
        }

        botonesOpcionesActuales.Add(nuevoBoton);
    }

    /// <summary>
    /// Limpia la descripción y los botones de la interfaz de inventario
    /// </summary>
    public void LimpiarPanelDetalles()
    {
        if (textoTitulo != null) textoTitulo.text = "";
        if (textoDescripcion != null) textoDescripcion.text = "";

        foreach (GameObject botonObjeto in botonesOpcionesActuales)
        {
            Destroy(botonObjeto);
        }
        botonesOpcionesActuales.Clear();
    }

    /// <summary>
    /// Añade un borde de color al objeto seleccionado.
    /// </summary>
    /// 
    /// <param name="objetoSeleccionado">Objeto a seleccionar</param>
    public void SeleccionarObjeto(Objeto objetoSeleccionado)
    {
        for(int i = 0; i<inventario.listaInventario.Count; i++)
        {
            var interfazObjeto = interfazInventario[i].GetComponent<InterfazObjeto>();
            if (interfazObjeto != null)
            {
                bool esEste = inventario.listaInventario[i] == objetoSeleccionado;
                interfazObjeto.MarcarComoSeleccionado(esEste);
            }
        }
    }

    /// <summary>
    /// Elimina la selección del objeto haciendo que desaparezca el borde de color.
    /// </summary>
    public void LimpiarSeleccion()
    {
        foreach (var objeto in interfazInventario)
        {
            if (objeto != null)
            {
                var interfazObjeto = objeto.GetComponent<InterfazObjeto>();
                if (interfazObjeto != null)
                {
                    interfazObjeto.MarcarComoSeleccionado(false);
                }
            }
        }
    }

    /// <summary>
    /// Ejecuta la acción en el objeto obtenida por opción.
    /// </summary>
    /// 
    /// <param name="objeto">Objeto al que se ejecuta la acción</param>
    /// <param name="opcion">Nombre de la acción a ejecutar</param>
    void EjecutarAccionObjeto(Objeto objeto, string opcion)
    {
        if (objeto == null) return;

        objeto.EjecutarAccionObjeto(opcion, inventario);

        ActualizarInterfaz();
        LimpiarPanelDetalles();
    }
}
