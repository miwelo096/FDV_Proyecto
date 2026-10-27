using System.Collections;
using UnityEngine;

public class ObjetoCaido : MonoBehaviour
{   

    public bool autoStart;

    const float DELAYRECOGIDA = 1.5f;

    public Objeto objeto;
    public bool recogido = false;

    private Collider2D colliderObjeto;


    void Awake()
    {
        colliderObjeto = GetComponent<Collider2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (autoStart&&objeto!=null)
        {
            Initialize(objeto);
        }
    }

    /// <summary>
    /// Inicializa el objeto y desactiva el collider hasta pasado un periodo de tiempo
    /// </summary>
    /// 
    /// <param name="objeto">Objeto a inicializar</param>
    public void Initialize(Objeto objeto)
    {
        this.objeto = objeto;
        if (this.objeto != null && this.objeto.prefabSuelo)
        {
            var objetoCaido = Instantiate(objeto.prefabSuelo, transform);
            objetoCaido.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
        }
        
        if (colliderObjeto != null)
        {
            colliderObjeto.enabled = false;
        }

        StartCoroutine(PermitirRecogida(DELAYRECOGIDA));
    }

    /// <summary>
    /// Permite la recogida del objeto activando el collider tras un periodo de tiempo
    /// </summary>
    /// 
    /// <param name="delay">Periodo de tiempo para volver a recoger</param>
    /// <returns></returns>
    IEnumerator PermitirRecogida(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (colliderObjeto != null)
        {
            colliderObjeto.enabled = true;
        }
    }
}
