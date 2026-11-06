using UnityEngine;
using TMPro;

public class UIVidas : MonoBehaviour
{
    // Player del que se leen las vidas
    [SerializeField]
    private Player player;

    // Texto donde se muestran las vidas
    private TextMeshProUGUI textoVidas;

    // Último valor mostrado (para no reescribir el texto cada frame)
    private int vidasMostradas = -1;


    private void Awake()
    {
        textoVidas = GetComponent<TextMeshProUGUI>();

        if (player == null)
        {
            Debug.LogError("UIVidas: falta asignar el Player.", this);
            enabled = false;
            return;
        }

        if (textoVidas == null)
        {
            Debug.LogError(
                "UIVidas: necesita un TextMeshProUGUI en el mismo objeto.",
                this);
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (player == null || textoVidas == null)
        {
            Debug.LogError(
                "UIVidas: se ha perdido una referencia necesaria.",
                this);
            enabled = false;
            return;
        }

        int vidas = player.getVidas();

        if (vidas == vidasMostradas)
            return;

        textoVidas.text = $"Vidas: {vidas}";
        vidasMostradas = vidas;
    }
}