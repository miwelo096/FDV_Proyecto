using System;
using UnityEditor.Rendering;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Constantes de velocidad de movimiento
    const float VELOCIDADAGACHADO = 3f;             // Velocidad escondido = velocidad agachado
    const float VELOCIDADANDANDO = 5f;
    const float VELOCIDADCORRIENDO = 8f;

    // Constante de fuerza de lanzamiento
    const float FUERZALANZAMIENTO = 8f;

    // Estados posibles del jugador
    public enum Estados
    {
        Agachado,   // 0
        Andando,    // 1
        Corriendo,  // 2
        Escondido   // 3
    }

    // Valor de velocidad de movimiento
    private float velocidad;

    // Estado del jugador
    private Estados estado;

    // Rigidbody del jugador (Para colisiones)
    private Rigidbody2D playerRB;
    // Sprite del jugador
    private SpriteRenderer playerSprite;

    // Sprite del escondite
    private SpriteRenderer esconditeSprite;


    // Prefab objeto lanzable
    public GameObject prefabObjetoLanzable;
    // Posición desde la que se lanza un objeto
    public Transform puntoDeLanzamiento;


    void Start()
    {
        this.playerRB = GetComponent<Rigidbody2D>();
        this.playerSprite = GetComponent<SpriteRenderer>();
        
        GameAsserts.IsValidReference(getPlayerRB(), "playerRB (Player)");
        GameAsserts.IsValidReference(getPlayerSprite(), "playerSprite (Player)");
        GameAsserts.IsValidReference(this.prefabObjetoLanzable, "prefabObjetoLanzable (Player)");
        GameAsserts.IsValidReference(this.puntoDeLanzamiento, "puntoDeLanzamiento (Player)");
        setEstado(Estados.Andando);
    }

    void Update()
    {
        escondido();

        /**
         * Si se pulsa el shift izquierdo, el jugador se agacha.
         * Si se pulsa el espacio, el jugador corre.
         * El jugador vuelve a andar pulsando de nuevo la misma tecla.
         **/
        if (getEstado() != Estados.Escondido)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (getEstado() == Estados.Agachado)
                {
                    setEstado(Estados.Andando);
                }
                else
                {
                    setEstado(Estados.Agachado);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                if (getEstado() == Estados.Corriendo)
                {
                    setEstado(Estados.Andando);
                }
                else
                {
                    setEstado(Estados.Corriendo);
                }
            }
        }

        // Si el estado es AGACHADO, ignora las colisiones de la mascara "Mesa"
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Mesa"),
                                        getEstado() == Estados.Agachado || getEstado() == Estados.Escondido);


        float inputVertical = Input.GetAxisRaw("Vertical");
        float inputHorizontal = Input.GetAxisRaw("Horizontal");
        Vector2 input = new Vector2(inputHorizontal, inputVertical);
        getPlayerRB().linearVelocity = input.normalized * getVelocidad();
    }


    /// <summary>
    /// Comprueba si está escondido o si ha dejado de esconderse
    /// </summary>
    void escondido()
    {
        // Se comprueba que el jugador esté debajo de algo con el layer "Mesa" (Tiene que estar hasta la mitad del collider)
        Collider2D colliderEscondite = Physics2D.OverlapBox(transform.position, Vector2.zero,
                                                0f, LayerMask.GetMask("Mesa"));
        
        bool debajoMesa = colliderEscondite != null;

        if (debajoMesa)
        {
            setEstado(Estados.Escondido);
            setSpriteEscondite(colliderEscondite.GetComponent<SpriteRenderer>());
            setTransparencia(0.3f, false);
        }
        else if (getEstado() == Estados.Escondido && !debajoMesa)
        {
            setEstado(Estados.Agachado);
            setTransparencia(1f, true);
        }
    }



    /*******************
     * GETTERS/SETTERS *
     *******************/
    


    /// <summary>
    /// Cambia el estado del Player
    /// </summary>
    /// 
    /// <param name="estado">Estado a cambiar</param>
    public void setEstado(Estados estado)
    {
        this.estado = estado;
        if (estado == Estados.Agachado || estado == Estados.Escondido)
        {
            getPlayerSprite().color = Color.yellow;
            setVelocidad(VELOCIDADAGACHADO);
        }
        else if (estado == Estados.Corriendo)
        {
            getPlayerSprite().color = Color.green;
            setVelocidad(VELOCIDADCORRIENDO);
        }
        else
        {
            getPlayerSprite().color = Color.red;
            setVelocidad(VELOCIDADANDANDO);
        }
    }
    
    /// <summary>
    /// Obtiene el estado del Player
    /// </summary>
    /// 
    /// <returns>Estado del Player</returns>
    public Estados getEstado()
    {
        return this.estado;
    }

    /// <summary>
    /// Cambia la velocidad del movimiento del Player
    /// </summary>
    /// 
    /// <param name="velocidad">Velocidad a cambiar</param>
    public void setVelocidad(float velocidad)
    {
        this.velocidad = velocidad;
    }

    /// <summary>
    /// Obtiene la velocidad del movimiento del Player
    /// </summary>
    /// 
    /// <returns>Velocidad del movimiento del Player</returns>
    public float getVelocidad()
    {
        return this.velocidad;
    }

    /// <summary>
    /// Cambia el valor de la transparencia. Puede poner a null a la variable "esconditeSprite"
    /// </summary>
    /// 
    /// <param name="valor">Valor a cambiar</param>
    /// <param name="eliminiar">True pone a null a la variable "esconditeSprite"</param>
    public void setTransparencia(float valor, bool eliminiar)
    {
        GameAsserts.IsValidReference(getSpriteEscondite(), "esconditeSprite (Player.setTransparencia)");
        GameAsserts.ValueBetweenIncluded(valor, 1f, 0f, "valor (Player.setTransparencia)");

        Color color = getSpriteEscondite().material.color;
        color.a = valor;
        getSpriteEscondite().material.color = color;

        if (eliminiar)
        {
            setSpriteEscondite(null);
        }
    }

    /// <summary>
    /// Cambia el SpriteRenderer por el de "sprite"
    /// </summary>
    /// 
    /// <param name="sprite">SpriteRenderer a cambiar</param>
    public void setSpriteEscondite(SpriteRenderer sprite)
    {
        this.esconditeSprite = sprite;
    }

    /// <summary>
    /// Obtiene el SpriteRenderer de donde está escondido el Player
    /// </summary>
    /// 
    /// <returns>SpriteRenderer de donde se está escondido</returns>
    public SpriteRenderer getSpriteEscondite()
    {
        return this.esconditeSprite;
    }

    /// <summary>
    /// Obtiene el SpriteRenderer de Player
    /// </summary>
    /// 
    /// <returns>SpriteRenderer de Player</returns>
    public SpriteRenderer getPlayerSprite()
    {
        return this.playerSprite;
    }

    /// <summary>
    /// Obtiene el RigidBody de Player
    /// </summary>
    /// 
    /// <returns>RigidBody de Player</returns>
    public Rigidbody2D getPlayerRB()
    {
        return this.playerRB;
    }
}
