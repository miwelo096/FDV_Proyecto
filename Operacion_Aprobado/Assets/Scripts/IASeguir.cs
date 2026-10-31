using UnityEngine;

public class IASeguir : MonoBehaviour
{
    public float speed;
    public Transform player;
    public float disMin;
    private CampodeVision campodeVision;
    private GameObject playerObj;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        campodeVision = GetComponent<CampodeVision>();
        // Busca automáticamente el objeto que tenga la etiqueta "Player" en tu escena
        playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(campodeVision.Viendo){
            Seguir();
            RotarHacia(playerObj.transform.position);
        }
    }

    private void Atacar(){
        Debug.Log("Atacar");
    }

    private void Seguir(){
        if(Vector2.Distance(transform.position, player.position) > disMin){
            transform.position= Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
        else{
            Atacar();
        }
    }

    private void RotarHacia(Vector2 destino) {
        // Calculamos la dirección vectorial desde nuestra posición hasta el destino
        Vector2 direccion = (destino - (Vector2)transform.position).normalized;

        // Calculamos el ángulo en grados usando Atan2
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;

        // Aplicamos la rotación en el eje Z (funciona para arriba, abajo, izq y der)
        transform.rotation = Quaternion.Euler(0f, 0f, angulo);
}
}
