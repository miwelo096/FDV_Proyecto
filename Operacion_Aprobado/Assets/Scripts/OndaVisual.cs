using UnityEngine;

public class OndaVisual : MonoBehaviour
{
    // Constante de tiempo de vida
    const float TIEMPODEVIDA = 0.5f;
    
    // SpriteRenderer del prefab
    private SpriteRenderer spriteRenderer;
    
    // Tiempo transcurrido
    private float tiempoTranscurrido = 0f;


    private void Start()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (getSpriteOnda() != null) 
        {
            setTiempoTranscurrido(Time.deltaTime);
            float alfa = Mathf.Clamp01(1f - (getTiempoTranscurrido() / TIEMPODEVIDA));

            Color colorActual = getSpriteOnda().color;
            colorActual.a = alfa;
            getSpriteOnda().color = colorActual;
        }
        else
        {
            GameAsserts.IsValidReference(getSpriteOnda(), "spriteRenderer (OndaVisual)");
            Debug.Log("No tiene SpriteRenderer");
        }
    }

    /// <summary>
    /// Obtiene el SpriteRenderer
    /// </summary>
    /// 
    /// <returns>SpriteRenderer</returns>
    public SpriteRenderer getSpriteOnda()
    {
        return this.spriteRenderer;
    }

    /// <summary>
    /// Añade tiempo al tiempo transcurrido
    /// </summary>
    /// 
    /// <param name="time">Tiempo a añadir</param>
    public void setTiempoTranscurrido(float time)
    {
        this.tiempoTranscurrido += time;
    }

    /// <summary>
    /// Obtiene el tiempo transcurrido
    /// </summary>
    /// 
    /// <returns>Tiempo transcurrido</returns>
    public float getTiempoTranscurrido()
    {
        return this.tiempoTranscurrido;
    }
}
