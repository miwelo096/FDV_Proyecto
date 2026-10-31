using UnityEngine;
using UnityEngine.SceneManagement;

public class conectarVPN : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelInicio;
    public GameObject panelApps;
    public GameObject panelVPN;
    public GameObject panelFin;
    public GameObject panelSorpresa;

    [Header("Elementos Específicos")]
    public GameObject imagenOn;
    public GameObject imagenOff;
    public GameObject botonConnect;
    public GameObject botonDisconnect;
    private bool vpnActivada = false; // Variable para saber si la VPN está encendida o apagada

    void Start()
    {
        // Al empezar, solo mostramos la pantalla de inicio
        panelInicio.SetActive(true);
        panelApps.SetActive(false);
        panelVPN.SetActive(false);
        panelFin.SetActive(false);
        panelSorpresa.SetActive(false);
        imagenOn.SetActive(false);
        imagenOff.SetActive(false);
        botonConnect.SetActive(false);
        botonDisconnect.SetActive(false);
    }

    // FUNCIONES DE LOS BOTONES

    public void PulsarCerrar()
    {
        panelInicio.SetActive(false);
        panelApps.SetActive(true);
    }

    public void PulsarOpenVPN()
    {
        if(vpnActivada)
        {
            imagenOn.SetActive(true);
            imagenOff.SetActive(false);
            botonConnect.SetActive(false);
            botonDisconnect.SetActive(true);
        }
        else
        {
            imagenOn.SetActive(false);
            imagenOff.SetActive(true);
            botonConnect.SetActive(true);
            botonDisconnect.SetActive(false);
        }
        panelApps.SetActive(false);
        panelVPN.SetActive(true);
    }

    public void PulsarVolver()
    {
        panelVPN.SetActive(false);
        panelSorpresa.SetActive(false);
        panelApps.SetActive(true);
    }

    public void PulsarConnect()
    {
        vpnActivada = !vpnActivada; // Cambia de true a false o viceversa

        if (vpnActivada)
        {
            imagenOn.SetActive(true);
            imagenOff.SetActive(false);
            botonConnect.SetActive(false);
            botonDisconnect.SetActive(true);
            Debug.Log("VPN Conectada");
        }
        else
        {
            imagenOn.SetActive(false);
            imagenOff.SetActive(true);
            botonConnect.SetActive(true);
            botonDisconnect.SetActive(false);
            Debug.Log("VPN Desconectada");
        }
    }

    public void PulsarSafari()
    {
        if(vpnActivada)
        {
            Debug.Log("Accediendo a la web de la UPM...");
            panelApps.SetActive(false);
            panelFin.SetActive(true);
        }
        else
        {
            Debug.Log("No se puede acceder a la web. La VPN no está conectada.");
            panelApps.SetActive(false);
            panelInicio.SetActive(true);
        }
    }

    public void PulsarSorpresa()
    {
        panelApps.SetActive(false);
        panelSorpresa.SetActive(true);
    }

    public void PulsarSalir()
    {
        Debug.Log("Cancelando... Volviendo a la escena principal.");
        SceneManager.LoadScene("SampleScene");
    }
}
