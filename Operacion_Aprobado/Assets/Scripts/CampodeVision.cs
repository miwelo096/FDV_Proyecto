using UnityEngine;

public class CampodeVision : MonoBehaviour
{
    public float distancia = 8f;
    public float angulo = 90f;
    public LayerMask obstaculos;
    public string tagObjetivo = "Player";

    // true si el objetivo está en rango, dentro del ángulo y sin obstáculos de por medio
    // el enemigo puede leer este campo para saber si debe cambiar de estado
    public bool Viendo;
    public Transform Objetivo;

    private Transform objetivo;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag(tagObjetivo);
        if (p != null) objetivo = p.transform;
    }

    void Update()
    {
        if (objetivo == null) return;

        Vector2 dir = objetivo.position - transform.position;

        Viendo = dir.magnitude <= distancia
            && Vector2.Angle(transform.right, dir) <= angulo / 2f
            && Physics2D.Raycast(transform.position, dir.normalized, dir.magnitude, obstaculos).collider == null;

        Objetivo = Viendo ? objetivo : null;
    }
}