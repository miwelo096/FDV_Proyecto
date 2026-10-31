using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Cambiar de escena

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

    // Función que se ejecutará al pulsar el botón "Conectar"
    public void PulsarConectar()
    {
        // Obtenemos lo que ha escrito el jugador
        string usuarioInput = campoUsuario.text;
        string contrasenaInput = campoContrasena.text;

        // Verificamos si coinciden con lo que queremos
        if (usuarioInput == usuarioCorrecto && contrasenaInput == contrasenaCorrecta)
        {
            Debug.Log("Login correcto. Cargando siguiente escena...");
            // Si es correcto, cargamos la siguiente escena (la imagen 2)
            SceneManager.LoadScene(siguienteEscena);
        }
        else
        {
            Debug.Log("Usuario o contraseña incorrectos.");
            if (textoError != null)
            {
                textoError.text = "Datos incorrectos. Inténtalo de nuevo.";
                textoError.color = Color.red;
            }
            // Borrar la contraseña para que lo intenten de nuevo
            campoContrasena.text = "";
        }
    }

    // Función que se ejecutará al pulsar el botón "Cancelar"
    public void PulsarCancelar()
    {
        Debug.Log("Cancelando... Volviendo a la escena principal.");
        // Cambia "EscenaPrincipal" por el nombre real de tu escena de menú
        SceneManager.LoadScene("SampleScene"); 
    }
}
