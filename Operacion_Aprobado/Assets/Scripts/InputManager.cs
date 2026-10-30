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
        // Se pulsa Escape para pausar o continuar
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu pauseMenuScript = FindAnyObjectByType<PauseMenu>();
            if (PauseMenu.IsPaused)
                pauseMenuScript.Resume();
            else
                pauseMenuScript.Pause();
        }

        // Si el juego esta en pausa, ninguna tecla funciona salvo Escape
        if (PauseMenu.IsPaused) return;

        // Se pulsa Left Shift para agacharse o andar
        // Se pulsa Space para correr o andar
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            OnLShiftPressed?.Invoke();
        }
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
