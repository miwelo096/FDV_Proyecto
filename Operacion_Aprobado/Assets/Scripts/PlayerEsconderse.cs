using UnityEngine;

public class PlayerEsconderse : MonoBehaviour
{
    private Color colorOriginalEscondite;
    // Sprite del escondite
    private SpriteRenderer esconditeSprite;

    private Player player;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Player>();

        GameAsserts.IsValidReference(player, $"player ({name})");
    }


    /// <summary>
    /// Comprueba si está escondido o si ha dejado de esconderse
    /// </summary>
    public void escondido()
    {
        Collider2D colliderEscondite = Physics2D.OverlapPoint(
            transform.position,
            LayerMask.GetMask("Mesa"));

        if (colliderEscondite != null)
        {
            SpriteRenderer spriteMesa =
                colliderEscondite.GetComponent<SpriteRenderer>();

            if (spriteMesa != esconditeSprite)
            {
                setSpriteEscondite(spriteMesa);

                if (spriteMesa != null)
                {
                    setTransparencia(0.3f, false);
                }
            }

            player.CambiarEstado(Player.Estados.Escondido);
            return;
        }

        setSpriteEscondite(null);

        if (player.EstadoPlayer == Player.Estados.Escondido)
        {
            player.CambiarEstado(Player.Estados.Agachado);
        }
    }

    /// <summary>
    /// Cambia el valor de la transparencia. Puede poner a null a la variable "esconditeSprite"
    /// </summary>
    /// 
    /// <param name="valor">Valor a cambiar</param>
    /// <param name="eliminiar">True pone a null a la variable "esconditeSprite"</param>
    void setTransparencia(float valor, bool eliminiar)
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
        if (esconditeSprite == sprite) return;

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
}
