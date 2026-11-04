using System.Runtime.CompilerServices;
using UnityEngine;

public static class Constantes
{
    // Tags
    // public const string PLAYERTAG = "Player";
    public const string ENEMYTAG = "Enemy";
    public const string ITEMTAG = "Item";
    public const string INTERACTIVOTAG = "Interactivo";



    // Layer Masks
    public const string PLAYERLAYER = "Player";
    public const string ENEMYLAYER = "Enemy";
    public const string PAREDLAYER = "Pared";
    public const string ITEMLAYER = "Item";
    public const string INTERACTIVOLAYER = "Interactivo";
    public const string SONIDOLAYER = "Sonido";
    public const string MESALAYER = "Mesa";



    // Escenas
    public const string ESCENAPRINCIPAL = "SampleScene";
    public const string ESCENATEST = "testScene";
    public const string ESCENADRACO = "PuzzleDracoScene";
    public const string ESCENAEDUROAM = "EduroamScene";
    public const string ESCENAVPN = "VPNScene";



    // Constantes de EduroamLogin
    public const string USUARIOCORRECTO = "XXX.X@alumnos.upm.es";
    public const string CONTRASEÑACORRECTA = "YYYYYYYY";



    // Constantes de Inventario
    public const int OBJETOSMAXIMOS = 10;



    // Constantes de LanzarObjeto
    public const float FUERZALANZAMIENTO = 9f;



    // Constantes de ObjetoCaido
    public const float DELAYRECOGIDA = 1.5f;



    // Constantes de ObjetoSonido
    public const float SONIDORAD = 6f;



    // Constantes de OndaVisual
    public const float TIEMPODEVIDA = 0.5f;



    // Constantes de Player
    public const float VELOCIDADAGACHADO = 3f;             // Velocidad escondido = velocidad agachado
    public const float VELOCIDADANDANDO = 5f;
    public const float VELOCIDADCORRIENDO = 8f;

    public const float VELOCIDADDESLIZAMIENTO = 12f;
    public const float DURACIONDESLIZAMIENTO = 0.35f;

    public const int VIDASINICIALES = 3;
    public const float TIEMPOINVULNERABLE = 1f;            // Segundos sin poder recibir daño tras un golpe



    // Constantes de PuzzleBoardManager
    public const int FILASCOLUMNAS = 3;
    public const float SPACING = 2.1f;
    public const float VELOCIDADDESPLAZAMIENTO = 15f;
}
