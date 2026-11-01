using UnityEngine;

public class PlayerNoiseMaker : MonoBehaviour
{
    private Collider2D colliderSonido;
    public GameObject burbujaSonido;
    public Player playerScript;

    void Start()
    {
        if (burbujaSonido != null)
        {
            burbujaSonido.SetActive(false);
            colliderSonido = burbujaSonido.GetComponent<Collider2D>();
            if (colliderSonido == null)
            {
                Debug.LogError("No se ha obtenido el collider de burbujaSonido");
            }
        }
    }

    void Update()
    {
        bool corriendo = playerScript != null && playerScript.getEstado() == Player.Estados.Corriendo;

        if (burbujaSonido != null)
        {
            burbujaSonido.SetActive(corriendo);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("¡El enemigo oyó al jugador correr!");
            // TODO: cambiar estado del enemigo que lo ha oido a SOSPECHA
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("El enemigo ha dejado de oir al jugador corre");
        }
    }
}