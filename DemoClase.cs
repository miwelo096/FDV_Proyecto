// =====================================================================
//  DemoClase.cs  ·  FdV L2-1 Unity3D: Introducción y Scripting
// ---------------------------------------------------------------------
//  USO: arrastra este script a cualquier GameObject (vale uno vacío) y Play.
//  Para cambiar de demo: comenta/descomenta la línea #define (Ctrl+/).
//  Puedes activar varias a la vez. Para la demo antes de cambiar.
//  La salida sale en la ventana Console.
//  Input: usa el Input System nuevo (por defecto en Unity 6) y las acciones
//  del asset InputSystem_Actions (Player/Move, Player/Attack).
// =====================================================================

#define D01_HOLA_MUNDO          // Diap. 11
//#define D02_TIPOS_OPERADORES  // Diap. 12-13
//#define D03_ENUMS_STRUCTS     // Diap. 14
//#define D04_REFERENCIAS       // Diap. 15
//#define D05_VAR_CASTING       // Diap. 16
//#define D06_OOP               // Diap. 18-21
//#define D07_ARRAYS            // Diap. 22
//#define D08_LISTAS            // Diap. 23
//#define D09_DICCIONARIOS      // Diap. 24
//#define D10_CICLO_VIDA        // Diap. 41-46  (desmarca el script en el Inspector / pulsa X)
//#define D11_MOVIMIENTO        // Diap. 40, 47 (flechas / WASD)
//#define D12_BUSQUEDA          // Diap. 48
//#define D13_INPUTS            // Diap. 49     (Espacio)
//#define D14_INSTANCIAR        // Diap. 50     (clic izquierdo / Enter)

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;   // Input System nuevo

#pragma warning disable 0649, 0219, 0414   // silencia avisos de demos desactivadas

// =====================================================================
//  TIPOS DE APOYO: delegados, enums, structs, clases e interfaces
// =====================================================================

// --- Delegados (D02) ---
delegate void MyFirstDelegate(int param1, int param2);

// --- Enums y structs (D03) ---
enum Dia { Lun, Mar, Mie, Jue, Vie, Sab, Dom };
enum Dia2 : ulong { Lun = 1, Mar = 5, Mie, Jue, Vie, Sab, Dom };

public struct Coords
{
    public float x;
    public float y;
}

// --- Reference types (D04) ---
class Numero
{
    public int valor;
    public Numero(int valor) { this.valor = valor; }
}

// --- OOP (D04, D06, D08) ---
class Persona
{
    public string nombre;
    public int edad;

    public Persona(string nombre, int edad)
    {
        this.nombre = nombre;
        this.edad = edad;
    }

    public virtual string Saludar()
    {
        return "Hola, soy " + nombre;
    }
}

class Trabajador : Persona
{
    public string puesto;

    public Trabajador(string nombre, int edad, string puesto) : base(nombre, edad)
    {
        this.puesto = puesto;
    }

    public override string Saludar()
    {
        return base.Saludar() + ", y mi puesto es " + puesto;
    }
}

public abstract class Employee
{
    public string Name { get; set; }
    public double Salary { get; set; }
    public abstract void IncreaseSalary(double rate);
}

public class SalesRepresentative : Employee
{
    public double AnnualSalesAmount { get; set; }

    public override void IncreaseSalary(double rate)
    {
        if (this.AnnualSalesAmount > 100000)
            this.Salary += (this.Salary * (rate + 5) / 100);
        else
            this.Salary += (this.Salary * rate / 100);
    }
}

interface ISaludador { string Saludar(); }

class Entrenador : ISaludador
{
    public string nombre;
    public virtual string Saludar()
    {
        return "Soy " + this.nombre + " y te reto a un combate!";
    }
}

class Pokemon : ISaludador
{
    public string nombre;
    public string Saludar()   // en la diapo pone override: aquí no hay nada que sobreescribir
    {
        return this.nombre;
    }
}

// =====================================================================
//  EL SCRIPT (MonoBehaviour)
// =====================================================================
public class DemoClase : MonoBehaviour
{
#if D11_MOVIMIENTO
    // Diap. 40: una variable pública aparece en el Inspector -> cámbiala en Play
    public float movementSpeed = 5f;
    private InputAction moveAction;
#endif
#if D12_BUSQUEDA
    public GameObject enemy;
    public GameObject respawn;
#endif
#if D14_INSTANCIAR
    public GameObject myPrefab;   // si lo dejas vacío, se crea un cubo solo
    private InputAction attackAction;
#endif

    // =================================================================
    void Awake()
    {
#if D10_CICLO_VIDA
        Debug.Log("1. Awake");
#endif
#if D11_MOVIMIENTO
        // Si el objeto no se ve (GameObject vacío), le ponemos un cubo
        if (GetComponent<Renderer>() == null)
        {
            var cubo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubo.transform.SetParent(transform, false);
        }
#endif
    }

