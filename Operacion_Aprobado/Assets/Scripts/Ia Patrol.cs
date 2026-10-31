using UnityEngine;
using System.Collections;

public class IaPatrol : MonoBehaviour
{

    public Transform[] waypoints;
    private int waypointactual;
    public float speed;
    public float waitTime;
    private bool isWaiting;
    private CampodeVision campodeVision;
    private Rigidbody2D rb;
 



void Start()
    {
        // Obtenemos el componente correctamente en Start
        campodeVision = GetComponent<CampodeVision>();
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("No configurado bien el rigidbody");
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (campodeVision == null) { Debug.LogError("CampodeVision es null"); return; }

        if(!campodeVision.Viendo)
        {
            if (waypoints == null || waypoints.Length == 0) { Debug.LogError("No hay waypoints asignados"); return; }
            
            Transform objetivoWaypoint = waypoints[waypointactual];

            if(Vector2.Distance(rb.position, objetivoWaypoint.position) > 0.01f)
            {
                Vector2 nuevaPosi = Vector2.MoveTowards(rb.position, objetivoWaypoint.position, speed * Time.fixedDeltaTime);
                
                rb.MovePosition(nuevaPosi);

                RotarHacia(objetivoWaypoint.position);
            }
            else if(!isWaiting)
            {
                StartCoroutine(Wait());
            }
        }
        /*if(transform.position != waypoints[waypointactual].position){
            transform.position=Vector2.MoveTowards(transform.position,waypoints[waypointactual].position, speed * Time.deltaTime);
        }
        else if(!isWaiting)
        {
            StartCoroutine(Wait());
        }*/
    }

    IEnumerator Wait(){
        isWaiting=true;
        yield return new WaitForSeconds(waitTime);
        
        waypointactual = (waypointactual+1)%waypoints.Length;

        isWaiting=false;
        Flip();
    }



    private void RotarHacia(Vector2 destino) {
            // Calculamos la dirección vectorial desde nuestra posición hasta el destino
            Vector2 direccion = (destino - (Vector2)transform.position).normalized;

            // Calculamos el ángulo en grados usando Atan2
            float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;

            // Aplicamos la rotación en el eje Z (funciona para arriba, abajo, izq y der)
            transform.rotation = Quaternion.Euler(0f, 0f, angulo);
    }

    private void Flip()
    {
        // Si la posición actual es mayor que la del waypoint, va hacia la izquierda
        if (rb.position.x > waypoints[waypointactual].position.x)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f); // Mira a la izquierda
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);   // Mira a la derecha
        }
    }
}
