using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Velocidad de movimiento
    public float velocidad = 5f;
    
    public bool agachado;
    public bool escondido;
    // Rigidbody para colisiones
    private Rigidbody2D rb;
    SpriteRenderer sr;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        agachado = false;
        sr = GetComponent<SpriteRenderer>();
    }
    void Update()
{
    bool debajoDeMesa = Physics2D.OverlapBox(transform.position, GetComponent<BoxCollider2D>().bounds.size, 0f, LayerMask.GetMask("Mesa")) != null;

    if (Input.GetKeyDown(KeyCode.LeftShift))
    {
        if (!agachado || !debajoDeMesa)
        {
            agachado = !agachado;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Mesa"), agachado);

            if (agachado)
            {
                sr.color = Color.yellow;
                velocidad = 2f;
            }
            else
            {
                sr.color = Color.red;
                velocidad = 5f;
            }
        }
    }

    escondido = agachado && debajoDeMesa;

    float inputVertical = Input.GetAxisRaw("Vertical");
    float inputHorizontal = Input.GetAxisRaw("Horizontal");
    Vector2 input = new Vector2(inputHorizontal, inputVertical);
    rb.linearVelocity = input.normalized * velocidad;
}
}
