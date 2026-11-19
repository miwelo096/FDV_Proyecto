using UnityEngine;

public class Player : MonoBehaviour
{
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
    public Estados EstadoPlayer { get; private set; }

    private bool deslizando = false;

    private float tiempoDeslizamiento = 0f;
    private Vector2 direccionDeslizamiento;
    private Vector2 entradaMovimiento;

    // Rigidbody del jugador (Para colisiones)
    public Rigidbody2D playerRB { get; private set; }
    // Sprite del jugador
    private SpriteRenderer playerSprite;

    private PlayerEsconderse playerEsconderse;




    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerEsconderse = GetComponent<PlayerEsconderse>();
        CambiarEstado(Estados.Andando);

        GameAsserts.IsValidReference(playerRB, "playerRB (Player)");
        GameAsserts.IsValidReference(playerSprite, "playerSprite (Player)");
    }

    void Update()
    {
        if (GameManager.JuegoBloqueado)
        {
            entradaMovimiento = Vector2.zero;

            // FixedUpdate no se ejecuta cuando timeScale es cero.
            if (playerRB != null)
                playerRB.linearVelocity = Vector2.zero;

            return;
        }

        entradaMovimiento = InputManager.Movimiento;

        playerEsconderse.escondido();
    }

    void FixedUpdate()
    {
        if (playerRB == null)
            return;

        if (GameManager.JuegoBloqueado)
        {
            playerRB.linearVelocity = Vector2.zero;
            return;
        }

        // Abandonar las posturas permitidas cancela el deslizamiento.
        if (deslizando &&
            EstadoPlayer != Estados.Agachado &&
            EstadoPlayer != Estados.Escondido)
        {
            deslizando = false;
        }

        if (deslizando)
        {
            tiempoDeslizamiento = Mathf.Max(
                0f,
                tiempoDeslizamiento - Time.fixedDeltaTime);

            float t =
                1f - (tiempoDeslizamiento / Constantes.DURACIONDESLIZAMIENTO);

            float velocidadActual = Mathf.Lerp(
                Constantes.VELOCIDADDESLIZAMIENTO,
                Constantes.VELOCIDADAGACHADO,
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
            entradaMovimiento * velocidad;
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

        playerEsconderse.setSpriteEscondite(null);
    }

    /// <summary>
    /// Alterna entre los estados Andando y Agachado
    /// </summary>
    void AlternarAgachado()
    {
        if (EstadoPlayer == Estados.Escondido) return;

        if (EstadoPlayer == Estados.Agachado)
        {
            deslizando = false;              // Si se levanta a mitad del deslizamiento, se corta
            CambiarEstado(Estados.Andando);
        }
        else
        {
            // Solo se desliza si está corriendo Y moviéndose de verdad
            bool enMovimiento = playerRB.linearVelocity.sqrMagnitude > 0.01f;

            if (EstadoPlayer == Estados.Corriendo && enMovimiento)
            {
                deslizando = true;
                tiempoDeslizamiento = Constantes.DURACIONDESLIZAMIENTO;
                direccionDeslizamiento = playerRB.linearVelocity.normalized;
            }

            CambiarEstado(Estados.Agachado);
        }
    }

    /// <summary>
    /// Alterna entre los estados Andando y Corriendo
    /// </summary>
    void AlternarCorrer()
    {
        if (EstadoPlayer != Estados.Escondido)
        {
            if (EstadoPlayer == Estados.Corriendo)
            {
                CambiarEstado(Estados.Andando);
            }
            else
            {
                CambiarEstado(Estados.Corriendo);
            }
        }
    }



    /*******************
     * GETTERS/SETTERS *
     *******************/



    /// <summary>
    /// Cambia el estado del Player
    /// </summary>
    /// 
    /// <param name="nuevoEstado">Estado al que se va a cambiar</param>
    public void CambiarEstado(Estados nuevoEstado)
    {
        if (EstadoPlayer == nuevoEstado) return;

        EstadoPlayer = nuevoEstado;

        bool posturaBaja =
            nuevoEstado == Estados.Agachado ||
            nuevoEstado == Estados.Escondido;

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
            playerSprite.color = Color.yellow;
            velocidad = Constantes.VELOCIDADAGACHADO;
        }
        else if (nuevoEstado == Estados.Corriendo)
        {
            playerSprite.color = Color.green;
            velocidad = Constantes.VELOCIDADCORRIENDO;
        }
        else
        {
            playerSprite.color = Color.red;
            velocidad = Constantes.VELOCIDADANDANDO;
        }
    }
}
