using UnityEngine;

public class PlayerNoiseMaker : MonoBehaviour
{
    private Player playerScript;

    [SerializeField]
    private GameObject burbujaSonido;


    void Start()
    {
        playerScript = GetComponent<Player>();

        if (playerScript == null)
        {
            Debug.LogError($"No se ha obtenido script de Player ({name})");
        }
        if (burbujaSonido == null)
        {
            Debug.LogError($"No se ha asignado GameObject a burbujaSonido ({name})");
        }

        burbujaSonido.SetActive(false);
    }

    void Update()
    {
        if (playerScript != null && burbujaSonido != null)
        {
            bool corriendo = playerScript.getEstado() == Player.Estados.Corriendo &&
                        playerScript.getPlayerRB().linearVelocity.sqrMagnitude > 0.01f;

            burbujaSonido.SetActive(corriendo);
        }

        
    }

    private void OnDisable()
    {
        if (burbujaSonido != null)
        {
            burbujaSonido.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Constantes.ENEMYTAG))
        {
            Debug.Log("El enemigo oyó al jugador correr");
            // TODO: cambiar estado del enemigo que lo ha oido a SOSPECHA
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Constantes.ENEMYTAG))
        {
            Debug.Log("El enemigo ha dejado de oir al jugador corre");
        }
    }
}