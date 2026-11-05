using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool InPuzzle = false;
    public GameObject mainScene;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ReiniciarEstado()
    {
        InPuzzle = false;
    }

    private void Awake()
    {
        if (mainScene == null)
        {
            Debug.LogError(
                "GameManager: falta asignar MainGameRoot en Main Scene.", this);
            return;
        }

        if (transform.IsChildOf(mainScene.transform))
        {
            Debug.LogError(
                "GameManager: el gestor debe estar fuera de MainGameRoot.", this);
            return;
        }

        mainScene.SetActive(true);
    }

    public static void EntrarPuzzle(string nombreEscena)
    {
        if (InPuzzle || PauseMenu.IsPaused)
            return;

        GameManager gm = FindAnyObjectByType<GameManager>();

        if (gm == null || !gm.isActiveAndEnabled || gm.mainScene == null)
        {
            Debug.LogError(
                "GameManager: falta un gestor activo con Main Scene asignado.");
            return;
        }

        if (gm.transform.IsChildOf(gm.mainScene.transform))
        {
            Debug.LogError(
                "GameManager: el gestor debe estar fuera de MainGameRoot.", gm);
            return;
        }

        if (string.IsNullOrWhiteSpace(nombreEscena) ||
            !Application.CanStreamedLevelBeLoaded(nombreEscena))
        {
            Debug.LogError(
                $"GameManager: no se puede cargar la escena '{nombreEscena}'.", gm);
            return;
        }

        string nombreSimple =
            System.IO.Path.GetFileNameWithoutExtension(nombreEscena);

        Scene escenaExistente = SceneManager.GetSceneByName(nombreSimple);

        if (escenaExistente.IsValid() && escenaExistente.isLoaded)
        {
            Debug.LogError(
                $"GameManager: la escena '{nombreSimple}' ya está cargada.", gm);
            return;
        }

        // El bloqueo comienza antes de iniciar la carga.
        InPuzzle = true;

        try
        {
            gm.mainScene.SetActive(false);

            AsyncOperation carga = SceneManager.LoadSceneAsync(
                nombreEscena, LoadSceneMode.Additive);

            if (carga != null)
                return;

            Debug.LogError(
                $"GameManager: no se pudo iniciar la carga de '{nombreEscena}'.",
                gm);
        }
        catch (System.Exception error)
        {
            Debug.LogError(
                $"GameManager: error al abrir '{nombreEscena}': {error.Message}",
                gm);
        }

        // Si no se pudo iniciar la carga, recuperamos el mundo.
        InPuzzle = false;
        gm.mainScene.SetActive(true);
    }

    public static void SalirPuzzle()
    {
        InPuzzle = false;
        
        GameManager gm = FindAnyObjectByType<GameManager>();
        if (gm != null && gm.mainScene != null)
        {
            gm.mainScene.SetActive(true); 
        }
    }
}