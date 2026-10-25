using UnityEngine;
using System;
using UnityEngine.UI;
using JetBrains.Annotations;

[RequireComponent(typeof(Button))]
public class InterfazObjeto : MonoBehaviour
{   


    public Sprite sprite;

    public Button boton;

    /// <summary>
    /// Inicializa el objeto
    /// </summary>
    /// 
    /// <param name="indiceInventario">Indice en el inventario del objeto</param>
    /// <param name="objeto">Objeto a inicializar</param>
    /// <param name="action">Acción a realizar al hacer click en el objeto en la interfaz del inventario</param>
    public void Initialize(int indiceInventario, Objeto objeto, Action<int, bool> action)
    {
        sprite = objeto.icono;
        GetComponent<Image>().sprite = objeto.icono;
        transform.localScale = Vector2.one;
        Debug.Log("InterfazObjeto: " + indiceInventario);
        boton.onClick.AddListener(()=>action.Invoke(indiceInventario, false));
    }

    void OnDestroy()
    {
        boton.onClick.RemoveAllListeners();
    }
}
