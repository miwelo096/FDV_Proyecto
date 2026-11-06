using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // Constantes de velocidad de movimiento
    const float VELOCIDADAGACHADO = 3f;             // Velocidad escondido = velocidad agachado
    const float VELOCIDADANDANDO = 5f;
    const float VELOCIDADCORRIENDO = 8f;

    const float VELOCIDADDESLIZAMIENTO = 12f;
    const float DURACIONDESLIZAMIENTO = 0.35f;

    const int VIDASINICIALES = 3;
    const float TIEMPOINVULNERABLE = 1f;      // Segundos sin poder recibir daño tras un golpe


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
    private Vector2 entradaMovimiento;

    // Vidas del jugador
    private int vidas;
    private float tiempoInvulnerable = 0f;
    // Rigidbody del jugador (Para colisiones)
    private Rigidbody2D playerRB;
    // Sprite del jugador
    private SpriteRenderer playerSprite;
    private Color colorOriginalEscondite;

    // Sprite del escondite
    private SpriteRenderer esconditeSprite;



    void Start()
    {
        this.playerRB = GetComponent<Rigidbody2D>();
        this.playerSprite = GetComponent<SpriteRenderer>();

        GameAsserts.IsValidReference(getPlayerRB(), "playerRB (Player)");
        GameAsserts.IsValidReference(getPlayerSprite(), "playerSprite (Player)");

        setEstado(Estados.Andando);
        vidas = VIDASINICIALES;
    }

    void Update()
    {
        if (PauseMenu.IsPaused || GameManager.InPuzzle)
        {
            entradaMovimiento = Vector2.zero;

            // FixedUpdate no se ejecuta cuando timeScale es cero.
            if (playerRB != null)
                playerRB.linearVelocity = Vector2.zero;

            return;
        }

        float inputHorizontal = Input.GetAxisRaw("Horizontal");
        float inputVertical = Input.GetAxisRaw("Vertical");

        entradaMovimiento =
            new Vector2(inputHorizontal, inputVertical).normalized;

        if (tiempoInvulnerable > 0f)
        {
            tiempoInvulnerable -= Time.deltaTime;
        }

        escondido();
    }


    void FixedUpdate()
    {
        if (playerRB == null)
            return;

        if (PauseMenu.IsPaused || GameManager.InPuzzle)
        {
            playerRB.linearVelocity = Vector2.zero;
            return;
        }

        // Abandonar las posturas permitidas cancela el deslizamiento.
        if (deslizando &&
            getEstado() != Estados.Agachado &&
            getEstado() != Estados.Escondido)
        {
            deslizando = false;
        }

        if (deslizando)
        {
            tiempoDeslizamiento = Mathf.Max(
                0f,
                tiempoDeslizamiento - Time.fixedDeltaTime);

            float t =
                1f - (tiempoDeslizamiento / DURACIONDESLIZAMIENTO);

            float velocidadActual = Mathf.Lerp(
                VELOCIDADDESLIZAMIENTO,
                VELOCIDADAGACHADO,
                t);

            playerRB.linearVelocity =
                direccionDeslizamiento * velocidadActual;

            if (tiempoDeslizamiento <= 0f)
            {
                deslizando = false;
            }

            return;
        }

        playerRB.linearVelocity =
            entradaMovimiento * getVelocidad();
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

        entradaMovimiento = Vector2.zero;
        deslizando = false;
        tiempoDeslizamiento = 0f;
        direccionDeslizamiento = Vector2.zero;

        if (playerRB != null)
        {
            playerRB.linearVelocity = Vector2.zero;
            playerRB.angularVelocity = 0f;
        }

        setSpriteEscondite(null);
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
    /// Detecta colisiones con enemigos
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            PerderVida();
        }
    }

    /// <summary>
    /// Resta una vida (mínimo 0) si no está en periodo de invulnerabilidad
    /// </summary>
    void PerderVida()
    {
        if (tiempoInvulnerable > 0f || vidas <= 0) return;

        vidas = Mathf.Max(vidas - 1, 0);
        tiempoInvulnerable = TIEMPOINVULNERABLE;

        Debug.Log($"Vidas restantes: {vidas}");
        if (vidas == 0)
        {
            SceneManager.LoadScene(gameObject.scene.name);
        }
    }

    /// <summary>
    /// Comprueba si está escondido o si ha dejado de esconderse
    /// </summary>
    void escondido()
    {
        Collider2D colliderEscondite = Physics2D.OverlapPoint(
            transform.position,
            LayerMask.GetMask("Mesa"));

        if (colliderEscondite != null)
        {
            SpriteRenderer spriteMesa =
                colliderEscondite.GetComponent<SpriteRenderer>();

            if (spriteMesa != getSpriteEscondite())
            {
                setSpriteEscondite(spriteMesa);

                if (spriteMesa != null)
                {
                    setTransparencia(0.3f, false);
                }
            }

            setEstado(Estados.Escondido);
            return;
        }

        setSpriteEscondite(null);

        if (getEstado() == Estados.Escondido)
        {
            setEstado(Estados.Agachado);
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
        if (this.estado == estado)
            return;

        this.estado = estado;

        bool posturaBaja =
            estado == Estados.Agachado ||
            estado == Estados.Escondido;

        int capaJugador = LayerMask.NameToLayer("Player");
        int capaMesa = LayerMask.NameToLayer("Mesa");

        if (Physics2D.GetIgnoreLayerCollision(capaJugador, capaMesa)
            != posturaBaja)
        {
            Physics2D.IgnoreLayerCollision(
                capaJugador,
                capaMesa,
                posturaBaja);
        }

        if (posturaBaja)
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
    /// <returns>Vidas restantes</returns>
    public int getVidas()
    {
        return this.vidas;
    }
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
        if (eliminiar)
        {
            setSpriteEscondite(null);
            return;
        }

        if (esconditeSprite == null)
            return;

        if (float.IsNaN(valor) || valor < 0f || valor > 1f)
        {
            Debug.LogWarning(
                "Player: la transparencia debe estar entre 0 y 1.",
                this);
            return;
        }

        Color color = esconditeSprite.color;
        color.a = valor;
        esconditeSprite.color = color;
    }

    /// <summary>
    /// Cambia el SpriteRenderer por el de "sprite"
    /// </summary>
    /// 
    /// <param name="sprite">SpriteRenderer a cambiar</param>
    public void setSpriteEscondite(SpriteRenderer sprite)
    {
        if (esconditeSprite == sprite)
            return;

        // Restauramos la mesa anterior antes de cambiar de referencia.
        if (esconditeSprite != null)
        {
            esconditeSprite.color = colorOriginalEscondite;
        }

        esconditeSprite = sprite;

        if (esconditeSprite != null)
        {
            colorOriginalEscondite = esconditeSprite.color;
        }
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
