using UnityEngine;

public class Player : MonoBehaviour
{
    // Velocidad de movimiento
    public float velocidad = 5f;
    
    // Rigidbody para colisiones
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        // Uso GetAxisRaw para que el movimiento sea preciso, sin suavizado
        float inputVertical = Input.GetAxisRaw("Vertical");
        float inputHorizontal = Input.GetAxisRaw("Horizontal");
        Vector2 input = new Vector2(inputHorizontal, inputVertical);


        
        rb.linearVelocity = input.normalized * velocidad;
    }
}
