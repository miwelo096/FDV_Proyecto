using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class InterfazInventario : MonoBehaviour
{

    public GameObject prefabInterfazObjeto;
    public Inventario inventario;
    public Transform padre;

    public List<GameObject> interfazInventario = new();

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
            casilla.GetComponent<UnityEngine.UI.Image>() == null)
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
        if (inventario == null ||
            padre == null ||
            prefabInterfazObjeto == null)
        {
            Debug.LogError(
                "InterfazInventario: asigna Inventario, Padre y Prefab Interfaz Objeto.",
                this);
            return;
        }

        ActualizarInterfaz();
    }

    /// <summary>
    /// Refresca toda la interfaz visual del inventario sincronizándola con la lista
    /// </summary>
    public void ActualizarInterfaz()
    {
        // Destruimos todos los objetos de la interfaz de inventario
        foreach (var obj in interfazInventario)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        interfazInventario.Clear();

        // Volvemos a crearlos ordenadamente desde el índice 0 en adelante
        foreach (Objeto objeto in inventario.listaInventario)
        {
            var interfazObjeto = Instantiate(prefabInterfazObjeto, padre, false).GetComponent<InterfazObjeto>();
            interfazObjeto.Initialize(objeto, inventario.SoltarObjeto);
            interfazInventario.Add(interfazObjeto.gameObject);
        }

        var lanzarScript = inventario.GetComponent<LanzarObjeto>();
        if (lanzarScript != null && lanzarScript.ObjetoPreparado != null)
        {
            SeleccionarObjeto(lanzarScript.ObjetoPreparado);
        }
    }

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
}
