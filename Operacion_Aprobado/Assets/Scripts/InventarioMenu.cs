using UnityEngine;
using UnityEngine.InputSystem;

public class InventarioMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject inventoryPanel;
    public static bool isInventoryOpen { get; private set; }





    void Awake()
    {
        isInventoryOpen = false;
        
        GameAsserts.IsValidReference(inventoryPanel, $"inventaryPanel ({name})");

        inventoryPanel.SetActive(false);
    }

    void OnEnable()
    {
        InputManager.OnTabPressed += AlternarInventario;
        InputManager.OnClosePressed += CerrarIventario;
    }

    void OnDisable()
    {
        InputManager.OnTabPressed -= AlternarInventario;
        InputManager.OnClosePressed -= CerrarIventario;
    }



    /// <summary>
    /// Si el inventario está abierto, lo cierra
    /// y si está cerrado, lo abre.
    /// </summary>
    void AlternarInventario()
    {
        if (!isInventoryOpen)
        {
            AbrirInventario();
        }
        else
        {
            CerrarIventario();
        }
    }

    /// <summary>
    /// Abre el inventario activando sus paneles y deteniendo el tiempo.
    /// </summary>
    void AbrirInventario()
    {
        isInventoryOpen = true;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    /// <summary>
    /// Cierra el inventario desactivando sus paneles y reactivando el tiempo.
    /// </summary>
    void CerrarIventario()
    {  
        isInventoryOpen = false;
        
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }
}
