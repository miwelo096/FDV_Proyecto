using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool InPuzzle = false;
    public GameObject mainScene;

    void Start()
    {   
        InPuzzle = false; 

        if (mainScene != null)
        {
            mainScene.SetActive(!InPuzzle);
        }
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