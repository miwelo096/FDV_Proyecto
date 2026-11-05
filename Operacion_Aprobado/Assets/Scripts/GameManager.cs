using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static bool InPuzzle = false;
    private static bool cerrandoPuzzle;
    public GameObject mainScene;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ReiniciarEstado()
    {
        InPuzzle = false;
        cerrandoPuzzle = false;
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
    private void CambiarActividadMundo(bool activo)
    {
        if (!activo)
        {
            foreach (ObjetoSonido proyectil in
                    mainScene.GetComponentsInChildren<ObjetoSonido>())
            {
                proyectil.GuardarMovimiento();
            }
        }

        mainScene.SetActive(activo);

        if (activo)
        {
            foreach (ObjetoSonido proyectil in
                    mainScene.GetComponentsInChildren<ObjetoSonido>())
            {
                proyectil.RestaurarMovimiento();
            }
        }
    }
    private void OnEnable()
{
    SceneManager.sceneLoaded += PrepararInterfazPuzzle;
}

private void OnDisable()
{
    SceneManager.sceneLoaded -= PrepararInterfazPuzzle;
}

private void PrepararInterfazPuzzle(Scene escena, LoadSceneMode modo)
    {
        if (!InPuzzle ||
            modo != LoadSceneMode.Additive ||
            escena == gameObject.scene)
        {
            return;
        }

        foreach (GameObject raiz in escena.GetRootGameObjects())
        {
            EventSystem[] sistemas =
                raiz.GetComponentsInChildren<EventSystem>(true);

            foreach (EventSystem sistema in sistemas)
            {
                sistema.gameObject.SetActive(false);
            }
        }

        if (EventSystem.current == null)
        {
            Debug.LogError(
                "GameManager: falta un EventSystem global activo.", this);
        }
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
            gm.CambiarActividadMundo(false);

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
        gm.CambiarActividadMundo(true);
    }

    public static void SalirPuzzle(Scene escenaPuzzle)
    {
        if (cerrandoPuzzle || PauseMenu.IsPaused)
            return;

        if (!escenaPuzzle.IsValid() || !escenaPuzzle.isLoaded)
        {
            Debug.LogError("GameManager: la escena del puzzle no está cargada.");
            return;
        }

        GameManager gm = FindAnyObjectByType<GameManager>();

        bool puzzleAislado =
            gm == null && SceneManager.sceneCount == 1;

        if (!puzzleAislado &&
            (gm == null || !gm.isActiveAndEnabled || gm.mainScene == null ||
            !InPuzzle || escenaPuzzle == gm.gameObject.scene))
        {
            Debug.LogError(
                "GameManager: no hay un contexto válido para cerrar el puzzle.");
            return;
        }

        if (puzzleAislado &&
            !Application.CanStreamedLevelBeLoaded(Constantes.ESCENAPRINCIPAL))
        {
            Debug.LogError("GameManager: no se puede cargar la escena principal.");
            return;
        }

        bool estadoAnterior = InPuzzle;
        cerrandoPuzzle = true;
        InPuzzle = true;

        try
        {
            AsyncOperation salida = puzzleAislado
                ? SceneManager.LoadSceneAsync(
                    Constantes.ESCENAPRINCIPAL, LoadSceneMode.Single)
                : SceneManager.UnloadSceneAsync(escenaPuzzle);

            if (salida != null)
            {
                salida.completed += _ =>
                {
                    InPuzzle = false;
                    cerrandoPuzzle = false;

                    if (gm != null && gm.mainScene != null)
                        gm.CambiarActividadMundo(true);
                };

                return;
            }

            Debug.LogError(
                "GameManager: no se pudo iniciar la salida del puzzle.", gm);
        }
        catch (System.Exception error)
        {
            Debug.LogError(
                $"GameManager: error al salir del puzzle: {error.Message}", gm);
        }

        InPuzzle = estadoAnterior;
        cerrandoPuzzle = false;
    }
}