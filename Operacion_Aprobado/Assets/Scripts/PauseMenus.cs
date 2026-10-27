using UnityEngine;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(-100)]
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    public static bool IsPaused { get; private set; }

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
                "PauseMenu: falta asignar el panel de pausa.", this);
            enabled = false;
            return;
        }

        pausePanel.SetActive(false);
    }

    private void Update()
    {
        
    }

    public void Pause()
    {
        if (IsPaused || !isActiveAndEnabled || pausePanel == null)
            return;

        IsPaused = true;
        Time.timeScale = 0f;
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
    }

    private void OnDisable()
    {
        Resume();
    }

    // hay que modificar para que pueda restart en el level correspondiente.    
    /*
    public void Restart()
    {
        Time.timeScale = 1f;      
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    } 
    */
}