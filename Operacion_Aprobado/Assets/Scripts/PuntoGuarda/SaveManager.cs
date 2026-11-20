using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("Configuración")]
    public int maxSaves = 5;
    private string saveFolder;

    // Lista de todas las partidas guardadas en memoria
    public List<ClaseSave> allSaves = new List<ClaseSave>();
     private ClaseSave pendingLoad = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        saveFolder = Path.Combine(Application.persistentDataPath, "Saves");
        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);

        LoadAllSavesFromDisk();
    }

    public ClaseSave NuevoSave(string saveName)
    {
        // Comprobar límite
        if (allSaves.Count >= maxSaves)
        {
            Debug.LogWarning("Límite de 10 guardados. Sobrescribe uno existente.");
            return null;
        }

        ClaseSave data = BuildCurrentSaveData();
        data.saveId = System.Guid.NewGuid().ToString();
        data.saveName = saveName;
        data.date = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        allSaves.Add(data);
        WriteSaveToDisk(data);
        return data;
    }

    public void SobreescribirSave(ClaseSave oldSave)
    {
        ClaseSave newData = BuildCurrentSaveData();
        newData.saveId = oldSave.saveId;     // Conserva el ID
        newData.saveName = oldSave.saveName; // Conserva el nombre
        newData.date = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // Actualiza la fecha

        int index = allSaves.FindIndex(s => s.saveId == oldSave.saveId);
        if (index >= 0)
        {
            allSaves[index] = newData;
            WriteSaveToDisk(newData);
        }
    }

    public void LoadSave(ClaseSave data)
    {
        if (data == null) return;

        pendingLoad = data;

        // Si estamos en otra escena, cargamos la del guardado
        if (SceneManager.GetActiveScene().name != data.sceneName)
            SceneManager.LoadScene(data.sceneName);
        else
            ApplyPendingLoad();
    }

    public void ApplyPendingLoad()
    {
        if (pendingLoad == null) return;

        ClaseSave data = pendingLoad;
        pendingLoad = null;

        // --- Jugador ---
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = data.playerPosicion;

            Player p = player.GetComponent<Player>();
            if (p != null) p.CambiarEstado((Player.Estados)data.playerEstado);

            PlayerVidas vidas = player.GetComponent<PlayerVidas>();
            if (vidas != null) vidas.SetVidas(data.playerVidas);

            // --- Inventario ---
            Inventario inv = player.GetComponent<Inventario>();
            if (inv != null && inv.Interfaz != null)
            {
                inv.LimpiarInventario();
                foreach (string id in data.inventarioItems)
                {
                    Objeto obj = ObjetoDataBase.GetById(id);
                    if (obj != null) inv.AddObjetoDesdeSave(obj);
                }
                inv.Interfaz.ActualizarInterfaz();
            }
        }

        // Narrativa
        // GameManager.Instance.SetNarrative(data.narrativeProgress);
        // GameManager.Instance.SetCollectedObjects(data.collectedObjects);

        Debug.Log("Partida cargada.");
    }

    private ClaseSave BuildCurrentSaveData()
    {
        ClaseSave data = new ClaseSave();
        data.sceneName = SceneManager.GetActiveScene().name;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            data.playerPosicion = player.transform.position;

            Player p = player.GetComponent<Player>();
            if (p != null) data.playerEstado = (int)p.EstadoPlayer;

            PlayerVidas vidas = player.GetComponent<PlayerVidas>();
            if (vidas != null) data.playerVidas = vidas.vidas;

            Inventario inv = player.GetComponent<Inventario>();
            if (inv != null)
            {
                data.inventarioItems.Clear();
                foreach (Objeto obj in inv.listaInventario)
                {
                    if (obj != null) data.inventarioItems.Add(obj.id);
                }
            }
        }

        return data;
    }

    // DISCO
    private void WriteSaveToDisk(ClaseSave data)
    {
        string path = Path.Combine(saveFolder, data.saveId + ".json");
        File.WriteAllText(path, JsonUtility.ToJson(data, true));
    }

    private void LoadAllSavesFromDisk()
    {
        allSaves.Clear();
        foreach (string file in Directory.GetFiles(saveFolder, "*.json"))
        {
            try
            {
                ClaseSave data = JsonUtility.FromJson<ClaseSave>(File.ReadAllText(file));
                if (data != null) allSaves.Add(data);
            }
            catch { Debug.LogWarning("Archivo corrupto: " + file); }
        }
    }

    // ELIMINAR
    public void DeleteAllSaves()
    {
        // Borrar los archivos físicos del disco
        if (System.IO.Directory.Exists(saveFolder))
        {
            string[] files = System.IO.Directory.GetFiles(saveFolder, "*.json");
            foreach (string file in files)
            {
                try
                {
                    System.IO.File.Delete(file);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"No se pudo borrar el archivo {file}: {e.Message}");
                }
            }
        }

        // Limpiar la lista en memoria
        allSaves.Clear();
        
        Debug.Log("Todos los guardados han sido eliminados.");
    }
}