using UnityEngine;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class InterfazObjeto : MonoBehaviour
{   


    public Sprite sprite;

    public Button boton;

    /// <summary>
    /// Inicializa el objeto
    /// </summary>
    /// 
    /// <param name="objeto">Objeto a inicializar</param>
    /// <param name="action">Acción a realizar al hacer click en el objeto en la interfaz del inventario</param>
    public void Initialize(Objeto objeto, Action<Objeto> action)
    {
        sprite = objeto.icono;
        GetComponent<Image>().sprite = objeto.icono;
        transform.localScale = Vector2.one;
        boton.onClick.AddListener(()=>action.Invoke(objeto));
    }

    void OnDestroy()
    {
        boton.onClick.RemoveAllListeners();
    }
}
