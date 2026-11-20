using System.Collections.Generic;
using UnityEngine;

public static class ObjetoDataBase
{
    private static Dictionary<string, Objeto> cache;

    private static void Build()
    {
        cache = new Dictionary<string, Objeto>();
        Objeto[] todos = Resources.LoadAll<Objeto>("Objetos");
        foreach (Objeto o in todos)
            cache[o.id] = o;
    }

    public static Objeto GetById(string id)
    {
        if (cache == null) Build();
        return cache.TryGetValue(id, out Objeto o) ? o : null;
    }
}
