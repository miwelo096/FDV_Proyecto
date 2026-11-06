using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzlePiece : MonoBehaviour, IPointerDownHandler
{
    private int miIdPieza;
    private PuzzleBoardManager managerTablero;

    public void InicializarPieza(int id, PuzzleBoardManager manager)
    {
        if (manager == null)
        {
            Debug.LogError(
                "PuzzlePiece: falta el gestor del tablero.",
                this);
            enabled = false;
            return;
        }

        miIdPieza = id;
        managerTablero = manager;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData == null ||
            eventData.button != PointerEventData.InputButton.Left ||
            PauseMenu.IsPaused ||
            InputManager.PausaPulsadaEsteFrame)
        {
            return;
        }

        if (managerTablero == null ||
            !managerTablero.isActiveAndEnabled)
        {
            return;
        }

        managerTablero.IntentarMoverPieza(miIdPieza);
    }
}