using UnityEngine;

public class ObjetoCaido : MonoBehaviour
{   
    [SerializeField]
    private bool autoStart;
    [SerializeField]
    private Objeto objeto;
    [SerializeField]
    private bool recogido;

    private Collider2D colliderObjeto;
    private float tiempoRecogidaRestante;

    void Awake()
    {
        colliderObjeto = GetComponent<Collider2D>();
        recogido = false;

        GameAsserts.IsValidReference(colliderObjeto, $"colliderObjeto ({name})");        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameAsserts.IsValidReference(objeto, $"objeto ({name})");
        if (autoStart&&objeto!=null)
        {
            Initialize(objeto);
        }
    }

    /// <summary>
    /// Inicializa el objeto y desactiva el collider hasta pasado un periodo de tiempo
    /// </summary>
    /// 
    /// <param name="nuevoObjeto">Objeto a inicializar</param>
    public void Initialize(Objeto nuevoObjeto)
    {
        GameAsserts.IsValidReference(
            nuevoObjeto, $"nuevoObjeto ({name}.Initialize())");

        objeto = nuevoObjeto;

        if (objeto != null && objeto.prefabSuelo)
        {
            var objetoCaido = Instantiate(objeto.prefabSuelo, transform);
            objetoCaido.transform.SetLocalPositionAndRotation(
                Vector2.zero, Quaternion.identity);
        }

        GameAsserts.IsGreaterThanZero(
            Constantes.DELAYRECOGIDA,
            $"Constantes.DELAYRECOGIDA ({name}.Initialize())");

        tiempoRecogidaRestante = Constantes.DELAYRECOGIDA;

        if (colliderObjeto != null)
        {
            colliderObjeto.enabled = false;
        }
    }

    private void Update()
    {
        if (PauseMenu.IsPaused ||
            GameManager.InPuzzle ||
            tiempoRecogidaRestante <= 0f)
        {
            return;
        }

        tiempoRecogidaRestante = Mathf.Max(
            0f, tiempoRecogidaRestante - Time.deltaTime);

        if (tiempoRecogidaRestante <= 0f && colliderObjeto != null)
        {
            colliderObjeto.enabled = true;
        }
    }

    /// <summary>
    /// get: Obtiene el objeto.
    /// </summary>
    public Objeto Objeto
    {
        get { return objeto; }
    }

    /// <summary>
    /// get: Obtiene el valor de recogido.
    /// set: Cambia el valor de recogido a value.
    /// </summary>
    public bool Recogido
    {
        get { return recogido; }
        set { recogido = value; }
    }
}
