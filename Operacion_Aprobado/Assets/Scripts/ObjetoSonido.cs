using UnityEngine;

public class ObjetoSonido : MonoBehaviour
{
    [SerializeField] // Prefab de la onda sonora
    private GameObject ondaSonoraPrefab;

    private bool yaEmitido = false;


    private void Start()
    {
        if (ondaSonoraPrefab == null)
        {
            Debug.LogError($"No se ha asignado el prefab de la onda sonora ({name})");
        }

        // Destruye el objeto automáticamente a los 2 segundos
        Invoke("expirarPorTiempo", 2f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EmitirSonidoUnico();
    }

    /// <summary>
    /// Destruye el objeto lanzado tras obtener los enemigos dentro de su rango
    /// </summary>
    void expirarPorTiempo()
    {
        EmitirSonidoUnico();
    }

    void EmitirSonidoUnico()
    {
        if (yaEmitido) return;
        yaEmitido = true;

        if (ondaSonoraPrefab != null)
        {
            GameObject onda = Instantiate(ondaSonoraPrefab, transform.position, Quaternion.identity);

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