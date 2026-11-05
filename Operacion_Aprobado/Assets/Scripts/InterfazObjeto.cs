using UnityEngine;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class InterfazObjeto : MonoBehaviour
{
    public Button boton;
    public Image imagenObjeto;
    public Outline bordeSeleccion;


    /// <summary>
    /// Inicializa el objeto
    /// </summary>
    /// 
    /// <param name="objeto">Objeto a inicializar</param>
    /// <param name="action">Acción a realizar al hacer click en el objeto en la interfaz del inventario</param>
    public void Initialize(Objeto objeto, Action<Objeto> action)
    {
        if (imagenObjeto == null)
        {
            imagenObjeto = GetComponent<Image>();
            GameAsserts.IsValidReference(imagenObjeto, $"imagenObjeto ({name})");
        }

        imagenObjeto.sprite = objeto.spriteInventario;
        transform.localScale = Vector2.one;
        boton.onClick.AddListener(()=>action.Invoke(objeto));

        if (bordeSeleccion != null)
        {
            bordeSeleccion.enabled = false;
        }
    }

    /// <summary>
    /// Activa o desactiva el borde del objeto.
    /// </summary>
    /// 
    /// <param name="seleccionado">True activa el borde. False lo desactiva</param>
    public void MarcarComoSeleccionado(bool seleccionado)
    {
        if (bordeSeleccion != null)
        {
            bordeSeleccion.enabled = seleccionado;
        }
    }

    void OnDestroy()
    {
        boton.onClick.RemoveAllListeners();
    }
}
