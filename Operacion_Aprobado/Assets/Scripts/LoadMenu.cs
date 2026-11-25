using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LoadMenu : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject loadPanel; // El panel que contiene la lista de partidas

    [Header("Lista de Carga")]
    [SerializeField] private Transform contenedorLista;
    [SerializeField] private GameObject prefabBotonGuardado;

    public static bool IsLoadMenuOpen { get; private set; }

    private void Awake()
    {
        IsLoadMenuOpen = false;
        
        GameAsserts.IsValidReference(loadPanel, $"loadPanel ({name})");
        GameAsserts.IsValidReference(contenedorLista, $"contenedorLista ({name})");
        GameAsserts.IsValidReference(prefabBotonGuardado, $"prefabBotonGuardado ({name})");

        if (loadPanel != null) loadPanel.SetActive(false);
    }


    public void AlternarLoad()
    {
        if (!IsLoadMenuOpen)
        {
            AbrirLoadMenu();
        }
        else
        {
            CerrarLoadMenu();
        }
    }
    
    // Abre el panel de carga y rellena la lista con los guardados disponibles
    void AbrirLoadMenu()
    {
        if (loadPanel == null) return;

        IsLoadMenuOpen = true;
        loadPanel.SetActive(true);

        RellenarLista();
    }

    // Cierra el panel de carga
    void CerrarLoadMenu()
    {
        if (loadPanel == null) return;

        IsLoadMenuOpen = false;
        loadPanel.SetActive(false);
    }

    // Rellena la lista con los botones de las partidas guardadas
    private void RellenarLista()
    {
        // Limpiamos botones anteriores
        foreach (Transform child in contenedorLista)
            Destroy(child.gameObject);

        if (SaveManager.Instance == null) return;

        List<ClaseSave> saves = SaveManager.Instance.allSaves;

        // Creamos un botón por cada guardado
        foreach (ClaseSave save in saves)
        {
            GameObject btnObj = Instantiate(prefabBotonGuardado, contenedorLista);

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
        Debug.Log($"Cargando partida desde el menú: {save.saveName}");
        
        CerrarLoadMenu();

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.LoadSave(save);
        }
    }
}
