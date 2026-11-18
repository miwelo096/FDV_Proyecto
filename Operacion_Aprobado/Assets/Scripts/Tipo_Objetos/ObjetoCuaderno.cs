using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cuaderno", menuName = "Scriptable Objects/Cuaderno")]
public class ObjetoCuaderno : Objeto
{
    [SerializeField]
    private List<string> textos = new List<string>();

    public string ObtenerTexto(int pagina)
    {
        if (pagina >= 0 && pagina < textos.Count)
        {
            return textos[pagina];
        }

        return null;
    }
}
