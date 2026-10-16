using UnityEngine;
using System;
using UnityEngine.UI;
using JetBrains.Annotations;

[RequireComponent(typeof(Button))]
public class InterfazObjeto : MonoBehaviour
{   
    public Sprite sprite;
    public Button boton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize(string idInventario, Objeto objeto, Action<String> removeObjetoAction)
    {
        sprite = objeto.icono;
        GetComponent<Image>().sprite = objeto.icono;
        transform.localScale = Vector2.one;
        boton.onClick.AddListener(()=>removeObjetoAction.Invoke(idInventario));
    }

    void OnDestroy()
    {
        boton.onClick.RemoveAllListeners();
    }
}
