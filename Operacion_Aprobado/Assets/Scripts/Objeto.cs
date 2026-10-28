using UnityEngine;

[CreateAssetMenu(fileName = "Objeto", menuName = "Scriptable Objects/Objeto")]
public class Objeto : ScriptableObject
{
    public string id;
    public string descripcion;
    public Sprite icono;
    public GameObject prefabSuelo;
    public GameObject prefabInventario;
}



[CreateAssetMenu(fileName = "Lanzable", menuName = "Scriptable Objects/Lanzable")]
public class ObjetoLanzable : Objeto
{
    public GameObject prefabObjetoLanzable;
}


[CreateAssetMenu(fileName = "Consumible", menuName = "Scriptable Objects/Consumible")]
public class ObjetoConsumible : Objeto
{
    public GameObject prefabObjetoConsumible;
}


[CreateAssetMenu(fileName = "Clave", menuName = "Scriptable Objects/Clave")]
public class ObjetoClave : Objeto
{
    public GameObject prefabObjetoClave;
}