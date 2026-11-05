using UnityEngine;

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

    public static void EntrarPuzzle()
    {
        InPuzzle = true;
        
        GameManager gm = FindAnyObjectByType<GameManager>();
        if (gm != null && gm.mainScene != null)
        {
            gm.mainScene.SetActive(false); 
        }
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