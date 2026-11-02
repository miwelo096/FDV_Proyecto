using UnityEngine;

public class PlayerNoiseMaker : MonoBehaviour
{
    private Collider2D colliderSonido;
    private Player playerScript;

    [SerializeField]
    private GameObject burbujaSonido;


    void Start()
    {
        if (burbujaSonido != null)
        {
            burbujaSonido.SetActive(false);
            colliderSonido = burbujaSonido.GetComponent<Collider2D>();
            if (colliderSonido == null)
            {
                Debug.LogError($"No se ha obtenido el collider de burbujaSonido ({name})");
            }
        }
        else
        {
            Debug.LogError($"No se ha asignado GameObject a burbujaSonido ({name})");
        }

        playerScript = GetComponent<Player>();
        if (playerScript == null)
        {
            Debug.LogError($"No se ha obtenido script de Player ({name})");
        }
    }

    void Update()
    {
        bool corriendo = playerScript.getEstado() == Player.Estados.Corriendo &&
                        playerScript.getPlayerRB().linearVelocity.sqrMagnitude > 0.01f;
        
        burbujaSonido.SetActive(corriendo);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("El enemigo oyó al jugador correr");
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