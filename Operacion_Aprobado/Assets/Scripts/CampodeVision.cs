using UnityEngine;

public class CampodeVision : MonoBehaviour
{   
    // distancia de vision
    float distancia = 10f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        // inicializar posicion de raya y su distancia de vision
        Ray2D ray=new Ray2D( transform.position, Vector2.right );
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, distancia);

    }


    // Update is called once per frame
    void Update()
    {  
        if(info.collider!=null)
        {
            GameObject obj = info.collider.gameObject;
        
            if(obj.CompareTag("Player"))
                Debug.Log(obj.name);
            // comprobar estado
            if()
            // funcion de cambiar estado
        }
    }


    


}
