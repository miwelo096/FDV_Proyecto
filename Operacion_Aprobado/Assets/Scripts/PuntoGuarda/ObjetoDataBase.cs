using System.Collections.Generic;
using UnityEngine;

public class ObjetoDataBase : MonoBehaviour
{
    public static ObjetoDataBase Instance { get; private set; }

    [Header("Arrastra aquí todos tus Objetos (ScriptableObjects)")]
    public List<Objeto> todosLosObjetos = new List<Objeto>();

    private Dictionary<string, Objeto> cache;

    private void Awake()
    {
        // Patrón Singleton para que sobreviva entre escenas
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Construimos el diccionario para búsquedas instantáneas
        cache = new Dictionary<string, Objeto>();
        foreach (Objeto obj in todosLosObjetos)
        {
            if (obj != null && !cache.ContainsKey(obj.id))
            {
                cache.Add(obj.id, obj);
            }
        }
    }

    public Objeto GetById(string id)
    {
        if (cache == null) return null;
        return cache.TryGetValue(id, out Objeto obj) ? obj : null;
    }
}
