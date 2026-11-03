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


    void Start()
    {
        textoVidas = GetComponent<TextMeshProUGUI>();

        GameAsserts.IsValidReference(player, "player (HUDVidas)");
        GameAsserts.IsValidReference(textoVidas, "textoVidas (HUDVidas)");
    }

    void Update()
    {
        int vidas = player.getVidas();

        if (vidas != vidasMostradas)
        {
            vidasMostradas = vidas;
            textoVidas.text = $"Vidas: {vidas}";
        }
    }
}