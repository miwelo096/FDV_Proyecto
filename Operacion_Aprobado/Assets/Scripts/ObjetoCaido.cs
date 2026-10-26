using System.Collections;
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

    /// <summary>
    /// Inicializa el objeto
    /// </summary>
    /// 
    /// <param name="objeto">Objeto a inicializar</param>
    public void Initialize(Objeto objeto)
    {
        this.objeto = objeto;
        var objetoCaido = Instantiate(objeto.prefabSuelo, transform);
        objetoCaido.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
        StartCoroutine(PermitirRecogida(delay));
    }

    /// <summary>
    /// Permite la recogida del objeto tras un periodo de tiempo
    /// </summary>
    /// 
    /// <param name="delay">Periodo de tiempo para volver a recoger</param>
    /// <returns></returns>
    IEnumerator PermitirRecogida(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<Collider2D>().enabled = true;
    }
}
