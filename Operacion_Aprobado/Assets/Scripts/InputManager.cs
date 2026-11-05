using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{

    public static event Action OnLShiftPressed;
    public static event Action OnSpacePressed;
    public static event Action OnQPressed;
    public static event Action OnEPressed;



    // Update is called once per frame
    void Update()
    {
        // ESC se procesa antes que las acciones del mundo.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu pauseMenuScript = FindAnyObjectByType<PauseMenu>();

            if (pauseMenuScript == null)
            {
                Debug.LogError(
                    "InputManager: no hay un PauseMenu activo.", this);
                return;
            }

            if (PauseMenu.IsPaused)
                pauseMenuScript.Resume();
            else
                pauseMenuScript.Pause();

            return;
        }

        // Los eventos del mundo quedan bloqueados en ambos contextos.
        if (PauseMenu.IsPaused || GameManager.InPuzzle)
            return;

        // Se pulsa Left Shift para agacharse o andar
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            OnLShiftPressed?.Invoke();
        }
        // Se pulsa Space para correr o andar
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            OnSpacePressed?.Invoke();
        }

        // Se pulsa 'Q' para lanzar el objeto
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OnQPressed?.Invoke();
        }

        // Se pulsa 'E' para interactuar con objetos/puertas...
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnEPressed?.Invoke();
        }
    }
}
