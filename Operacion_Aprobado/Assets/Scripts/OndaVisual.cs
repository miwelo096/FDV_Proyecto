using UnityEngine;

public class OndaVisual : MonoBehaviour
{
    // SpriteRenderer del prefab
    private SpriteRenderer spriteOnda;
    
    // Tiempo transcurrido
    private float tiempoTranscurrido = 0f;


    private void Start()
    {
        spriteOnda = GetComponent<SpriteRenderer>();
        
        if (spriteOnda == null)
        {
            Debug.LogError($"No se ha encontrado el componente SpriteRenderer ({name})");
        }

        Destroy(gameObject, Constantes.TIEMPODEVIDA);
    }

    void Update()
    {
        if (spriteOnda != null) 
        {
            tiempoTranscurrido += Time.deltaTime;
            float alfa = Mathf.Clamp01(1f - (tiempoTranscurrido / Constantes.TIEMPODEVIDA));

            Color colorActual = spriteOnda.color;
            colorActual.a = alfa;
            spriteOnda.color = colorActual;
        }
        else
        {
            GameAsserts.IsValidReference(spriteOnda, $"spriteRenderer ({name})");
            Debug.Log("No tiene SpriteRenderer");
        }
    }
}
