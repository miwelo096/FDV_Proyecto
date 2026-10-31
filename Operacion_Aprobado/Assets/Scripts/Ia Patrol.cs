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
 



void Start()
    {
        // Obtenemos el componente correctamente en Start
        campodeVision = GetComponent<CampodeVision>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {   
        if(!campodeVision.Viendo){
            if(transform.position != waypoints[waypointactual].position){
                Transform objetivoWaypoint = waypoints[waypointactual];
                transform.position=Vector2.MoveTowards(transform.position,waypoints[waypointactual].position, speed * Time.deltaTime);
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
        waypointactual++;
        if(waypointactual == waypoints.Length){
            waypointactual=0;
        }
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
    if (transform.position.x > waypoints[waypointactual].position.x)
    {
        transform.rotation = Quaternion.Euler(0f, 180f, 0f); // Mira a la izquierda
    }
    else
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);   // Mira a la derecha
    }
}
}
