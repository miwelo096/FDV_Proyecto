using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    public static bool IsPaused { get; private set; }

    private readonly Dictionary<Selectable, bool> controlesAntesDePausa =
        new Dictionary<Selectable, bool>();

    private readonly Dictionary<ScrollRect, bool> scrollAntesDePausa =
        new Dictionary<ScrollRect, bool>();

    private readonly Dictionary<ScrollRect, Vector2> velocidadesAntesDePausa =
        new Dictionary<ScrollRect, Vector2>();

    private bool restauracionPendiente;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ReiniciarEstado()
    {
        IsPaused = false;
        Time.timeScale = 1f;
    }

    private void Awake()
    {
        if (pausePanel == null)
        {
            Debug.LogError(
                "PauseMenu: falta asignar el panel de pausa.",
                this);
            enabled = false;
            return;
        }

        pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (IsPaused ||
            !restauracionPendiente ||
            InputManager.PausaPulsadaEsteFrame)
        {
            return;
        }

        Mouse raton = Mouse.current;

        // Una pulsación anterior debe terminar con la UI bloqueada.
        if (raton != null &&
            (raton.leftButton.wasReleasedThisFrame ||
             (raton.leftButton.isPressed &&
              !raton.leftButton.wasPressedThisFrame)))
        {
            return;
        }

        RestaurarUI();
    }

    public void Pause()
    {
        if (IsPaused || !isActiveAndEnabled || pausePanel == null)
            return;

        IsPaused = true;
        Time.timeScale = 0f;
        restauracionPendiente = false;

        BloquearUI();
        pausePanel.SetActive(true);

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void Resume()
    {
        if (!IsPaused)
            return;

        IsPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        restauracionPendiente = true;
    }

    private void BloquearUI()
    {
        foreach (Selectable control in FindObjectsByType<Selectable>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None))
        {
            if (control.transform.IsChildOf(pausePanel.transform))
                continue;

            if (!controlesAntesDePausa.ContainsKey(control))
            {
                controlesAntesDePausa.Add(
                    control, control.interactable);
            }

            control.interactable = false;
        }

        foreach (ScrollRect scroll in FindObjectsByType<ScrollRect>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None))
        {
            if (scroll.transform.IsChildOf(pausePanel.transform))
                continue;

            if (!scrollAntesDePausa.ContainsKey(scroll))
            {
                scrollAntesDePausa.Add(scroll, scroll.enabled);
                velocidadesAntesDePausa.Add(scroll, scroll.velocity);
            }

            scroll.enabled = false;
        }
    }

    private void RestaurarUI()
    {
        foreach (var estado in controlesAntesDePausa)
        {
            if (estado.Key != null)
                estado.Key.interactable = estado.Value;
        }

        foreach (var estado in scrollAntesDePausa)
        {
            ScrollRect scroll = estado.Key;

            if (scroll == null)
                continue;

            scroll.enabled = estado.Value;

            if (estado.Value &&
                velocidadesAntesDePausa.TryGetValue(
                    scroll, out Vector2 velocidad))
            {
                scroll.velocity = velocidad;
            }
        }

        controlesAntesDePausa.Clear();
        scrollAntesDePausa.Clear();
        velocidadesAntesDePausa.Clear();
        restauracionPendiente = false;
    }

    private void OnDisable()
    {
        Resume();
        RestaurarUI();
    }
}
