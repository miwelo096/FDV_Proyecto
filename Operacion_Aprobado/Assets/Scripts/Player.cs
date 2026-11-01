using UnityEngine;

public class Player : MonoBehaviour
{
    // Constantes de velocidad de movimiento
    const float VELOCIDADAGACHADO = 3f;             // Velocidad escondido = velocidad agachado
    const float VELOCIDADANDANDO = 5f;
    const float VELOCIDADCORRIENDO = 8f;

    const float VELOCIDADDESLIZAMIENTO = 12f;
    const float DURACIONDESLIZAMIENTO = 0.35f;


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

    private bool deslizando = false;

    private float tiempoDeslizamiento = 0f;
    private Vector2 direccionDeslizamiento;
    // Rigidbody del jugador (Para colisiones)
    private Rigidbody2D playerRB;
    // Sprite del jugador
    private SpriteRenderer playerSprite;

    // Sprite del escondite
    private SpriteRenderer esconditeSprite;



    void Start()
    {
        this.playerRB = GetComponent<Rigidbody2D>();
        this.playerSprite = GetComponent<SpriteRenderer>();

        GameAsserts.IsValidReference(getPlayerRB(), "playerRB (Player)");
        GameAsserts.IsValidReference(getPlayerSprite(), "playerSprite (Player)");
        
        setEstado(Estados.Andando);
    }

    void Update()
    {
        if (PauseMenu.IsPaused)
        {
            if (playerRB != null)
                playerRB.linearVelocity = Vector2.zero;

            return;
        }

        escondido();

        // Si el estado es AGACHADO, ignora las colisiones de la mascara "Mesa"
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Mesa"),
                                        getEstado() == Estados.Agachado || getEstado() == Estados.Escondido);

        // Si cambia de estado durante el deslizamiento (p. ej. pulsa Espacio), se cancela
        if (deslizando && getEstado() != Estados.Agachado && getEstado() != Estados.Escondido)
        {
            deslizando = false;
        }

        if (deslizando)
        {
            tiempoDeslizamiento -= Time.deltaTime;

            float t = 1f - (tiempoDeslizamiento / DURACIONDESLIZAMIENTO);
            float velocidadActual = Mathf.Lerp(VELOCIDADDESLIZAMIENTO, VELOCIDADAGACHADO, t);

            getPlayerRB().linearVelocity = direccionDeslizamiento * velocidadActual;

            if (tiempoDeslizamiento <= 0f)
            {
                deslizando = false;
            }
            return;
        }
        float inputVertical = Input.GetAxisRaw("Vertical");
        float inputHorizontal = Input.GetAxisRaw("Horizontal");
        Vector2 input = new Vector2(inputHorizontal, inputVertical);
        getPlayerRB().linearVelocity = input.normalized * getVelocidad();
    }

    /// <summary>
    /// Suscribe los metodos a los eventos de entrada
    /// </summary>
    private void OnEnable()
    {
        InputManager.OnLShiftPressed += AlternarAgachado;
        InputManager.OnSpacePressed += AlternarCorrer;
    }

    /// <summary>
    /// Desuscribe los metodos de los eventos de entrada
    /// </summary>
    private void OnDisable()
    {
        InputManager.OnLShiftPressed -= AlternarAgachado;
        InputManager.OnSpacePressed -= AlternarCorrer;
    }

    /// <summary>
    /// Alterna entre los estados Andando y Agachado
    /// </summary>
    void AlternarAgachado()
    {
        if (getEstado() == Estados.Escondido) return;

        if (getEstado() == Estados.Agachado)
        {
            deslizando = false;              // Si se levanta a mitad del deslizamiento, se corta
            setEstado(Estados.Andando);
        }
        else
        {
            // Solo se desliza si está corriendo Y moviéndose de verdad
            bool enMovimiento = getPlayerRB().linearVelocity.sqrMagnitude > 0.01f;

            if (getEstado() == Estados.Corriendo && enMovimiento)
            {
                deslizando = true;
                tiempoDeslizamiento = DURACIONDESLIZAMIENTO;
                direccionDeslizamiento = getPlayerRB().linearVelocity.normalized;
            }

            setEstado(Estados.Agachado);
        }
    }

    /// <summary>
    /// Alterna entre los estados Andando y Corriendo
    /// </summary>
    void AlternarCorrer()
    {
        if (getEstado() != Estados.Escondido)
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
