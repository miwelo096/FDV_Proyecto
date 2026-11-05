using UnityEngine;

public class OndaVisual : MonoBehaviour
{
    // SpriteRenderer del prefab
    private SpriteRenderer spriteOnda;
    
    // Tiempo transcurrido
    private float tiempoTranscurrido = 0f;


    private void Awake()
    {
        spriteOnda = GetComponent<SpriteRenderer>();

        if (spriteOnda == null)
        {
            Debug.LogError(
                $"OndaVisual: falta SpriteRenderer en '{name}'.", this);

            enabled = false;
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (PauseMenu.IsPaused || GameManager.InPuzzle)
            return;

        tiempoTranscurrido += Time.deltaTime;

        float alfa = Mathf.Clamp01(
            1f - tiempoTranscurrido / Constantes.TIEMPODEVIDA);

        Color colorActual = spriteOnda.color;
        colorActual.a = alfa;
        spriteOnda.color = colorActual;

        if (tiempoTranscurrido >= Constantes.TIEMPODEVIDA)
        {
            Destroy(gameObject);
        }
    }
}
