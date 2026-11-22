using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static bool InPuzzle = false;
    private static bool cerrandoPuzzle;
    public GameObject mainScene;

    private bool mostrandoPanelCompletado = false;
    private float temporizadorCierre = 0f;
    private Scene escenaPuzzleActual;

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
            Debug.LogError("GameManager: falta asignar MainGameRoot en Main Scene.", this);
            return;
        }

        if (transform.IsChildOf(mainScene.transform))
        {
            Debug.LogError("GameManager: el gestor debe estar fuera de MainGameRoot.", this);
            return;
        }

        mainScene.SetActive(true);
    }
    
    private void Update()
    {
        if (mostrandoPanelCompletado && temporizadorCierre > 0f)
        {
            temporizadorCierre -= Time.unscaledDeltaTime;

            if (temporizadorCierre <= 0f)
            {
                mostrandoPanelCompletado = false;
                
                if (escenaPuzzleActual.IsValid() && escenaPuzzleActual.isLoaded)
                {
                    Scene escena = escenaPuzzleActual;
                    escenaPuzzleActual = default;
                }
            }
        }
    }
    
    private void CambiarActividadMundo(bool activo)
    {
        if (!activo)
        {
            foreach (ObjetoSonido proyectil in mainScene.GetComponentsInChildren<ObjetoSonido>())
            {
                proyectil.GuardarMovimiento();
            }
        }

        mainScene.SetActive(activo);

        if (activo)
        {
            foreach (ObjetoSonido proyectil in mainScene.GetComponentsInChildren<ObjetoSonido>())
            {
                proyectil.RestaurarMovimiento();
            }
        }
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += PrepararInterfazPuzzle;
        SceneManager.sceneLoaded += CargarGuardados;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= PrepararInterfazPuzzle;
        SceneManager.sceneLoaded -= CargarGuardados;
    }

    private void PrepararInterfazPuzzle(Scene escena, LoadSceneMode modo)
    {
        if (!InPuzzle || modo != LoadSceneMode.Additive || escena == gameObject.scene)
            return;

        foreach (GameObject raiz in escena.GetRootGameObjects())
        {
            EventSystem[] sistemas = raiz.GetComponentsInChildren<EventSystem>(true);
            foreach (EventSystem sistema in sistemas)
            {
                sistema.gameObject.SetActive(false);
            }
        }

        if (EventSystem.current == null)
        {
            Debug.LogError("GameManager: falta un EventSystem global activo.", this);
        }
    }

    public static void EntrarPuzzle(string nombreEscena)
    {
        if (InPuzzle || PauseMenu.IsPaused) return;

        GameManager gm = FindAnyObjectByType<GameManager>();

        if (gm == null || !gm.isActiveAndEnabled || gm.mainScene == null)
        {
            Debug.LogError("GameManager: falta un gestor activo con Main Scene asignado.");
            return;
        }

        if (gm.transform.IsChildOf(gm.mainScene.transform))
        {
            Debug.LogError("GameManager: el gestor debe estar fuera de MainGameRoot.", gm);
            return;
        }

        if (string.IsNullOrWhiteSpace(nombreEscena) || !Application.CanStreamedLevelBeLoaded(nombreEscena))
        {
            Debug.LogError($"GameManager: no se puede cargar la escena '{nombreEscena}'.", gm);
            return;
        }

        string nombreSimple = System.IO.Path.GetFileNameWithoutExtension(nombreEscena);
        Scene escenaExistente = SceneManager.GetSceneByName(nombreSimple);

        if (escenaExistente.IsValid() && escenaExistente.isLoaded)
        {
            Debug.LogError($"GameManager: la escena '{nombreSimple}' ya está cargada.", gm);
            return;
        }

        InPuzzle = true;

        try
        {
            gm.CambiarActividadMundo(false);
            AsyncOperation carga = SceneManager.LoadSceneAsync(nombreEscena, LoadSceneMode.Additive);

            if (carga != null) return;

            Debug.LogError($"GameManager: no se pudo iniciar la carga de '{nombreEscena}'.", gm);
        }
        catch (System.Exception error)
        {
            Debug.LogError($"GameManager: error al abrir '{nombreEscena}': {error.Message}", gm);
        }

        InPuzzle = false;
        gm.CambiarActividadMundo(true);
    }

    public static void PuzzleCompletado(Scene escenaPuzzle, float autoCerrarSegundos = 0f)
    {
        if (!InPuzzle || cerrandoPuzzle) return;

        if (!escenaPuzzle.IsValid() || !escenaPuzzle.isLoaded)
        {
            Debug.LogError("GameManager: PuzzleCompletado recibió una escena no válida.");
            return;
        }

        GameManager gm = FindAnyObjectByType<GameManager>();
        if (gm == null || !gm.isActiveAndEnabled) return;

        gm.escenaPuzzleActual = escenaPuzzle;
        gm.mostrandoPanelCompletado = true; 
        gm.temporizadorCierre = autoCerrarSegundos;
    }

    // Dibuja el texto directamente en pantalla sin fondo
    private void OnGUI()
    {
        if (!mostrandoPanelCompletado || !InPuzzle) return; 

        GUIStyle style = new GUIStyle();
        style.fontSize = Screen.height / 10; 
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = new Color(1f, 0.8f, 0f);

        GUIStyle shadowStyle = new GUIStyle(style);
        shadowStyle.normal.textColor = Color.black;

        // Sombra
        GUI.Label(new Rect(3, 3, Screen.width, Screen.height), "¡PUZZLE COMPLETADO!", shadowStyle);

        // Texto principal
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "¡PUZZLE COMPLETADO!", style);
    }

    public static void SalirPuzzle(Scene escenaPuzzle)
    {
        if (cerrandoPuzzle || PauseMenu.IsPaused) return;

        if (!escenaPuzzle.IsValid() || !escenaPuzzle.isLoaded)
        {
            Debug.LogError("GameManager: la escena del puzzle no está cargada.");
            return;
        }

        GameManager gm = FindAnyObjectByType<GameManager>();
        bool puzzleAislado = gm == null && SceneManager.sceneCount == 1;

        if (!puzzleAislado && (gm == null || !gm.isActiveAndEnabled || gm.mainScene == null ||
            !InPuzzle || escenaPuzzle == gm.gameObject.scene))
        {
            Debug.LogError("GameManager: no hay un contexto válido para cerrar el puzzle.");
            return;
        }

        bool estadoAnterior = InPuzzle;
        cerrandoPuzzle = true;
        InPuzzle = true;

        try
        {
            AsyncOperation salida = puzzleAislado
                ? SceneManager.LoadSceneAsync(Constantes.ESCENAPRINCIPAL, LoadSceneMode.Single)
                : SceneManager.UnloadSceneAsync(escenaPuzzle);

            if (salida != null)
            {
                salida.completed += _ =>
                {
                    InPuzzle = false;
                    cerrandoPuzzle = false;

                    if (gm != null)
                    {
                        gm.mostrandoPanelCompletado = false;
                        gm.temporizadorCierre = 0f;
                    }

                    if (gm != null && gm.mainScene != null)
                        gm.CambiarActividadMundo(true);
                };
                return;
            }
        }
        catch (System.Exception error)
        {
            Debug.LogError($"GameManager: error al salir del puzzle: {error.Message}", gm);
        }

        InPuzzle = estadoAnterior;
        cerrandoPuzzle = false;
    }

    private void CargarGuardados(Scene escena, LoadSceneMode modo)
    {
        if (SaveManager.Instance != null && modo == LoadSceneMode.Single)
        {
            if (escena.name == SaveManager.Instance.EscenaPendienteDeCarga) StartCoroutine(EsperarYAplicar());
        }
    }

    private System.Collections.IEnumerator EsperarYAplicar()
    {
        yield return null;
        if (SaveManager.Instance != null) SaveManager.Instance.ApplyPendingLoad();
    }
}