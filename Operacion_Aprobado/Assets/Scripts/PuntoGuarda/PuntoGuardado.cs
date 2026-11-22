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
    [SerializeField] private GameObject PanelGuardado;    // Panel principal del menú de guardado
    [SerializeField] private Transform ListaDeGuardados;    // Contenedor donde se instancian los botones de guardados existentes
    [SerializeField] private GameObject ButtonPrefab; // Prefab de botón para cada guardado
    [SerializeField] private Button newSaveButton;    // Botón "Nuevo guardado"

    public static bool IsPuntoGuardadoOpen = false;

    private void Start()
    {
        IsPuntoGuardadoOpen = false;

        GameAsserts.IsValidReference(PanelGuardado, $"PanelGuardado ({name})");
        GameAsserts.IsValidReference(ListaDeGuardados, $"ListaDeGuardados ({name})");
        GameAsserts.IsValidReference(ButtonPrefab, $"ButtonPrefab ({name})");
        GameAsserts.IsValidReference(newSaveButton, $"newSaveButton ({name})");

        PanelGuardado.SetActive(false);
    }

    public GameObject canvasInteraccion
    {
        get { return _canvasInteraccion; }
    }

    public void Interactuar()
    {
        if (!IsPuntoGuardadoOpen)
        {
            OpenSaveMenu();
        }
    }


    void OnEnable()
    {
        InputManager.OnClosePressed += CloseSaveMenu;
    }

    void OnDisable()
    {
        InputManager.OnClosePressed -= CloseSaveMenu;
    }

    private void OpenSaveMenu()
    {
        if (PanelGuardado == null)
        {
            Debug.LogWarning("Asigna el panel de UI en el inspector.");
            return;
        }

        IsPuntoGuardadoOpen = true;
        PanelGuardado.SetActive(true);

        if (_canvasInteraccion != null) _canvasInteraccion.SetActive(false);
        GameManager.AplicarEstadoGlobal();
        RefreshSaveList();
    }

    private void CloseSaveMenu()
    {
        if (PanelGuardado != null) PanelGuardado.SetActive(false);
        IsPuntoGuardadoOpen = false;
        if (_canvasInteraccion != null) _canvasInteraccion.SetActive(true);
        GameManager.AplicarEstadoGlobal();
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
