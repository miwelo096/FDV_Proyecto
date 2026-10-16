using System.Collections;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class ObjetoCaido : MonoBehaviour
{   

    public bool autoStart;

    public float delay = 3.0f;

    public Objeto objeto;
    public bool recogido = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (autoStart&&objeto!=null)
        {
            Initialize(objeto);
        }
    }

    public void Initialize(Objeto objeto)
    {
        this.objeto = objeto;
        var objetoCaido = Instantiate(objeto.prefab, transform);
        objetoCaido.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
        StartCoroutine(PermitirRecogida(delay));
    }

    IEnumerator PermitirRecogida(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<Collider2D>().enabled = true;
    }
}
