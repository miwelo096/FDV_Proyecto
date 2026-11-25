using UnityEngine;
using UnityEngine.AI;

public class IaPatrol : MonoBehaviour
{
    public Transform[] waypoints;
    private int waypointactual;
    public float speed;
    public float waitTime;
    public float distanciaLlegada = 0.2f;

    private bool isWaiting;
    private float tiempoEsperaRestante;
    private CampodeVision campodeVision;
    private Rigidbody2D rb;
    private NavMeshAgent agent;

    private void Awake()
    {
        campodeVision = GetComponent<CampodeVision>();
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();

        GameAsserts.IsValidReference(campodeVision, $"campodeVision ({name})");
        GameAsserts.IsValidReference(rb, $"rb ({name})");
        GameAsserts.IsValidReference(agent, $"agent ({name})");

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void OnEnable()
    {
        if (agent != null)
        {
            agent.speed = speed;
            agent.isStopped = false;
        }
    }

    private void OnDisable()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.ResetPath();
        }
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    private void Update()
    {
        if (rb == null || agent == null || !agent.isOnNavMesh)
            return;

        if (GameManager.JuegoBloqueado || campodeVision == null)
        {
            agent.isStopped = true;
            return;
        }

        if (campodeVision.Viendo)
        {
            isWaiting = false;
            return;
        }

        if (waypoints == null ||
            waypoints.Length == 0 ||
            waypointactual < 0 ||
            waypointactual >= waypoints.Length ||
            waypoints[waypointactual] == null)
        {
            agent.isStopped = true;
            return;
        }

        agent.speed = speed;

        if (isWaiting)
        {
            agent.isStopped = true;
            tiempoEsperaRestante = Mathf.Max(0f, tiempoEsperaRestante - Time.deltaTime);

            if (tiempoEsperaRestante <= 0f)
            {
                waypointactual = (waypointactual + 1) % waypoints.Length;
                isWaiting = false;
                agent.isStopped = false;
                agent.SetDestination(waypoints[waypointactual].position);
            }
            return;
        }

        Transform objetivoWaypoint = waypoints[waypointactual];
        agent.isStopped = false;
        agent.SetDestination(objetivoWaypoint.position);

        if (!agent.pathPending && agent.remainingDistance <= distanciaLlegada)
        {
            agent.isStopped = true;
            isWaiting = true;
            tiempoEsperaRestante = Mathf.Max(0f, waitTime);
        }
        else if (agent.velocity.sqrMagnitude > 0.01f)
        {
            RotarHacia((Vector2)rb.position + (Vector2)agent.velocity);
        }
    }

    public void RetomarEnWaypointMasCercano()
    {
        if (waypoints == null || waypoints.Length == 0 || agent == null || !agent.isOnNavMesh)
            return;

        int indiceMasCercano = -1;
        float menorDistanciaSqr = Mathf.Infinity;
        Vector2 posicionActual = rb != null ? rb.position : (Vector2)transform.position;

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null)
                continue;

            float distSqr = ((Vector2)waypoints[i].position - posicionActual).sqrMagnitude;
            if (distSqr < menorDistanciaSqr)
            {
                menorDistanciaSqr = distSqr;
                indiceMasCercano = i;
            }
        }

        if (indiceMasCercano == -1)
            return;

        waypointactual = indiceMasCercano;
        isWaiting = false;
        tiempoEsperaRestante = 0f;
        agent.speed = speed;
        agent.isStopped = false;
        agent.SetDestination(waypoints[waypointactual].position);
    }

    private void RotarHacia(Vector2 destino)
    {
        Vector2 direccion = (destino - rb.position).normalized;
        if (direccion.sqrMagnitude < 0.0001f)
            return;

        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        rb.MoveRotation(angulo);
    }
}