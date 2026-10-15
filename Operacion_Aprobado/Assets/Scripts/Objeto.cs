using UnityEngine;

[CreateAssetMenu(fileName = "Objeto", menuName = "Scriptable Objects/Objeto")]
public class Objeto : ScriptableObject
{
    public string id;
    public string descripcion;
    public Sprite icono;
    public GameObject prefab;
}
