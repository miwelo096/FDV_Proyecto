using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PuzzleBoardManager : MonoBehaviour
{
    [SerializeField] // Lista de Sprites de las piezas
    private List<Sprite> originalSprites = new List<Sprite>(Constantes.FILASCOLUMNAS*Constantes.FILASCOLUMNAS);

    // Tablero con el numero de pieza
    private int[] boardState;
    // Tablero con los GameObjects de las piezas
    private GameObject[] piezasVisuales;

    private bool estaMoviendo;
    private bool estaTerminado;
    private int totalPosiciones;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        totalPosiciones = Constantes.FILASCOLUMNAS*Constantes.FILASCOLUMNAS;
        estaMoviendo = false;
        estaTerminado = false;
        boardState = new int[totalPosiciones];
        piezasVisuales = new GameObject[totalPosiciones];

        if (originalSprites != null)
        {
            int i = 0;
            foreach (Sprite sprite in originalSprites)
            {
                if (sprite == null)
                {
                    Debug.LogError("Sprite_" + i + " no añadido (PuzzleDraco)");
                }
                i++;
            }
        }
        else
        {
            Debug.LogError("Lista 'origSprites' es null (PuzzleDraco)");
        }

        // Pone en orden los indices de las piezas
        for (int i = 0; i<boardState.Length; i++)
        {
            boardState[i] = i;
        }
        
        TableroAleatorio();
        InicializarTablero();
    }

    /// <summary>
    /// Mueve las fichas a una posición vecina aleatoria desde la solución para que sea matematicamente posible completar el puzzle.
    /// </summary>
    void TableroAleatorio()
    {
        int ultimoIndice = totalPosiciones-1;

        //Movimiento a hacer para que se aleatorice el puzzle
        int movimientos = Random.Range(20, 40);

        for (int i = 0; i<movimientos; i++)
        {
            int indice = System.Array.IndexOf(boardState, totalPosiciones-1);

            List<int> indicesVecinos = ComprobarPiezasVecinas(indice);

            indicesVecinos.Remove(ultimoIndice);

            int indiceAleatorio = Random.Range(0, indicesVecinos.Count);
            int indiceMovimiento = indicesVecinos[indiceAleatorio];

            ultimoIndice = indiceMovimiento;

            int temp = boardState[indice];
            boardState[indice] = boardState[indiceMovimiento];
            boardState[indiceMovimiento] = temp;
        }

        // En caso de que el tablero esté ordenado, se reorganiza de nuevo aleatoriamente.
        if (EstaOrdenado())
        {
            TableroAleatorio();
        }
    }

    /// <summary>
    /// Inicializa el tablero poniendo un GameObject con el sprite de cada pieza en su posición asignada.
    /// </summary>
    void InicializarTablero()
    {
        for (int i = 0; i<boardState.Length; i++)
        {
            int idPieza = boardState[i];

            if (idPieza == totalPosiciones-1) continue;

            GameObject piezaGO = new GameObject("Pieza_" + idPieza);
            piezaGO.transform.parent = this.transform;

            SpriteRenderer sr = piezaGO.AddComponent<SpriteRenderer>();
            sr.sprite = originalSprites[idPieza];
            sr.sortingOrder = 1;

            // Añadimos el script para que responda al ratón
            PuzzlePiece componentPiece = piezaGO.AddComponent<PuzzlePiece>();
            componentPiece.InicializarPieza(idPieza, this);

            // Añadimos BoxCollider2D para poder pulsar con el ratón
            piezaGO.AddComponent<BoxCollider2D>();

            piezaGO.transform.position = Posicion(i);

            // Guardamos la referencia en un array ordenado por su ID de pieza
            piezasVisuales[idPieza] = piezaGO;
        }
    }

    /// <summary>
    /// Obtiene los indices de las posibles piezas vecinas (Arriba, abajo, derecha, izquierda).
    /// </summary>
    /// 
    /// <param name="indice">Indice de la pieza a comprobar</param>
    /// <returns>Lista con los indices de las piezas vecinas</returns>
    List<int> ComprobarPiezasVecinas(int indice)
    {
        List<int> vecinos = new List<int>();

        int row = indice/Constantes.FILASCOLUMNAS;
        int column = indice%Constantes.FILASCOLUMNAS;

        if (row > 0) vecinos.Add(indice - Constantes.FILASCOLUMNAS);
        if (row < Constantes.FILASCOLUMNAS-1) vecinos.Add(indice + Constantes.FILASCOLUMNAS);
        if (column > 0) vecinos.Add(indice - 1);
        if (column < Constantes.FILASCOLUMNAS-1) vecinos.Add(indice + 1);

        return vecinos;
    }

    /// <summary>
    /// Se calcula la posición de la pieza en la escena.
    /// </summary>
    /// 
    /// <param name="indice">Indice de la pieza</param>
    /// <returns>Vector3 con la posicion de la pieza</returns>
    Vector2 Posicion(int indice)
    {
        int row = indice/Constantes.FILASCOLUMNAS;
        int column = indice%Constantes.FILASCOLUMNAS;
        float posX = (column-1) * Constantes.SPACING;
        float posY = (1-row) * Constantes.SPACING;

        return new Vector3(posX, posY, 0f);
    }

    /// <summary>
    /// Si no hay otra pieza moviendose, mueve la pieza a la posicion vacia.
    /// </summary>
    /// 
    /// <param name="idPiezaAMover">Indice de la pieza a mover</param>
    public void IntentarMoverPieza(int idPiezaAMover)
    {
        if (!isActiveAndEnabled ||
            PauseMenu.IsPaused ||
            InputManager.PausaPulsadaEsteFrame ||
            estaMoviendo ||
            estaTerminado)
        {
            return;
        }

        int indicePieza =
            System.Array.IndexOf(boardState, idPiezaAMover);

        int indiceVacio =
            System.Array.IndexOf(boardState, totalPosiciones - 1);

        List<int> listaPiezasVecinas =
            ComprobarPiezasVecinas(indiceVacio);

        if (listaPiezasVecinas.Contains(indicePieza))
        {
            boardState[indiceVacio] = idPiezaAMover;
            boardState[indicePieza] = totalPosiciones - 1;

            StartCoroutine(
                DeslizarPieza(piezasVisuales[idPiezaAMover], indiceVacio));
        }
    }

    /// <summary>
    /// Impide que se muevan otras piezas mientras se mueve esta.
    /// Desliza la pieza y comprueba si se ha completado el juego.
    /// En caso de que el juego se haya completado, cambia la escena.
    /// </summary>
    /// 
    /// <param name="pieza">GameObject de la pieza a mover</param>
    /// <param name="nuevoIndice">Indice de la nueva posición de la pieza</param>
    /// <returns></returns>
    IEnumerator DeslizarPieza(GameObject pieza, int nuevoIndice)
    {
        estaMoviendo = true;
        Vector2 posicionObjetivo = Posicion(nuevoIndice);

        while (Vector2.Distance(
            pieza.transform.position, posicionObjetivo) > 0.01f)
        {
            pieza.transform.position = Vector2.Lerp(
                pieza.transform.position,
                posicionObjetivo,
                Constantes.VELOCIDADDESPLAZAMIENTO * Time.deltaTime);

            yield return null;
        }

        pieza.transform.position = posicionObjetivo;
        estaMoviendo = false;

        if (EstaOrdenado())
        {
            estaTerminado = true;

            yield return new WaitForSeconds(1f);
            yield return new WaitUntil(() => !PauseMenu.IsPaused);

            GameManager.SalirPuzzle(gameObject.scene);
        }
    }

    /// <summary>
    /// Comprueba si el tablero está ordenado correctamente.
    /// </summary>
    /// 
    /// <returns>True si está ordenado</returns>
    bool EstaOrdenado()
    {
        for (int i = 0; i<boardState.Length-1; i++)
        {
            if (boardState[i] >= boardState[i+1]) return false;
        }
        return true;
    }
}
