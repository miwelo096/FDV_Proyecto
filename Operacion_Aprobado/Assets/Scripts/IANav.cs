using UnityEngine;
using UnityEngine.AI;

public class IANav : MonoBehaviour
{
    public float speed;
    public Transform player;
    public float disMin;

    private CampodeVision campodeVision;
    private IaPatrol patrol;
    private GameObject playerObj;
    private Rigidbody2D rb;
    private NavMeshAgent agent;
    private bool veiaAlJugador;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Start()
    {
        campodeVision = GetComponent<CampodeVision>();
        patrol = GetComponent<IaPatrol>();
        playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        if (agent != null)
        {
            agent.speed = speed;
        }
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (campodeVision == null || player == null || agent == null)
            return;

        if (campodeVision.Viendo)
        {
            veiaAlJugador = true;
            agent.isStopped = false;
            agent.speed = speed;
            Seguir();
            RotarHacia(player.position);
        }
        else if (veiaAlJugador)
        {
            // Acaba de perder de vista al jugador: retoma en el waypoint más cercano
            veiaAlJugador = false;
            if (patrol != null)
            {
                patrol.RetomarEnWaypointMasCercano();
            }
            else if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.ResetPath();
            }
        }
    }

    private void Atacar()
    {
        Debug.Log("Atacar");
    }

    private void Seguir()
    {
        if (Vector2.Distance(rb.position, player.position) > disMin)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;
            Atacar();
        }
    }

    private void RotarHacia(Vector2 destino)
    {
        Vector2 direccion = (destino - rb.position).normalized;
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        rb.MoveRotation(angulo);
    }
}