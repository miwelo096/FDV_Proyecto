using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerVidas : MonoBehaviour
{
    public int vidas { get; private set; }
    private float tiempoInvulnerable = 0f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vidas = Constantes.VIDASINICIALES;
    }

    // Update is called once per frame
    void Update()
    {
        if (tiempoInvulnerable > 0f)
        {
            tiempoInvulnerable -= Time.deltaTime;
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
        tiempoInvulnerable = Constantes.TIEMPOINVULNERABLE;

        Debug.Log($"Vidas restantes: {vidas}");
        if (vidas == 0)
        {
            SceneManager.LoadScene(gameObject.scene.name);
        }
    }
}
