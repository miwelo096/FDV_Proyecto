using UnityEditor.Build;
using UnityEngine;

public class PlayerNoiseMaker : MonoBehaviour
{
    public Collider2D colliderSonido;
    public GameObject burbujaSonido;
    public Player playerScript;

    void Start()
    {
        if (colliderSonido != null)
        {
            colliderSonido.enabled = false; // Desactivado por defecto
        }
        if (burbujaSonido != null)
        {
            burbujaSonido.SetActive(false);
        }
    }

    void Update()
    {
        bool corriendo = playerScript != null && playerScript.getEstado() == Player.Estados.Corriendo;

        if (colliderSonido != null)
        {
            colliderSonido.enabled = corriendo;
        }
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