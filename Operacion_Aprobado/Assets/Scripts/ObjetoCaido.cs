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
    private bool inicializado;

    void Awake()
    {
        colliderObjeto = GetComponent<Collider2D>();
        recogido = false;

        GameAsserts.IsValidReference(colliderObjeto, $"colliderObjeto ({name})");        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (autoStart && !inicializado)
        {
            Initialize(objeto);
        }
    }

    /// <summary>
    /// Inicializa el objeto y desactiva el collider hasta pasado un periodo de tiempo
    /// </summary>
    /// 
    /// <param name="nuevoObjeto">Objeto a inicializar</param>
    public bool Initialize(Objeto nuevoObjeto)
    {
        if (inicializado)
        {
            return false;
        }

        if (!isActiveAndEnabled || colliderObjeto == null)
        {
            Debug.LogError(
                "ObjetoCaido: necesita un componente activo y un Collider2D.",
                this);
            return false;
        }

        if (nuevoObjeto == null || nuevoObjeto.prefabSuelo == null)
        {
            Debug.LogError(
                "ObjetoCaido: faltan Objeto o Prefab Suelo.",
                this);
            return false;
        }

        GameObject visual = Instantiate(nuevoObjeto.prefabSuelo, transform);
        visual.transform.SetLocalPositionAndRotation(
            Vector3.zero, Quaternion.identity);

        objeto = nuevoObjeto;
        tiempoRecogidaRestante = Constantes.DELAYRECOGIDA;
        colliderObjeto.enabled = false;
        inicializado = true;

        return true;
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
