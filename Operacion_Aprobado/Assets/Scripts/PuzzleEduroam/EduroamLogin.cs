using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class EduroamLogin : MonoBehaviour
{
    [Header("Referencias de UI")]
    public TMP_InputField campoUsuario;
    public TMP_InputField campoContrasena;
    public TextMeshProUGUI textoError; // Un texto para mostrar si fallan los datos

    [Header("Configuración")]
    public string usuarioCorrecto = "XXX.X@alumnos.upm.es"; // Cambia XXX.X 
    public string contrasenaCorrecta = "YYYYYYYY"; // Cambia YYYYYYYY 
    public string siguienteEscena = "SampleScene";


    void Start()
    {
        if (campoUsuario == null)
        {
            Debug.LogError($"No asignado input field a campoUsuario ({name})");
        }
        if (campoContrasena == null)
        {
            Debug.LogError($"No asignado input field a campoContrasena ({name})");
        }
        if (textoError == null)
        {
            Debug.LogError($"No asignado input field a textoError ({name})");
        }
    }

    public void PulsarConectar()
    {
        StartCoroutine(ComprobarConexion());
    }

    // Función que se ejecutará al pulsar el botón "Conectar"
    IEnumerator ComprobarConexion()
    {
        string usuarioInput = campoUsuario.text;
        string contrasenaInput = campoContrasena.text;

        if (usuarioInput == Constantes.USUARIOCORRECTO &&
            contrasenaInput == Constantes.CONTRASEÑACORRECTA)
        {
            Debug.Log("Login correcto. Volviendo al juego...");
            textoError.text = "Datos correctos.";
            textoError.color = Color.green;

            yield return new WaitForSeconds(1.5f);

            yield return new WaitUntil(() =>
                !PauseMenu.IsPaused &&
                !InputManager.PausaPulsadaEsteFrame);

            PulsarCancelar();
        }
        else
        {
            Debug.Log("Usuario o contraseña incorrectos.");

            if (textoError != null)
            {
                textoError.text = "Datos incorrectos. Inténtalo de nuevo.";
                textoError.color = Color.red;
            }

            campoContrasena.text = "";
        }
    }


    // Función que se ejecutará al pulsar el botón "Cancelar"
    public void PulsarCancelar()
    {
        GameManager.SalirPuzzle(gameObject.scene);
    }
}
