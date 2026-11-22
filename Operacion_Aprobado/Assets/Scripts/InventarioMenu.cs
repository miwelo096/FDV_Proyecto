using UnityEngine;

public class InventarioMenu : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    public static bool IsInventoryOpen { get; private set; }





    void Awake()
    {
        IsInventoryOpen = false;
        
        GameAsserts.IsValidReference(inventoryPanel, $"inventaryPanel ({name})");

        inventoryPanel.SetActive(false);
    }

    void OnEnable()
    {
        InputManager.OnClosePressed += CerrarIventario;
    }

    void OnDisable()
    {
        InputManager.OnClosePressed -= CerrarIventario;
    }



    /// <summary>
    /// Si el inventario está abierto, lo cierra
    /// y si está cerrado, lo abre.
    /// </summary>
    public void AlternarInventario()
    {
        if (!IsInventoryOpen)
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
        IsInventoryOpen = true;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
        }

        GameManager.AplicarEstadoGlobal();
    }

    /// <summary>
    /// Cierra el inventario desactivando sus paneles y reactivando el tiempo.
    /// </summary>
    void CerrarIventario()
    {  
        IsInventoryOpen = false;
        
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        GameManager.AplicarEstadoGlobal();
    }
}
