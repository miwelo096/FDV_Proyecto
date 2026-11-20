using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PuntoGuardado : MonoBehaviour, IInteractuable
{
    [Header("Interacción Visual")]
    [SerializeField] 
    private GameObject _canvasInteraccion;

    [Header("UI de Guardado")]
    public GameObject PanelGuardado;    // Panel principal del menú de guardado
    public Transform ListaDeGuardados;    // Contenedor donde se instancian los botones de guardados existentes
    public GameObject ButtonPrefab; // Prefab de botón para cada guardado
    public Button newSaveButton;    // Botón "Nuevo guardado"

    private bool isMenuOpen = false;

     private void Start()
    {
        if (PanelGuardado != null)
        {
            PanelGuardado.SetActive(false);
        }
    }

    public GameObject canvasInteraccion
    {
        get { return _canvasInteraccion; }
    }

    public void Interactuar()
    {
        if (!isMenuOpen)
        {
            OpenSaveMenu();
        }
    }

    private void OpenSaveMenu()
    {
        if (PanelGuardado == null)
        {
            Debug.LogWarning("Asigna el panel de UI en el inspector.");
            return;
        }

        PanelGuardado.SetActive(true);
        Time.timeScale = 0f; // Pausa el juego
        isMenuOpen = true;
        if (_canvasInteraccion != null) _canvasInteraccion.SetActive(false);
        RefreshSaveList();
    }

    private void CloseSaveMenu()
    {
        isMenuOpen = false;
        Time.timeScale = 1f; // Reanuda el juego
        if (PanelGuardado != null) PanelGuardado.SetActive(false);
        if (_canvasInteraccion != null) _canvasInteraccion.SetActive(true);
    }

    // Rellena la lista de guardados existentes para poder sobrescribir
    private void RefreshSaveList()
    {
        // Limpiar botones anteriores
        foreach (Transform child in ListaDeGuardados)
            Destroy(child.gameObject);

        List<ClaseSave> saves = SaveManager.Instance.allSaves;

        if (newSaveButton != null)
            newSaveButton.interactable = saves.Count < SaveManager.Instance.maxSaves;

        foreach (ClaseSave save in saves)
        {
            GameObject btnObj = Instantiate(ButtonPrefab, ListaDeGuardados);

            TMP_Text txt = btnObj.GetComponentInChildren<TMP_Text>();
            if (txt != null)
                txt.text = $"{save.saveName}\n{save.date}";

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                ClaseSave captured = save; // captura local para el closure
                btn.onClick.AddListener(() => OnClickOverwrite(captured));
            }
        }
    }

    // Métodos de los botones

    public void OnClickNewSave()
    {
        // Genera un nombre automático o pídelo por InputField
        string newName = "Guardado " + (SaveManager.Instance.allSaves.Count + 1);
        ClaseSave created = SaveManager.Instance.NuevoSave(newName);

        if (created != null)
        {
            Debug.Log($"Partida guardada: {created.saveName}");
            CloseSaveMenu();
        }
        else
        {
            Debug.LogWarning("No se pudo guardar: límite de 10 alcanzado.");
        }
    }

    public void OnClickOverwrite(ClaseSave oldSave)
    {
        SaveManager.Instance.SobreescribirSave(oldSave);
        Debug.Log($"Partida sobrescrita: {oldSave.saveName}");
        CloseSaveMenu();
    }
    
    public void OnClickCloseMenu()
    {
        CloseSaveMenu();
    }
    public void OnClickDeleteAllSaves()
    {
        if (SaveManager.Instance == null) return;
        SaveManager.Instance.DeleteAllSaves();
        RefreshSaveList();
        Debug.Log("Se han borrado todos los guardados.");
    }
}
