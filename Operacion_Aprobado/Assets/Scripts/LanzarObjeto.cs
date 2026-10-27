using UnityEngine;

public class LanzarObjeto : MonoBehaviour
{
    // Constante de fuerza de lanzamiento
    const float FUERZALANZAMIENTO = 8f;


    // Posición desde la que se lanza un objeto
    public Transform puntoDeLanzamiento;
    // Inventario del jugador
    public Inventario inventarioScript;



    // Update is called once per frame
    void Update()
    {   
        
    }

    /// <summary>
    /// Suscribe el metodo al evento de entrada
    /// </summary>
    void OnEnable()
    {
        InputManager.OnQPressed += lanzar;
    }

    /// <summary>
    /// Desuscribe el metodo del evento de entrada
    /// </summary>
    void OnDisable()
    {
        InputManager.OnQPressed -= lanzar;
    }


    /// <summary>
    /// Lanza un objeto del inventario
    /// </summary>
    public void lanzar()
    {
        if (PauseMenu.IsPaused)
            return;
            
        GameAsserts.IsValidReference(inventarioScript, "inventarioScript (LanzarObjeto)");
        GameAsserts.IsValidReference(puntoDeLanzamiento, "puntoDeLanzamiento (LanzarObjeto)");

        if (inventarioScript != null && inventarioScript.isNotEmpty())
        {
            int indice = obtenerIndicePrimerLanzable();
            Objeto item = inventarioScript.ObtenerObjeto(indice);


            if (item != null && item.prefabLanzar != null && puntoDeLanzamiento != null)
            {
                posicionRatonYLanzar(item);

                inventarioScript.ConsumirObjeto(indice);
            }
            else
            {
                Debug.Log("No encontrado objeto lanzable en el inventario");
            }
        }
        else
        {
            Debug.Log("Inventario vacio");
        }
    }

    /// <summary>
    /// Obtiene el indice del primer objeto lanzable en el inventario
    /// </summary>
    /// 
    /// <returns>Indice primer objeto lanzable</returns>
    int obtenerIndicePrimerLanzable()
    {
        for (int i = 0; i<inventarioScript.inventarioCount(); i++)
        {
            Objeto ob = inventarioScript.ObtenerObjeto(i);
            if (ob != null && ob.prefabLanzar != null && ob.lanzable)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Busca la posición del ratón aplica fuerza al objeto hacia esa dirección
    /// </summary>
    /// 
    /// <param name="item">Objeto a lanzar</param>
    void posicionRatonYLanzar(Objeto item)
    {
        GameObject objetoInstanciado = Instantiate(item.prefabLanzar, puntoDeLanzamiento.position, Quaternion.identity);
                
        var rbObjeto = objetoInstanciado.GetComponent<Rigidbody2D>();

        GameAsserts.IsValidReference(rbObjeto, "rbObjeto (LanzarObjeto.lanzar())");

        if (rbObjeto != null)
        {
            // Obtener la posición del ratón
            Vector2 posicionRaton = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            // Calcular la dirección normalizada desde el punto de lanzamiento hasta el ratón
            Vector2 direccion = (posicionRaton - (Vector2)puntoDeLanzamiento.position).normalized;

            // Aplicar la velocidad en esa dirección
            rbObjeto.linearVelocity = direccion * FUERZALANZAMIENTO;
        }
    }
}
