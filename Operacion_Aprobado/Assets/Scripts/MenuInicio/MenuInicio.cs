using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MenuInicio : MonoBehaviour
{
    [Header("Paneles de UI")]
    [SerializeField] private GameObject panelPrincipal; // El panel con Start, Load, Exit
    [SerializeField] private GameObject panelLoad;      // El panel que muestra la lista de guardados

    [Header("Botones Principales")]
    [SerializeField] private Button botonLoadGame; // Para desactivarlo si no hay partidas guardadas

    [Header("Lista de Carga (Load)")]
    [SerializeField] private Transform contenedorListaLoad;
    [SerializeField] private GameObject prefabBotonGuardado;

    private void Start()
    {
        GameAsserts.IsValidReference(panelPrincipal, $"panelPrincipal ({name})");
        GameAsserts.IsValidReference(panelLoad, $"panelLoad ({name})");
        GameAsserts.IsValidReference(botonLoadGame, $"botonLoadGame ({name})");
        GameAsserts.IsValidReference(contenedorListaLoad, $"contenedorListaLoad ({name})");
        GameAsserts.IsValidReference(prefabBotonGuardado, $"prefabBotonGuardado ({name})");

        panelPrincipal.SetActive(true);
        panelLoad.SetActive(false);

        // Si no hay guardados, desactivamos el botón de Load Game
        ActualizarEstadoBotonLoad();
    }

    private void ActualizarEstadoBotonLoad()
    {
        if (botonLoadGame != null && SaveManager.Instance != null)
        {
            // El botón se activa solo si hay 1 o más guardados
            botonLoadGame.interactable = SaveManager.Instance.allSaves.Count > 0;
        }
    }

    // BOTONES PRINCIPALES
    public void OnClickStartGame()
    {
        Debug.Log("Iniciando nueva partida...");
        
        // Nos aseguramos de que no haya un guardado pendiente de cargar
        if (SaveManager.Instance != null) 
            SaveManager.Instance.CancelarCargaPendiente(); 

        // Cargamos la escena del juego
        SceneManager.LoadScene(Constantes.ESCENAPRINCIPAL);
    }

    public void OnClickLoadGame()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.allSaves.Count == 0)
        {
            Debug.LogWarning("No hay partidas guardadas para cargar.");
            return;
        }

        // Ocultamos el menú principal y mostramos la lista de guardados
        panelPrincipal.SetActive(false);
        panelLoad.SetActive(true);

        RellenarListaDeCarga();
    }

    public void OnClickExitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // PANEL DE CARGA (LOAD)

    private void RellenarListaDeCarga()
    {
        // Limpiamos botones anteriores
        foreach (Transform child in contenedorListaLoad)
            Destroy(child.gameObject);

        List<ClaseSave> saves = SaveManager.Instance.allSaves;

        // Creamos un botón por cada guardado
        foreach (ClaseSave save in saves)
        {
            GameObject btnObj = Instantiate(prefabBotonGuardado, contenedorListaLoad);

            TMP_Text txt = btnObj.GetComponentInChildren<TMP_Text>();
            if (txt != null)
            {
                string fechaCorta = save.date;
                if (fechaCorta.Length > 16) fechaCorta = fechaCorta.Substring(0, 16);
                txt.text = $"{save.saveName}\n{fechaCorta}";
            }

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                ClaseSave captured = save;
                btn.onClick.AddListener(() => CargarPartidaSeleccionada(captured));
            }
        }
    }

    private void CargarPartidaSeleccionada(ClaseSave save)
    {
        Debug.Log($"Cargando partida: {save.saveName}");
        
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.LoadSave(save);
        }
    }

    // Botón para volver atrás desde el panel de carga al menú principal
    public void OnClickVolverAtras()
    {
        panelLoad.SetActive(false);
        panelPrincipal.SetActive(true);
        ActualizarEstadoBotonLoad();
    }
}
