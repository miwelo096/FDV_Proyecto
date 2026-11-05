using UnityEngine;

public class IaPatrol : MonoBehaviour
{

    public Transform[] waypoints;
    private int waypointactual;
    public float speed;
    public float waitTime;
    private bool isWaiting;
    private float tiempoEsperaRestante;
    private CampodeVision campodeVision;
    private Rigidbody2D rb;
 

private void Awake()
    {
        // Obtenemos el componente correctamente en Start
        campodeVision = GetComponent<CampodeVision>();
        rb = GetComponent<Rigidbody2D>();

        if (rb == null || campodeVision == null)
        {
            Debug.LogError(
                "IaPatrol: se necesitan Rigidbody2D y CampodeVision.", this);

            enabled = false;
        }
    }

    private void OnDisable()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (rb == null)
            return;

        if (PauseMenu.IsPaused ||
            GameManager.InPuzzle ||
            campodeVision == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Durante la persecución, IASeguir controla el movimiento.
        if (campodeVision.Viendo)
            return;

        if (waypoints == null ||
            waypoints.Length == 0 ||
            waypointactual >= waypoints.Length ||
            waypoints[waypointactual] == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isWaiting)
        {
            rb.linearVelocity = Vector2.zero;

            tiempoEsperaRestante = Mathf.Max(
                0f, tiempoEsperaRestante - Time.fixedDeltaTime);

            if (tiempoEsperaRestante <= 0f)
            {
                waypointactual = (waypointactual + 1) % waypoints.Length;
                isWaiting = false;
                Flip();
            }

            return;
        }

        Transform objetivoWaypoint = waypoints[waypointactual];

        if (Vector2.Distance(rb.position, objetivoWaypoint.position) > 0.1f)
        {
            Vector2 direccion =
                ((Vector2)objetivoWaypoint.position - rb.position).normalized;

            rb.linearVelocity = direccion * speed;
            RotarHacia(objetivoWaypoint.position);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            isWaiting = true;
            tiempoEsperaRestante = Mathf.Max(0f, waitTime);
        }
    }

    private void RotarHacia(Vector2 destino) {
            // Calculamos la dirección vectorial desde nuestra posición hasta el destino
            Vector2 direccion = (destino - rb.position).normalized;

            // Calculamos el ángulo en grados usando Atan2
            float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;

            // Aplicamos la rotación en el eje Z (funciona para arriba, abajo, izq y der)
            rb.MoveRotation(angulo);
    }

    private void Flip()
    {
        // Si la posición actual es mayor que la del waypoint, va hacia la izquierda
        if (rb.position.x > waypoints[waypointactual].position.x)
        {
            rb.MoveRotation(180f); // Mira a la izquierda
        }
        else
        {
            rb.MoveRotation(0f);   // Mira a la derecha
        }
    }
}
