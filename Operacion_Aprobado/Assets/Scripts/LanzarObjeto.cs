using JetBrains.Annotations;
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
            ObjetoLanzable lanzable = obtenerPrimerLanzable();

            if (lanzable != null && lanzable.prefabObjetoLanzable != null && puntoDeLanzamiento != null)
            {
                posicionRatonYLanzar(lanzable);

                inventarioScript.ConsumirObjeto(lanzable);
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
    /// Obtiene el primer objeto lanzable del inventario
    /// </summary>
    /// 
    /// <returns>Primer objeto lanzable del inventario</returns>
    ObjetoLanzable obtenerPrimerLanzable()
    {
        foreach(Objeto item in inventarioScript.listaInventario)
        {
            if (item is ObjetoLanzable lanzable)
            {
                return lanzable;
            }
        }
        return null;
    }

    /// <summary>
    /// Busca la posición del ratón y aplica fuerza al objeto hacia esa dirección
    /// </summary>
    /// 
    /// <param name="item">Objeto a lanzar</param>
    void posicionRatonYLanzar(ObjetoLanzable item)
    {
        GameObject objetoInstanciado = Instantiate(item.prefabObjetoLanzable, puntoDeLanzamiento.position, Quaternion.identity);
                
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
