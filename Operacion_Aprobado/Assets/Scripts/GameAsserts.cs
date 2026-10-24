using UnityEngine;
using UnityEngine.Assertions;

public static class GameAsserts
{
    /// <summary>
    /// Comprueba que una referencia no sea null
    /// </summary>
    /// 
    /// <param name="target">Referencia a comprobar</param>
    /// <param name="targetName">Nombre de la referencia</param>
    public static void IsValidReference(Object target, string targetName)
    {
        Assert.IsNotNull(target, $"Variable '{targetName}' es null");
    }

    /// <summary>
    /// Comprueba que el valor sea mayor que 0
    /// </summary>
    /// 
    /// <param name="valor">Valor a comprobar</param>
    /// <param name="varName">Nombre de la variable</param>
    public static void IsGreaterThanZero(float valor, string varName)
    {
        Assert.IsTrue(valor > 0f, $"{varName}: Valor menor que 0");
    }

    /// <summary>
    /// Comprueba que el array no esté vacio
    /// </summary>
    /// 
    /// <typeparam name="T">Tipo del array</typeparam>
    /// <param name="array">Array a comprobar</param>
    /// <param name="arrayName">Nombre del array</param>
    public static void IsNotEmpty<T>(T[] array, string arrayName)
    {
        bool hasElements = array != null && array.Length > 0;
        Assert.IsTrue(hasElements, $"El array o lista '{arrayName}' es nulo o no contiene elementos.");
    }

    /// <summary>
    /// Comprueba que el Player esté en el estado correcto
    /// </summary>
    /// 
    /// <param name="estadoActual">Estado en el que se encuentra el jugador</param>
    /// <param name="estadoSupuesto">Estado en el que deberia estar</param>
    public static void IsValidPlayerState(Player.Estados estadoActual, Player.Estados estadoSupuesto)
    {
        Assert.AreEqual(estadoSupuesto, estadoActual,
                        $"El estado de Player es '{estadoActual}' y debería ser '{estadoSupuesto}'");
    }

    /// <summary>
    /// Comprueba que el Player esté en el estado equivocado
    /// </summary>
    /// 
    /// <param name="estadoActual">Estado en el que se encuentra el jugador</param>
    /// <param name="estadoProhibido">Estado en el que no deberia estar</param>
    public static void IsProhibitedPlayerState(Player.Estados estadoActual, Player.Estados estadoProhibido)
    {
        Assert.AreNotEqual(estadoProhibido, estadoActual,
                        $"El estado de Player es '{estadoActual}' y no debería estar en ese estado");
    }

    /// <summary>
    /// Comprueba que el valor esté entre dos números, incluyendolos
    /// </summary>
    /// 
    /// <param name="valor">Valor a comprobar</param>
    /// <param name="max">Valor máximo</param>
    /// <param name="min">Valor mínimo</param>
    /// <param name="valorName">Nombre del valor</param>
    public static void ValueBetweenIncluded(float valor, float max, float min, string valorName)
    {
        Assert.IsTrue(max >= valor && valor >= min, $"Valor '{valorName}' fuera de rango. Debería ser {max} >= '{valorName}' >= {min}");
    }
}
