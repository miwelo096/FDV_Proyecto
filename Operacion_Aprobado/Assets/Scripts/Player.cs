using System;
using UnityEditor.Rendering;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Constantes de velocidad de movimiento
    const float VELOCIDADAGACHADO = 2f;
    const float VELOCIDADANDANDO = 5f;
    const float VELOCIDADCORRIENDO = 8f;

    public enum Estados
    {
        Agachado,   // 0
        Andando,    // 1
        Corriendo   // 2
    }

    // Valor de velocidad de movimiento
    private float velocidad;
    // Estado del jugador
    private Estados estado;
    private bool escondido;
    // Rigidbody para colisiones
    private Rigidbody2D rb;
    SpriteRenderer sr;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        setEstado(Estados.Andando);
    }

    void Update()
    {
        bool debajoDeMesa = Physics2D.OverlapBox(transform.position, GetComponent<BoxCollider2D>().bounds.size, 0f, LayerMask.GetMask("Mesa")) != null;

        /**
         * Si se pulsa el shift izquierdo, el jugador se agacha.
         * Si se pulsa el espacio, el jugador corre.
         * El jugador vuelve a andar pulsando de nuevo la misma tecla y
         * no se puede pasar de agacharse a correr ni vice versa.
         **/
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!getEscondido())
            {
                if (getEstado() == Estados.Andando)
                {
                    setEstado(Estados.Agachado);
                }
                else if (getEstado() == Estados.Agachado)
                {
                    setEstado(Estados.Andando);
                }
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Mesa"), getEstado() == Estados.Agachado);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (getEstado() == Estados.Andando)
            {
                setEstado(Estados.Corriendo);
            }
            else if (getEstado() == Estados.Corriendo)
            {
                setEstado(Estados.Andando);
            }
        }

        setEscondido(debajoDeMesa);

        float inputVertical = Input.GetAxisRaw("Vertical");
        float inputHorizontal = Input.GetAxisRaw("Horizontal");
        Vector2 input = new Vector2(inputHorizontal, inputVertical);
        rb.linearVelocity = input.normalized * getVelocidad();
    }


    /*******************
     * GETTERS/SETTERS *
     *******************/
    public void setEstado(Estados estado)
    {
        this.estado = estado;
        if (estado == Estados.Agachado)
        {
            sr.color = Color.yellow;
            setVelocidad(VELOCIDADAGACHADO);
        }
        else if (estado == Estados.Corriendo)
        {
            sr.color = Color.green;
            setVelocidad(VELOCIDADCORRIENDO);
        }
        else
        {
            sr.color = Color.red;
            setVelocidad(VELOCIDADANDANDO);
        }
    }
    
    public Estados getEstado()
    {
        return this.estado;
    }

    public void setVelocidad(float velocidad)
    {
        this.velocidad = velocidad;
    }

    public float getVelocidad()
    {
        return this.velocidad;
    }

    public void setEscondido(bool debajoMesa)
    {
        this.escondido = getEstado() == Estados.Agachado && debajoMesa;
    }

    public bool getEscondido()
    {
        return this.escondido;
    }
}
