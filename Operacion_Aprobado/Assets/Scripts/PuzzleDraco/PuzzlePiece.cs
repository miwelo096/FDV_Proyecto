using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    private int miIdPieza;
    private PuzzleBoardManager managerTablero;


    /// <summary>
    /// Inicializa la pieza guardando el indice y el script del manager
    /// </summary>
    /// 
    /// <param name="id">Indice de la pieza</param>
    /// <param name="manager">Script del manager</param>
    public void InicializarPieza(int id, PuzzleBoardManager manager)
    {
        miIdPieza = id;
        managerTablero = manager;
        
    }

    /// <summary>
    /// Unity detecta cuando se hace click izquierdo sobre el Collider de este objeto e intenta mover la pieza
    /// </summary>
    void OnMouseDown()
    {
        managerTablero.IntentarMoverPieza(miIdPieza);
    }
}