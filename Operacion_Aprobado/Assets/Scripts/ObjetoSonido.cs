using UnityEngine;

public class ObjetoSonido : MonoBehaviour
{
    [SerializeField] // Prefab de la onda sonora
    private GameObject ondaSonoraPrefab;

    private bool yaEmitido = false;
    private float tiempoVidaRestante = 2f;
    private Rigidbody2D cuerpo;
    private Vector2 velocidadGuardada;
    private float velocidadAngularGuardada;
    private bool movimientoGuardado;

    private void Awake()
    {
        cuerpo = GetComponent<Rigidbody2D>();

        if (cuerpo == null)
        {
            Debug.LogError("ObjetoSonido: falta Rigidbody2D.", this);
        }
    }

    public void GuardarMovimiento()
    {
        if (cuerpo == null || yaEmitido || movimientoGuardado)
            return;

        velocidadGuardada = cuerpo.linearVelocity;
        velocidadAngularGuardada = cuerpo.angularVelocity;
        movimientoGuardado = true;
    }

    public void RestaurarMovimiento()
    {
        if (cuerpo == null || yaEmitido || !movimientoGuardado)
            return;

        cuerpo.linearVelocity = velocidadGuardada;
        cuerpo.angularVelocity = velocidadAngularGuardada;
        movimientoGuardado = false;
    }    
    private void Start()
    {
        if (ondaSonoraPrefab == null)
        {
            Debug.LogError(
                $"No se ha asignado el prefab de la onda sonora ({name})",
                this);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActiveAndEnabled ||
            PauseMenu.IsPaused ||
            GameManager.InPuzzle)
        {
            return;
        }

        EmitirSonidoUnico();
    }

    private void Update()
    {
        if (yaEmitido || PauseMenu.IsPaused || GameManager.InPuzzle)
            return;

        tiempoVidaRestante = Mathf.Max(
            0f, tiempoVidaRestante - Time.deltaTime);

        if (tiempoVidaRestante <= 0f)
        {
            EmitirSonidoUnico();
        }
    }

    void EmitirSonidoUnico()
    {
        if (yaEmitido) return;
        yaEmitido = true;

        if (ondaSonoraPrefab != null)
        {
            GameObject onda = Instantiate(
                ondaSonoraPrefab,
                transform.position,
                Quaternion.identity,
                transform.parent);

            onda.transform.localScale = new Vector2(Constantes.SONIDORAD, Constantes.SONIDORAD);
        }

        // Crea la burbuja de sonido en la posición exacta del impacto
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, Constantes.SONIDORAD/2, LayerMask.GetMask(Constantes.ENEMYLAYER));

        foreach (Collider2D enemy in enemiesInRange)
        {
            Debug.Log("Se ha escuchado un objeto romperse" + enemy);
            // TODO: cambiar estado del enemigo que lo ha oido a SOSPECHA
        }

        Destroy(gameObject);
    }
}