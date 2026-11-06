using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-200)]
public class InputManager : MonoBehaviour
{
    public static event Action OnLShiftPressed;
    public static event Action OnSpacePressed;
    public static event Action OnQPressed;
    public static event Action OnEPressed;

    public static Vector2 Movimiento { get; private set; }

    private InputAction accionMovimiento;
    private InputAction accionPausa;
    private InputAction accionAgachado;
    private InputAction accionCarrera;
    private InputAction accionPrepararLanzamiento;
    private InputAction accionInteractuar;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ReiniciarEstado()
    {
        Movimiento = Vector2.zero;
    }

    private void Awake()
    {
        InputActionAsset acciones = InputSystem.actions;

        if (acciones == null)
        {
            Debug.LogError(
                "InputManager: falta asignar el asset de acciones del proyecto.",
                this);
            enabled = false;
            return;
        }

        InputActionMap mapaJugador =
            acciones.FindActionMap("Player", false);

        if (mapaJugador == null)
        {
            Debug.LogError(
                "InputManager: no se encuentra el mapa Player.",
                this);
            enabled = false;
            return;
        }

        accionMovimiento = mapaJugador.FindAction("Move", false);
        accionPausa = mapaJugador.FindAction("Pause", false);
        accionAgachado = mapaJugador.FindAction("Crouch", false);
        accionCarrera = mapaJugador.FindAction("Sprint", false);
        accionPrepararLanzamiento =
            mapaJugador.FindAction("PrepareThrow", false);
        accionInteractuar = mapaJugador.FindAction("Interact", false);

        if (accionMovimiento == null ||
            accionPausa == null ||
            accionAgachado == null ||
            accionCarrera == null ||
            accionPrepararLanzamiento == null ||
            accionInteractuar == null)
        {
            Debug.LogError(
                "InputManager: el mapa Player necesita las acciones " +
                "Move, Pause, Crouch, Sprint, PrepareThrow e Interact.",
                this);
            enabled = false;
            return;
        }

        // Conservamos los controles actuales de teclado y ratón.
        mapaJugador.bindingMask =
            InputBinding.MaskByGroup("Keyboard&Mouse");
    }

    private void Update()
    {
        // Cada frame comienza sin una dirección pendiente.
        Movimiento = Vector2.zero;

        // ESC reserva este frame para pausar o reanudar.
        if (accionPausa.WasPressedThisFrame())
        {
            PauseMenu pauseMenuScript =
                FindAnyObjectByType<PauseMenu>();

            if (pauseMenuScript == null ||
                !pauseMenuScript.isActiveAndEnabled)
            {
                Debug.LogError(
                    "InputManager: no hay un PauseMenu activo.",
                    this);
                return;
            }

            if (PauseMenu.IsPaused)
                pauseMenuScript.Resume();
            else
                pauseMenuScript.Pause();

            return;
        }

        if (PauseMenu.IsPaused || GameManager.InPuzzle)
            return;

        Movimiento =
            accionMovimiento.ReadValue<Vector2>().normalized;

        // Conservamos la prioridad de Shift sobre Espacio.
        if (accionAgachado.WasPressedThisFrame())
        {
            OnLShiftPressed?.Invoke();
        }
        else if (accionCarrera.WasPressedThisFrame())
        {
            OnSpacePressed?.Invoke();
        }

        if (accionPrepararLanzamiento.WasPressedThisFrame())
        {
            OnQPressed?.Invoke();
        }

        if (accionInteractuar.WasPressedThisFrame())
        {
            OnEPressed?.Invoke();
        }
    }

    private void OnDisable()
    {
        Movimiento = Vector2.zero;
    }
}