    void OnEnable()
    {
#if D10_CICLO_VIDA
        Debug.Log("2. OnEnable");
#endif
    }

    // =================================================================
    void Start()
    {
#if D10_CICLO_VIDA
        Debug.Log("3. Start");
#endif

#if D01_HOLA_MUNDO
        Titulo("D01 · Hola Mundo");
        System.Console.WriteLine("Hello World!"); // En Unity esto NO sale en la Console...
        Debug.Log("Hello World!");                // ...en Unity se usa Debug.Log
        Debug.LogWarning("Esto es un warning");
        Debug.LogError("Esto es un error (tranquilos, es de mentira)");
#endif

#if D02_TIPOS_OPERADORES
        Titulo("D02 · Tipos de datos");
        int entero = 7;
        float real = 3.5f;          // ojo a la f
        double doble = 3.5;
        bool verdad = true;
        char letra = 'A';
        Debug.Log($"int {entero} | float {real} | double {doble} | bool {verdad} | char {letra}");

        Vector3 pos = new Vector3(1, 2, 3);       // struct (value type)
        Quaternion rot = Quaternion.identity;     // struct (value type)
        Debug.Log($"Vector3 {pos} | Quaternion {rot}");

        MyFirstDelegate miDelegado = Sumar;       // delegado = referencia a un método
        miDelegado(2, 3);

        Titulo("D02 · Operadores");
        int i = 5;
        Debug.Log($"Aritméticos: 5/2={5 / 2}  5%2={5 % 2}  i++={i++} (ahora i={i})  ++i={++i}");
        Debug.Log($"Relacionales: 3==3 {3 == 3} | 3!=3 {3 != 3} | 2<5 {2 < 5}");
        Debug.Log($"Lógicos: true&&false {true && false} | true||false {true || false} | !true {!true}");
        Debug.Log($"Bitwise: 5&3={5 & 3}  5|3={5 | 3}  5^3={5 ^ 3}  1<<3={1 << 3}  16>>2={16 >> 2}");
        int acc = 10; acc += 5; acc *= 2;
        Debug.Log($"Asignación: 10 += 5, *= 2 -> {acc}");
        object caja = "soy un string";
        Debug.Log($"Misc: sizeof(int)={sizeof(int)} | typeof(float)={typeof(float)} | caja is string={caja is string} | caja as string={caja as string}");
        Debug.Log($"Prioridad: 2 + 3 * 4 = {2 + 3 * 4}  |  (2 + 3) * 4 = {(2 + 3) * 4}");
#endif

#if D03_ENUMS_STRUCTS
        Titulo("D03 · Enums");
        Dia hoy = Dia.Mie;
        Debug.Log($"Hoy es {hoy}, valor interno {(int)hoy}");
        Debug.Log($"Dia2.Lun={(ulong)Dia2.Lun}  Dia2.Mar={(ulong)Dia2.Mar}  Dia2.Mie={(ulong)Dia2.Mie}");

        Titulo("D03 · Structs (se copian por VALOR)");
        Coords p1 = new Coords();
        p1.x = 0;
        p1.y = 5;
        Coords p2 = p1;   // copia
        p2.x = 99;
        Debug.Log($"p1=({p1.x},{p1.y})  p2=({p2.x},{p2.y})  <- p1 no cambia");
#endif

#if D04_REFERENCIAS
        Titulo("D04 · Reference types");
        Numero a = new Numero(1);
        Numero b = a;
        Debug.Log($"A: {a.valor}, B: {b.valor}"); // A: 1, B: 1
        a.valor = 5;
        Debug.Log($"A: {a.valor}, B: {b.valor}"); // A: 5, B: 5  <- ¡comparten dato!

        Persona ana = new Persona("Ana", 0);
        Persona juan = null;
        int edadAna = ana.edad; // 0
        Debug.Log($"Edad Ana: {edadAna}");
        try
        {
            int edadJuan = juan.edad; // NullReferenceException
            Debug.Log(edadJuan);
        }
        catch (System.NullReferenceException e)
        {
            Debug.LogException(e);    // sale en rojo, pero la demo sigue
        }
#endif

#if D05_VAR_CASTING
        Titulo("D05 · var y conversiones");
        var implicita = 10;   // Implicitly typed
        int explicita = 10;   // Explicitly typed
        Debug.Log($"var -> {implicita.GetType()} | int -> {explicita.GetType()}");

        float f = 0;          // el 0 es int, pero se convierte implícitamente: f es float
        Debug.Log($"float f = 0 -> {f.GetType()}");

        int casteado = (int)15.5;   // casting explícito: se pierden decimales
        Debug.Log($"(int)15.5 = {casteado}");

        var result1 = 5 / 2;        // int / int
        var result2 = 0.5f / 2;     // float / int -> float
        Debug.Log($"5/2 = {result1} ({result1.GetType()})  |  0.5f/2 = {result2} ({result2.GetType()})");
#endif

#if D06_OOP
        Titulo("D06 · Herencia y sobreescritura");
        Persona persona = new Persona("Alba", 22);
        Trabajador trabajador = new Trabajador("Luis", 40, "programador");
        Debug.Log(persona.Saludar());
        Debug.Log(trabajador.Saludar());   // override + base.Saludar()

        Persona polimorfica = trabajador;  // un Trabajador ES una Persona
        Debug.Log($"Como Persona: {polimorfica.Saludar()}");

        Titulo("D06 · Abstracción");
        // Employee e = new Employee();    // <- descomenta: error, es abstracta
        SalesRepresentative rep = new SalesRepresentative { Name = "Marta", Salary = 1000, AnnualSalesAmount = 150000 };
        rep.IncreaseSalary(10);
        Debug.Log($"{rep.Name} ahora cobra {rep.Salary}");

        Titulo("D06 · Interfaces");
        List<ISaludador> saludadores = new List<ISaludador>
        {
            new Entrenador { nombre = "Ash" },
            new Pokemon { nombre = "¡Pikachu!" }
        };
        foreach (ISaludador s in saludadores)
            Debug.Log(s.Saludar());
#endif

#if D07_ARRAYS
        Titulo("D07 · Arrays unidimensionales");
        string[] cars = { "Volvo", "BMW", "Ford", "Mazda" };
        for (int c = 0; c < cars.Length; c++)
            Debug.Log(cars[c]);

        Titulo("D07 · Multidimensionales");
        var palabras = new string[3, 5];   // matriz 3x5
        string[,] masPalabras = { { "hola", "hello" },
                                  { "adiós", "bye" } }; // 2x2
        var hola = masPalabras[0, 0];
        Debug.Log($"palabras: {palabras.GetLength(0)}x{palabras.GetLength(1)} | masPalabras[0,0] = {hola} | [1,1] = {masPalabras[1, 1]}");

        Titulo("D07 · Escalonadas (jagged)");
        int[][] jaggedArray = new int[][]
        {
            new int[] { 1, 3, 5, 7, 9 },
            new int[] { 0, 2, 4, 6 },
            new int[] { 11, 22 }
        };
        for (int fila = 0; fila < jaggedArray.Length; fila++)
            Debug.Log($"Fila {fila} ({jaggedArray[fila].Length} elem.): {string.Join(", ", jaggedArray[fila])}");
#endif

#if D08_LISTAS
        Titulo("D08 · Listas");
        List<Persona> personas = new List<Persona>(); // lista vacía
        var alba = new Persona("Alba", 22);

        personas.Add(alba);                   // al final
        personas.Add(new Persona("Ana", 23));

        var anaLista = personas[1];
        Debug.Log($"personas[1] = {anaLista.nombre} | Count = {personas.Count}"); // 2

        foreach (var p in personas)
            Debug.Log($"Nombre: {p.nombre}, edad: {p.edad}");

        personas.Remove(alba);                // elimina a Alba
        Debug.Log($"Tras Remove: {personas.Count}");   // 1

        personas.RemoveAt(0);                 // elimina el primero (Ana)
        Debug.Log($"Tras RemoveAt: {personas.Count}"); // 0
#endif

#if D09_DICCIONARIOS
        Titulo("D09 · Diccionarios");
        Dictionary<string, int> numeros = new Dictionary<string, int>();

        numeros["cero"] = 0;
        numeros["siete"] = 7;
        numeros.Add("cinco", 5);
        numeros.Add("ocho", 8);

        Debug.Log($"Count = {numeros.Count}"); // 4
        // Debug.Log(numeros.Capacity);        // no existe en la versión de .NET de Unity

        foreach (var valor in numeros.Values)
            Debug.Log(valor);

        foreach (var par in numeros)
            Debug.Log($"{par.Key} -> {par.Value}");

        numeros["siete"] = 777;                 // con [] se sobreescribe
        Debug.Log($"siete = {numeros["siete"]}");
        try
        {
            numeros.Add("cero", 1);             // con Add, clave repetida = error
        }
        catch (System.ArgumentException e)
        {
            Debug.LogException(e);
        }
#endif

#if D12_BUSQUEDA
        Titulo("D12 · Búsqueda de GameObjects");
        // --- Preparación (normalmente esto ya estaría montado en la escena) ---
        var enemigoEscena = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        enemigoEscena.name = "Enemy";
        enemigoEscena.transform.position = new Vector3(-2, 0, 0);
        var puntoRespawn = new GameObject("PuntoRespawn");
        puntoRespawn.tag = "Respawn";   // tag que Unity trae de serie
        puntoRespawn.transform.position = new Vector3(2, 0, 0);
        // --- Código de la diapositiva ---
        if (respawn == null)
            respawn = GameObject.FindWithTag("Respawn");

        enemy = GameObject.Find("Enemy");
        Renderer enemyRenderer = enemy.GetComponent<Renderer>(); // diapo: GetComponent<State>()
        enemyRenderer.material.color = Color.red;                // diapo: setHealth(100)

        Instantiate(enemy, respawn.transform.position,
            respawn.transform.rotation);
        Debug.Log("Enemigo pintado de rojo y clonado en el punto de respawn");
#endif

#if D14_INSTANCIAR
        if (myPrefab == null)
        {
            myPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            myPrefab.name = "PrefabDemo";
            myPrefab.SetActive(false);   // plantilla oculta
        }
        attackAction = BuscarAccion("Attack");   // Player/Attack: clic izq., Enter, mando
        Debug.Log("D14 · Haz clic en la vista Game (o Enter) para instanciar");
#endif

#if D11_MOVIMIENTO
        moveAction = BuscarAccion("Move");       // Player/Move: WASD, flechas, stick
        Debug.Log("D11 · Mueve el objeto con flechas / WASD. Cambia movementSpeed en el Inspector.");
#endif
#if D13_INPUTS
        Debug.Log("D13 · Pulsa, mantén y suelta Espacio");
#endif
    }

