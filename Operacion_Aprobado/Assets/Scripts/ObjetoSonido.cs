using UnityEngine;

public class ObjetoSonido : MonoBehaviour
{
    // Constante del diametro del sonido
    const float SONIDODIAM = 6f;

    // Mascara del enemigo
    public LayerMask enemyLayer;

    // Prefab de la onda sonora
    public GameObject ondaSonoraPrefab;

    private void Start()
    {
        // Destruye el objeto automáticamente a los 2 segundos
        Invoke("expirarPorTiempo", 2f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        rangoSonido();
        Destroy(gameObject);
    }

    /// <summary>
    /// Destruye el objeto lanzado tras obtener los enemigos dentro de su rango
    /// </summary>
    void expirarPorTiempo()
    {
        rangoSonido();
        Destroy(gameObject);
    }

    /// <summary>
    /// Dibuja el rango de sonido y obtiene los enemigos que estén dentro del rango de sonido
    /// </summary>
    void rangoSonido()
    {
        if (getOndaSonoraPrefab() != null)
        {
            GameObject onda = Instantiate(getOndaSonoraPrefab(), transform.position, Quaternion.identity);

            onda.transform.localScale = new Vector3(SONIDODIAM, SONIDODIAM, 1f);
            Destroy(onda, 0.5f);
        }
        // Crea la burbuja de sonido en la posición exacta del impacto
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, SONIDODIAM, enemyLayer);

        foreach (Collider2D enemy in enemiesInRange)
        {
            Debug.Log("Se ha escuchado un objeto romperse");
            // TODO: cambiar estado del enemigo que lo ha oido a SOSPECHA
        }
    }

    /// <summary>
    /// Obtiene el GameObject del prefab de onda sonora
    /// </summary>
    /// 
    /// <returns>GameObject del prefab de onda sonora</returns>
    public GameObject getOndaSonoraPrefab()
    {
        return this.ondaSonoraPrefab;
    }
}