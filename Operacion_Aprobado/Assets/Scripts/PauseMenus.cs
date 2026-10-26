using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public static bool isPaused = false;

    void Update()
    {   
        //if (GameOverMenu.isGameOver) return; // if game over block pause

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) 
                Resume();
            else 
                Pause();
        }
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        FindAnyObjectByType<Player>().enabled = false; // desactivar update de usuario
        FindAnyObjectByType<LanzarObjeto>().enabled = false;

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;      
        isPaused = false;
        FindAnyObjectByType<Player>().enabled = true; // activar update de usuario
        FindAnyObjectByType<LanzarObjeto>().enabled = true;

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