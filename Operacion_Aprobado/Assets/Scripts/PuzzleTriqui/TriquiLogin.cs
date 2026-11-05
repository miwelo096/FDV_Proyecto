using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TriquiLogin : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelVentanilla;
    public GameObject panelTerminal;

    [Header("Elementos del Terminal")]
    public TextMeshProUGUI textoTerminal; 
    public TMP_InputField inputTerminal;
    public ScrollRect scrollRect; 

    [Header("Configuración")]
    public string SiguienteEscena = "SampleScene";
    public string usuarioCorrecto = "ZZZ.Z@triqui.fi.upm.es";
    public string contrasenaCorrecta = "C1AB3";

    // Variables de estado del juego
    // 0: Pidiendo usuario, 1: Pidiendo "yes", 2: Pidiendo contraseña, 3: Terminal normal
    private int estadoLogin = 0; 
    private bool enCarpetaSecreta = false;

    void Start()
    {
        // Todo empieza cerrado
        panelVentanilla.SetActive(false);
        panelTerminal.SetActive(false);
        
        // Configuramos el input para que al pulsar Enter se envíe el comando
        inputTerminal.onSubmit.AddListener(ProcesarComando);
        inputTerminal.ActivateInputField();
    }

    void Update()
    {
        // Detectar clic derecho en el escritorio
        if (Input.GetMouseButtonDown(1) && !panelTerminal.activeSelf)
        {
            panelVentanilla.SetActive(true);
        }

        if (Input.GetMouseButtonDown(0) && panelVentanilla.activeSelf)
        {
            // Si se hace clic fuera del panel, cerrarlo
            if (!RectTransformUtility.RectangleContainsScreenPoint(panelVentanilla.GetComponent<RectTransform>(), Input.mousePosition))
            {
                panelVentanilla.SetActive(false);
            }
        }

    }

    // FUNCIONES DE LOS BOTONES

    public void AbrirTerminal()
    {
        panelVentanilla.SetActive(false);
        panelTerminal.SetActive(true);
        textoTerminal.text = "upm@etsiinf:~/Escritorio$ ";
        inputTerminal.text = "";
        estadoLogin = 0;
    }

    public void CerrarTerminal()
    {
        panelTerminal.SetActive(false);
    }

    public void SalirEscena()
    {
        GameManager.SalirPuzzle(gameObject.scene);
    }

    // LÓGICA DEL TERMINAL

    public void ProcesarComando(string comando)
    {
        if (string.IsNullOrEmpty(comando)) return;

        // Escribimos lo que el jugador ha tecleado en la pantalla
        textoTerminal.text += comando + "\n";
        string cmd = comando.ToLower().Trim();

        // Evaluamos en qué estado estamos
        if (estadoLogin == 0)
        {
            string comandoEsperado = "ssh " + usuarioCorrecto.ToLower();
            
            if (cmd == comandoEsperado)
            {
                textoTerminal.text += "The authenticity of host 'triqui.fi.upm.es' can't be established.\n";
                textoTerminal.text += "Are you sure you want to continue connecting (yes/no)?\n> ";
                estadoLogin = 1;
            }
            else
            {
                textoTerminal.text += "Comando no reconocido o usuario incorrecto. Inténtalo de nuevo.\n> ";
            }
        }
        else if (estadoLogin == 1)
        {
            if (cmd == "yes")
            {
                textoTerminal.text += "Warning: Permanently added 'triqui.fi.upm.es' to the list of known hosts.\n";
                textoTerminal.text += "ZZZ.Z@triqui.fi.upm.es's password:\n> ";
                estadoLogin = 2;
            }
            else
            {
                textoTerminal.text += "Operación cancelada.\n> ";
                estadoLogin = 0;
            }
        }
        else if (estadoLogin == 2)
        {
            // Ocultamos la contraseña que se acaba de escribir en el historial
            textoTerminal.text = textoTerminal.text.Replace(comando, ""); 

            // Usamos comando.Trim() porque el jugador podría meter espacios sin querer
            if (comando.Trim() == contrasenaCorrecta)
            {
                textoTerminal.text += "\nConexión establecida.\n";
                textoTerminal.text += "Bienvenido a Triqui (triqui.fi.upm.es)\n> ";
                estadoLogin = 3; // Pasamos al terminal normal
            }
            else
            {
                // Si falla, volvemos a pedir la contraseña
                textoTerminal.text += "Permission denied, please try again.\n";
                textoTerminal.text += "ZZZ.Z@triqui.fi.upm.es's password:\n> ";
            }
        }
        else if (estadoLogin == 3)
        {
            // Navegación por el terminal
            if (cmd == "ls")
            {
                if (enCarpetaSecreta)
                    textoTerminal.text += "fichero.txt\n> ";
                else
                    textoTerminal.text += "Secreto\n> ";
            }
            else if (cmd == "cd secreto")
            {
                enCarpetaSecreta = true;
                textoTerminal.text += "Entrando en el directorio Secreto...\n> ";
            }
            else if (cmd == "cat fichero.txt")
            {
                if (enCarpetaSecreta)
                    textoTerminal.text += "Pegado debajo de la silla...\n> ";
                else
                    textoTerminal.text += "cat: fichero.txt: No such file or directory\n> ";
            }
            else
            {
                textoTerminal.text += "Comando no reconocido.\n> ";
            }
        }

        inputTerminal.text = "";
        inputTerminal.ActivateInputField();
        
    }

}
