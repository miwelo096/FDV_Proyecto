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
    private bool viendoAntes = false;

    private LineRenderer lineaDerecha;
    private LineRenderer lineaIzquierda;
    private Player player;
    private bool avisoReferenciaAusente = false;
    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag(tagObjetivo);
        if (p != null)
        {
            objetivo = p.transform;
            player = p.GetComponent<Player>();
        } 

        lineaDerecha = CrearLinea("VisionDer");
        lineaIzquierda = CrearLinea("VisionIzq");
    }

    // visualizar campo de vision
    LineRenderer CrearLinea(string nombre)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(transform);
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.yellow;
        lr.endColor = Color.yellow;
        return lr;
    }

    void Update()
    {
        Viendo = false;
        Objetivo = null;

        if (objetivo == null || player == null)
        {
            if (!avisoReferenciaAusente)
            {
                string motivo = objetivo == null
                    ? $"No hay objetivo con el tag '{tagObjetivo}' o ha desaparecido."
                    : "El objetivo no tiene un componente Player válido.";
                
                Debug.LogWarning($"{name}: {motivo}", this);
                avisoReferenciaAusente = true;
            }
        }
        else
        {
            avisoReferenciaAusente = false;
            
            Vector2 dir = objetivo.position - transform.position;

            Viendo = dir.magnitude <= distancia
                && Vector2.Angle(transform.right, dir) <= angulo / 2f
                && Physics2D.Raycast(
                    transform.position,
                    dir.normalized,
                    dir.magnitude,
                    obstaculos
                ).collider == null
                && !player.getEscondido();
            
            Objetivo = Viendo ? objetivo : null;
        }

        Vector3 forward = transform.right * distancia;
        Vector3 der = transform.position + Quaternion.Euler(0, 0, -angulo / 2f) * forward;
        Vector3 izq = transform.position + Quaternion.Euler(0, 0,  angulo / 2f) * forward;

        lineaDerecha.SetPosition(0, transform.position);
        lineaDerecha.SetPosition(1, der);

        lineaIzquierda.SetPosition(0, transform.position);
        lineaIzquierda.SetPosition(1, izq);


        // actualizar el feedback cuando cambia la dirección
        if (Viendo != viendoAntes)
        {
            viendoAntes = Viendo;

            Color colorVision = Viendo ? Color.red : Color.yellow;

            lineaDerecha.startColor = colorVision;
            lineaDerecha.endColor = colorVision;
            lineaIzquierda.startColor = colorVision;
            lineaIzquierda.endColor = colorVision;

            Debug.Log($"{name}: {(Viendo ? "Te veo" : "Te perdí")}", this);
        }
    }
}