using UnityEngine;

public class ObjetoSonido : MonoBehaviour
{
    public float noiseRad = 6f;
    public LayerMask enemyLayer;

    private void Start()
    {
        // Destruye el objeto automáticamente a los 2 segundos de ser instanciado
        Destroy(gameObject, 2f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Crea la burbuja de sonido en la posición exacta del impacto
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, noiseRad, enemyLayer);

        foreach (Collider2D enemy in enemiesInRange)
        {
            Debug.Log("Se ha escuchado un objeto romperse");
            // TODO: cambiar estado del enemigo que lo ha oido a SOSPECHA
        }
    }

    // Método para dibujar el círculo visualmente en la escena de Unity
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; // Color del círculo de la burbuja
        Gizmos.DrawWireSphere(transform.position, noiseRad);
    }
}