    // =================================================================
    void Update()
    {
#if D10_CICLO_VIDA
        if (Time.frameCount <= 3)
            Debug.Log($"4. Update (frame {Time.frameCount})");
        if (Keyboard.current.xKey.wasPressedThisFrame)
            Destroy(gameObject);   // -> OnDisable + OnDestroy
#endif

#if D11_MOVIMIENTO
        Vector2 input = moveAction.ReadValue<Vector2>();   // x = horizontal, y = vertical

        // update the position
        transform.position = transform.position + new Vector3(
            input.x * movementSpeed * Time.deltaTime,
            input.y * movementSpeed * Time.deltaTime,
            0);

        // output to log the position change
        if (input != Vector2.zero)
            Debug.Log(transform.position);
#endif

#if D13_INPUTS
        // Leyendo el dispositivo directamente (lo más parecido al Input antiguo)
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("He pulsado espacio");
        }
        if (Keyboard.current.spaceKey.isPressed)
        {
            Debug.Log("Estoy pulsando espacio");
        }
        if (Keyboard.current.spaceKey.wasReleasedThisFrame)
        {
            Debug.Log("He dejado de pulsar espacio");
        }
        // Alternativa con acciones (Player/Jump, que incluye Espacio y mando):
        //   var jump = InputSystem.actions.FindAction("Jump");
        //   jump.WasPressedThisFrame() / jump.IsPressed() / jump.WasReleasedThisFrame()
#endif

#if D14_INSTANCIAR
        if (attackAction.WasPressedThisFrame())
        {
            var clone = Instantiate(myPrefab,
                new Vector3(Random.Range(-3f, 3f), 3, 0), Quaternion.identity); // diapo: Vector3(0,0,0)
            clone.SetActive(true);
            clone.AddComponent<Rigidbody>();   // ahora tiene físicas: cae
            Destroy(clone, 3.0f);              // desaparece a los 3 s
        }
#endif
    }

