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
    public static Vector2 PosicionRaton { get; private set; }

    public static bool ConfirmarLanzamientoPulsado { get; private set; }
    public static bool CancelarLanzamientoPulsado { get; private set; }
    public static bool PausaPulsadaEsteFrame { get; private set; }

    private InputAction accionMovimiento;
    private InputAction accionPosicionRaton;
    private InputAction accionPausa;
    private InputAction accionAgachado;
    private InputAction accionCarrera;
    private InputAction accionPrepararLanzamiento;
    private InputAction accionConfirmarLanzamiento;
    private InputAction accionCancelarLanzamiento;
    private InputAction accionInteractuar;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ReiniciarEstado()
    {
        LimpiarEntrada();
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
        accionPosicionRaton = mapaJugador.FindAction("Point", false);
        accionPausa = mapaJugador.FindAction("Pause", false);
        accionAgachado = mapaJugador.FindAction("Crouch", false);
        accionCarrera = mapaJugador.FindAction("Sprint", false);
        accionPrepararLanzamiento =
            mapaJugador.FindAction("PrepareThrow", false);
        accionConfirmarLanzamiento =
            mapaJugador.FindAction("ConfirmThrow", false);
        accionCancelarLanzamiento =
            mapaJugador.FindAction("CancelThrow", false);
        accionInteractuar = mapaJugador.FindAction("Interact", false);

        if (accionMovimiento == null ||
            accionPosicionRaton == null ||
            accionPausa == null ||
            accionAgachado == null ||
            accionCarrera == null ||
            accionPrepararLanzamiento == null ||
            accionConfirmarLanzamiento == null ||
            accionCancelarLanzamiento == null ||
            accionInteractuar == null)
        {
            Debug.LogError(
                "InputManager: el mapa Player necesita las acciones " +
                "Move, Point, Pause, Crouch, Sprint, PrepareThrow, " +
                "ConfirmThrow, CancelThrow e Interact.",
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
        LimpiarEntrada();

        PausaPulsadaEsteFrame = accionPausa.WasPressedThisFrame();

        // ESC reserva este frame para pausar o reanudar.
        if (PausaPulsadaEsteFrame)
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

        // Las coordenadas de pantalla se conservan sin normalizar.
        PosicionRaton = accionPosicionRaton.ReadValue<Vector2>();

        ConfirmarLanzamientoPulsado =
            accionConfirmarLanzamiento.WasPressedThisFrame();

        CancelarLanzamientoPulsado =
            accionCancelarLanzamiento.WasPressedThisFrame();

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
        LimpiarEntrada();
    }

    private static void LimpiarEntrada()
    {
        Movimiento = Vector2.zero;
        PosicionRaton = Vector2.zero;
        ConfirmarLanzamientoPulsado = false;
        CancelarLanzamientoPulsado = false;
        PausaPulsadaEsteFrame = false;
    }
}