#if D10_CICLO_VIDA
    int fixedCount = 0;
    void FixedUpdate()
    {
        if (fixedCount++ < 5)
            Debug.Log($"5. FixedUpdate (frame {Time.frameCount})");
    }

    void LateUpdate()
    {
        if (Time.frameCount <= 3)
            Debug.Log($"6. LateUpdate (frame {Time.frameCount})");
    }

    void OnDisable() { Debug.Log("7. OnDisable"); }
    void OnDestroy() { Debug.Log("8. OnDestroy"); }
#endif

    // =================================================================
    //  UTILIDADES (no hace falta tocarlas)
    // =================================================================
    void Titulo(string texto)
    {
        Debug.Log($"<b><color=cyan>===== {texto} =====</color></b>");
    }

    // Busca una acción en el asset de acciones del proyecto (Unity 6: InputSystem_Actions)
    InputAction BuscarAccion(string nombre)
    {
        InputAction accion = InputSystem.actions != null ? InputSystem.actions.FindAction(nombre) : null;
        if (accion == null)
            Debug.LogError($"No encuentro la acción '{nombre}'. Revisa Project Settings > Input System Package > Project-wide Actions.");
        return accion;
    }

    // Método al que apunta el delegado (D02)
    void Sumar(int x, int y) { Debug.Log($"Delegado: {x} + {y} = {x + y}"); }
}
