using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

/*PROYECTO FINAL - V1.0.2
         * Registro de versiones y actualizaciones
         * 
         * Versión: 1.0.2
         * Fecha: 2/2/2025
         * Hora: 20:56
         * Autor: J
         * 
         * He diseñado e implementado el primer escenario para el juego "laberinto" con temática espacial.
         * Además se han modificado y añadido métodos que funcionan acorde al nuevo escenario.
         * Las clases, miembros, variables (no auxiliares), arreglos, métodos y objetos importantes creados fueron:
         * -Clase
         *      EsPosicionValida
         *      MaximizarVentana (método opcional)
         * -Miembros
         *      -Variables
         *          filePath de tipo string
         *          xAnterior de tipo int
         *          yAnterior de tipo int
         *          celda de tipo string
         *      -Arreglos
         *          Zvezda de tipo string
         *          lines de tipo string
         * 
         * Versión: 1.0.1
         * Fecha: 26/1/2025
         * Hora: 15:49
         * Autor: J
         * 
         * He modularizado el código y corregido un error en el método ActualizarEscenario.
         * Las clases, miembros, variables (no auxiliares), arreglos, métodos y objetos creados fueron:
         * -Clase
         *      Menú
         * -Miembros
         *      -Variables
         *          Opcion de tipo var
         *      
         *      
         * Versión: 1.0.0
         * Fecha: 26/1/2025
         * Hora: 10:41
         * Autor: J
         * 
         * He programado lo básico para el primer juego "Laberinto" dentro de una clase; se han creado las variables y funciones fundamentales para el escenario y el movimiento del personaje.
         * Las clases, miembros, variables (no auxiliares), arreglos, métodos y objetos creados fueron:
         * -Clase
         *      Laberinto
         * -Miembros
         *      -Variables
         *          r de tipo Random
         *      -Arreglos
         *          Pantalla (Propiedad estática)
         *          CoordPersonaje (Propiedad estática)
         *      -Métodos
         *          PrepararEscenario (Método estático)
         *          PintarEscenario (Método estático)
         *          ActualizarEscenario (Método estático)
         *      -Objetos
         *          BotonPresionado (Objeto estático)
         *          
         */

namespace MINIJUEGOS_ASCII
{
    internal class Menú
    {
        public static void Main(string[] args)
        {
            // Limpia y restablece ajustes de la consola, antes de iniciar el Menú principal, ya que se podrá regresar aquí desde cualquier minijuego
            Console.Clear();
            Console.CursorVisible = true;
            Console.SetCursorPosition(0,0);

            Console.WriteLine("Menú:");
            Console.WriteLine("1. Laberinto");
            Console.WriteLine("2. Pidra, papel y tijeras");
            Console.WriteLine("3. El ahorcado");
            Console.WriteLine("4. Salir");
            Console.WriteLine("Seleccione una opción:");

            ////Info.de desarrollador
            //Console.SetBufferSize(120, 35);
            //Console.WriteLine($"Buffer: {Console.BufferWidth}x{Console.BufferHeight}");

            var Opcion = Console.ReadLine();

            switch (Opcion)
            {
                case "1":
                    Laberinto.Menu();
                    break;
                case "2":
                    PiedraPapelYTijeras.Iniciar();
                    break;
                case "3":
                    Ahorcado.Iniciar();
                    break;
                case "4":
                    Console.WriteLine("Saliendo...");
                    System.Threading.Thread.Sleep(3000); // Pausa de 5 segundos
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }
        //public static void MaximizarVentana() //Ajustar el tamaño de la ventana de la consola al máximo tamaño posible
        //{
        //    int maxColumns = Console.LargestWindowWidth;
        //    int maxRows = Console.LargestWindowHeight;

        //    Console.SetWindowSize(maxColumns, maxRows);
        //    Console.SetBufferSize(maxColumns, maxRows);
        //}
    }
    internal class Laberinto
    {
        //MIEMBROS FUNDAMENTALES PARA EL JUEGO
        //                                           X=100 Columnas y Y=35 filas
        public static int[] CoordPersonaje = new int[2]; //Un arreglo de dos posiciones para guardar las coordenadas X y Y de nuestro personaje
        public static ConsoleKeyInfo BotonPresionado = new ConsoleKeyInfo(); // Objeto al que se le pueden asignar valores de lo que se teclea
        public static string EscenarioActual = "Zvezda";

        //MIEMBROS NECESARIOS PARA LA FUNCIONALIDAD DE ENTRADA POR RATÓN
        public static CONSOLE_FONT_INFO consoleFontInfo;
        public static Rectangle consoleWindow; // Nos servirá después para obtener el rectángulo de la ventana de la consola

        // Escenarios
        public static string[,] Zvezda = new string[100, 35]; // Conecta directamente con "Zarya"
        public static string[,] Zarya = new string[100, 35];
        public static string[,] MLM = new string[100, 35]; // Conecta mediante ascensor con "Zvezda"

        public static void PrepararAmbienteConsola() // Forma parte de la funcionalidad de entrada por ratón
        {
            // Ajustes previos para asegurarnos que el minijuego se vea bien visualmente
            MaximizarVentana(); // Maximizar la ventana de la consola
            Console.Clear(); // Limpiar la pantalla de la consola
            Console.CursorVisible = false; // Ocultar el cursor de la consola

            Application.EnableVisualStyles(); // Habilitar estilos visuales para Windows Forms
            Application.SetCompatibleTextRenderingDefault(false); // Configurar renderizado de texto compatible

            // Ajustes previos para la funcionalidad encargada de leer la entrada por ratón

            IntPtr hConsole = GetStdHandle(-11); // Obtener el manejador de la consola de salida estánda

            if (!GetCurrentConsoleFont(hConsole, false, out consoleFontInfo)) // Obtener información de la fuente de la consola
            {
                Console.WriteLine("Error al obtener la información de la fuente de la consola.");
                return;
            }

            if (consoleFontInfo.dwFontSize.X <= 0 || consoleFontInfo.dwFontSize.Y <= 0) // Validar que los valores sean correctos
            {
                consoleFontInfo.dwFontSize = new Point(8, 16); // Asignar valores predeterminados comunes
            }

            consoleWindow = GetConsoleWindowRectangle(); // Obtener el rectángulo de la ventana de la consola
        }
        public static void CargarMenu() // Carga e imprime el menú del minijuego
        {
            // Cargar en memoria el menú gráfico
            string filePath = "MenuInicio.txt"; // Ruta del archivo de texto, el cual contiene el menú con gráficos ASCII
            string[] lines = File.ReadAllLines(filePath); // Leer todas las líneas del archivo
            string[,] escenario = new string[100, 35];

            // Llenar el arreglo bidimensional 'escenario' con los caracteres del archivo previamente guardados en el arreglo 'lines'
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    escenario[x, y] = lines[y][x].ToString();
                }
            }

            // Imprimir el arreglo que contiene nuestro menú gráfico
            for (int y = 0; y < 35; y++)
            {
                Console.SetCursorPosition(0, y);
                for (int x = 0; x < 100; x++)
                {
                    Console.Write(escenario[x, y]);
                }
            }
        }
        public static void Menu() // Punto de llamada de las funcionalidades para el menú del minijuego
        {
            PrepararAmbienteConsola();
            CargarMenu();

            string[] opciones = { "Iniciar", "Salir" }; // Opciones del menú

            while (true) // Bluce encargado de mantener ejecutando las funcionalidades para el menú hasta que se elija una de las dos opciones
            {
                Point posicionCursor = Cursor.Position; // Obtener la posición actual del cursor del ratón
                int consoleLeft = consoleWindow.Left + 8; // Ajuste por borde izquierdo
                int consoleTop = consoleWindow.Top + 30; // Ajuste por borde superior

                // Convertir coordenadas de pantalla a coordenadas de la consola
                Point posicionConsola = new Point(
                    (posicionCursor.X - consoleLeft) / consoleFontInfo.dwFontSize.X,
                    (posicionCursor.Y - consoleTop) / consoleFontInfo.dwFontSize.Y
                );

                Console.SetCursorPosition(0, 0);

                // Verificar si se presionó el botón izquierdo del mouse
                if (Control.MouseButtons == MouseButtons.Left)
                {
                    // Coordenadas del "botón" Iniciar
                    if (posicionConsola.X >= 55 && posicionConsola.X <= 79 && posicionConsola.Y >= 20 && posicionConsola.Y <= 24)
                    {
                        SendKeys.SendWait("{ESCAPE}"); // Simular la pulsación de la tecla Escape para no interrumpir la ejecución del programa 
                        Iniciar(); // Inicia el juego
                        break;
                    }

                    // Coordenadas del "botón" Salir
                    if (posicionConsola.X >= 57 && posicionConsola.X <= 77 && posicionConsola.Y >= 28 && posicionConsola.Y <= 32)
                    {
                        SendKeys.SendWait("{ESCAPE}");
                        string[] args = { }; // Crear un argumento vacío para args
                        Menú.Main(args);
                        break;
                    }
                }
            }
        }
        public static void Iniciar() // Método que "arranca" nuestro minijuego
        {
            // Ajustes previos para una mejor estética visual
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false; // Ocultamos el cursor para que no moleste

            // Carga en memoria los mapas para el minijuego

            PrepararEscenario("Zvezda.txt", Zvezda); // Ejecutamos el método PrepararEscenario para cargar nuestro "escenario" en los arreglos
            PrepararEscenario("Zarya.txt", Zarya);
            PrepararEscenario("MLM.txt", MLM);

            PintarEscenario(Zvezda, 49, 27); // Imprimimos por primera vez nuestro primer escenario "Zvezda"

            while (true) // Bluce encargado de mantener ejecutando el minijuego
            {
                if (Console.KeyAvailable)  // Comprobamos si se lee una tecla
                {
                    if (EscenarioActual == "Zvezda")
                    {
                        Movimiento(Zvezda);
                    }
                    if (EscenarioActual == "Zarya")
                    {
                        Movimiento(Zvezda);
                    }
                }
            }
        }
        public static void PrepararEscenario(string rutaEscenario, string[,] escenario)
        {
            string filePath = rutaEscenario; // Ruta del archivo de texto
            string[] lines = File.ReadAllLines(filePath); // Leer todas las líneas del archivo

            // Llenar el arreglo con los caracteres del archivo
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    escenario[x, y] = lines[y][x].ToString();
                }
            }
        }
        public static void PintarEscenario(string[,] escenario, int NuevaCoordXPersonaje, int NuevaCoordYPersonaje) // Imprime el escenario que recibe como parámetro de entrada y también modifica la posición del jugador
        {
            CoordPersonaje[0] = NuevaCoordXPersonaje; CoordPersonaje[1] = NuevaCoordYPersonaje; //Establece la posición de nuestro personaje

            Console.Clear();

            for (int y = 0; y < 35; y++)
            {
                Console.SetCursorPosition(0, y);
                for (int x = 0; x < 100; x++)
                {
                    Console.Write(escenario[x, y]);
                }
            }
            Console.SetCursorPosition(NuevaCoordXPersonaje, NuevaCoordYPersonaje);
            Console.Write("P");


        }
        public static void Movimiento(string[,] escenario)
        {
            BotonPresionado = Console.ReadKey(true); // true evita que se muestre la tecla presionada

            int xAnterior = CoordPersonaje[0];
            int yAnterior = CoordPersonaje[1]; //Guarda la posición actual antes de moverse

            if (BotonPresionado.Key == ConsoleKey.DownArrow && yAnterior < 34 && EsPosicionValida(escenario, new int[] { xAnterior, yAnterior + 1 }))
            {
                if (!SiguenteEscenario(escenario, new int[] { xAnterior, yAnterior + 1 }))
                {
                    CoordPersonaje[1]++;
                }
                else
                {
                    PintarEscenario(Zarya, 50, 30);
                    EscenarioActual = "Zarya";
                }

            }
            else if (BotonPresionado.Key == ConsoleKey.UpArrow && yAnterior > 0 && EsPosicionValida(escenario, new int[] { xAnterior, yAnterior - 1 }))
            {
                if (!SiguenteEscenario(escenario, new int[] { xAnterior, yAnterior - 1 }))
                {
                    CoordPersonaje[1]--;
                }
                else
                {
                    PintarEscenario(Zarya, 50, 34);
                    EscenarioActual = "Zarya";
                }
            }
            else if (BotonPresionado.Key == ConsoleKey.LeftArrow && xAnterior > 0 && EsPosicionValida(escenario, new int[] { xAnterior - 1, yAnterior }))
            {
                CoordPersonaje[0]--;
            }
            else if (BotonPresionado.Key == ConsoleKey.RightArrow && xAnterior < 99 && EsPosicionValida(escenario, new int[] { xAnterior + 1, yAnterior }))
            {
                CoordPersonaje[0]++;
            }

            ActualizarEscenario(escenario, xAnterior, yAnterior); //Enviamos la posición anterior para que se restaure bien
        }

        public static void ActualizarEscenario(string[,] escenario, int xAnterior, int yAnterior)
        {
            // Restaurar la celda en la posición anterior
            Console.SetCursorPosition(xAnterior, yAnterior);
            Console.Write(escenario[xAnterior, yAnterior]); // Escribimos lo que había en la celda antes de que el personaje estuviera allí

            // Dibujar el personaje en la nueva posición
            Console.SetCursorPosition(CoordPersonaje[0], CoordPersonaje[1]);
            Console.Write("P");
        }

        public static bool EsPosicionValida(string[,] escenario, int[] coordenadas)
        {
            string celda = escenario[coordenadas[0], coordenadas[1]];
            return celda != "#" && celda != "║" && celda != "(" && celda != ")";
        }
        public static bool SiguenteEscenario(string[,] escenario, int[] coordenadas)
        {
            string celda = escenario[coordenadas[0], coordenadas[1]];
            return celda == "S";
        }
        public static bool Ascensor(string[,] escenario, int[] coordenadas)
        {
            string celda = escenario[coordenadas[0], coordenadas[1]];
            return celda == "A" || celda == "S";
        }

        private static void MaximizarVentana()
        {
            IntPtr identificadorConsola = GetConsoleWindow(); // Obtener el identificador de la ventana de la consola
            ShowWindow(identificadorConsola, 3); // Maximizar la ventana de la consola

            /* ShowWindow(1): Muestra la ventana en su tamaño y posición originales.
             * ShowWindow(2): Muestra la ventana minimizada.
             * ShowWindow(3): Maximiza la ventana para que ocupe toda la pantalla
             */
        }


        // C O D I G O  D E L   F U N C I O N A M I E N T O   D E L   M O U S E

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(int nStdHandle); // Obtener el identificador del dispositivo estándar (consola)

        [DllImport("kernel32.dll")]
        private static extern bool GetCurrentConsoleFont(IntPtr identificadorConsolaOutput, bool bMaximumWindow, out CONSOLE_FONT_INFO lpConsoleCurrentFont); // Obtener información sobre la fuente de la consola

        [StructLayout(LayoutKind.Sequential)]
        public struct CONSOLE_FONT_INFO
        {
            public int nFont; // Identificador de la fuente
            public Point dwFontSize; // Tamaño de la fuente en la consola
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect); // Obtener el rectángulo que delimita la ventana de la consola

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow(); // Obtener el identificador de la ventana de la consola

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow); // Cambiar el estado de la ventana de la consola

        private static Rectangle GetConsoleWindowRectangle()
        {
            IntPtr identificadorConsola = GetConsoleWindow(); // Obtener el identificador de la ventana de la consola
            RECT rect;
            GetWindowRect(identificadorConsola, out rect); // Obtener las coordenadas de la ventana de la consola
            return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top); // Convertir la estructura RECT en un objeto Rectangle
        }
    }

        internal class PiedraPapelYTijeras
    {
        private static readonly string[] opciones = { "Piedra", "Papel", "Tijeras" }; // Opciones del juego
        private static Random random = new Random(); // Generador de números aleatorios

        public static void Iniciar()
        {
            MaximizarVentana(); // Maximizar la ventana de la consola
            Console.Clear(); // Limpiar la pantalla de la consola
            Console.CursorVisible = false; // Ocultar el cursor de la consola

            Application.EnableVisualStyles(); // Habilitar estilos visuales para Windows Forms
            Application.SetCompatibleTextRenderingDefault(false); // Configurar renderizado de texto compatible

            IntPtr hConsole = GetStdHandle(-11); // Obtener el manejador de la consola de salida estándar
            CONSOLE_FONT_INFO consoleFontInfo;

            if (!GetCurrentConsoleFont(hConsole, false, out consoleFontInfo)) // Obtener información de la fuente de la consola
            {
                Console.WriteLine("Error al obtener la información de la fuente de la consola.");
                return;
            }

            if (consoleFontInfo.dwFontSize.X <= 0 || consoleFontInfo.dwFontSize.Y <= 0) // Validar que los valores sean correctos
            {
                consoleFontInfo.dwFontSize = new Point(8, 16); // Asignar valores predeterminados comunes
            }

            Rectangle consoleWindow = GetConsoleWindowRectangle(); // Obtener el rectángulo de la ventana de la consola

            while (true)
            {
                Point posicionCursor = Cursor.Position; // Obtener la posición actual del cursor del ratón
                int consoleLeft = consoleWindow.Left + 8; // Ajuste por borde izquierdo
                int consoleTop = consoleWindow.Top + 30; // Ajuste por borde superior

                // Convertir coordenadas de pantalla a coordenadas de la consola
                Point posicionConsola = new Point(
                    (posicionCursor.X - consoleLeft) / consoleFontInfo.dwFontSize.X,
                    (posicionCursor.Y - consoleTop) / consoleFontInfo.dwFontSize.Y
                );

                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Seleccione una opción pasando el cursor sobre ella:");

                // Dibujar opciones en consola con sus representaciones en ASCII
                for (int i = 0; i < opciones.Length; i++)
                {
                    Console.SetCursorPosition(10, 5 + i * 5);
                    Console.WriteLine(opciones[i]);
                    DibujarOpcion(opciones[i], 10, 6 + i * 5);
                }

                for (int i = 0; i < opciones.Length; i++)
                {
                    if (posicionConsola.X >= 10 && posicionConsola.X <= 20 && posicionConsola.Y == 5 + i * 5)
                    {
                        Jugar(opciones[i]);
                        return;
                    }
                }
            }
        }

        private static void Jugar(string eleccionUsuario)
        {
            string eleccionComputadora = opciones[random.Next(3)]; // Selección aleatoria de la computadora
            Console.Clear();
            Console.WriteLine($"Tú elegiste: {eleccionUsuario}");
            Console.WriteLine($"La computadora eligió: {eleccionComputadora}");
            DibujarOpcion(eleccionUsuario, 5, 5);
            DibujarOpcion(eleccionComputadora, 25, 5);

            if (eleccionUsuario == eleccionComputadora)
            {
                Console.WriteLine("¡Es un empate!");
            }
            else if ((eleccionUsuario == "Piedra" && eleccionComputadora == "Tijeras") ||
                     (eleccionUsuario == "Papel" && eleccionComputadora == "Piedra") ||
                     (eleccionUsuario == "Tijeras" && eleccionComputadora == "Papel"))
            {
                Console.WriteLine("¡Ganaste!");
            }
            else
            {
                Console.WriteLine("Perdiste. Inténtalo de nuevo.");
            }

            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }

        private static void DibujarOpcion(string opcion, int x, int y)
        {
            Console.SetCursorPosition(x, y);
            switch (opcion)
            {
                case "Piedra":
                    Console.WriteLine("   ███   ");
                    Console.SetCursorPosition(x, y + 1);
                    Console.WriteLine(" ███████ ");
                    Console.SetCursorPosition(x, y + 2);
                    Console.WriteLine(" ███████ ");
                    Console.SetCursorPosition(x, y + 3);
                    Console.WriteLine("   ███   ");
                    break;
                case "Papel":
                    Console.WriteLine(" ███████ ");
                    Console.SetCursorPosition(x, y + 1);
                    Console.WriteLine(" ███████ ");
                    Console.SetCursorPosition(x, y + 2);
                    Console.WriteLine(" ███████ ");
                    Console.SetCursorPosition(x, y + 3);
                    Console.WriteLine(" ███████ ");
                    break;
                case "Tijeras":
                    Console.WriteLine("    █  █  ");
                    Console.SetCursorPosition(x, y + 1);
                    Console.WriteLine("     ███  ");
                    Console.SetCursorPosition(x, y + 2);
                    Console.WriteLine("      █   ");
                    Console.SetCursorPosition(x, y + 3);
                    Console.WriteLine(" ██████  ");
                    break;
            }
        }

        private static void MaximizarVentana()
        {
            IntPtr hConsole = GetConsoleWindow(); // Obtener el identificador de la ventana de la consola
            ShowWindow(hConsole, SW_MAXIMIZE); // Maximizar la ventana de la consola
        }

        // Importaciones de funciones de la API de Windows para manipular la consola
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(int nStdHandle); // Obtener el identificador del dispositivo estándar (consola)

        [DllImport("kernel32.dll")]
        private static extern bool GetCurrentConsoleFont(IntPtr hConsoleOutput, bool bMaximumWindow, out CONSOLE_FONT_INFO lpConsoleCurrentFont); // Obtener información sobre la fuente de la consola

        [StructLayout(LayoutKind.Sequential)]
        private struct CONSOLE_FONT_INFO
        {
            public int nFont; // Identificador de la fuente
            public Point dwFontSize; // Tamaño de la fuente en la consola
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect); // Obtener el rectángulo que delimita la ventana de la consola

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow(); // Obtener el identificador de la ventana de la consola

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow); // Cambiar el estado de la ventana de la consola

        private const int SW_MAXIMIZE = 3; // Constante para maximizar la ventana

        private static Rectangle GetConsoleWindowRectangle()
        {
            IntPtr hConsole = GetConsoleWindow(); // Obtener el identificador de la ventana de la consola
            RECT rect;
            GetWindowRect(hConsole, out rect); // Obtener las coordenadas de la ventana de la consola
            return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top); // Convertir la estructura RECT en un objeto Rectangle
        }
    }

    internal class Ahorcado
    {
        public static void Iniciar()
        {
            Console.Clear(); // Limpiar la pantalla
            Console.SetCursorPosition(0, 0); // Asegurar que el cursor esté en la posición inicial

            // Lista de palabras y sus pistas
            Dictionary<string, string> palabras = new Dictionary<string, string>
        {
            { "compilador", "Traduce código fuente a código máquina." },
            { "algoritmo", "Conjunto de pasos para resolver un problema." },
            { "variable", "Espacio en memoria que almacena un valor." },
            { "funcion", "Bloque de código reutilizable." },
            { "recursividad", "Cuando una función se llama a sí misma." },
            { "programacion", "Proceso de escribir código para computadoras." },
            { "depuracion", "Proceso de encontrar y corregir errores." },
            { "framework", "Conjunto de herramientas para desarrollo." },
            { "matriz", "Estructura de datos en filas y columnas." },
            { "puntero", "Variable que almacena direcciones de memoria." },
            { "interfaz", "Define métodos sin implementarlos." },
            { "herencia", "Mecanismo de reutilización en POO." },
            { "clase", "Plantilla para crear objetos en POO." }
        };

            Random aleatorio = new Random(); // Generador de números aleatorios
                                             // Seleccionar una palabra aleatoria del diccionario
            KeyValuePair<string, string> palabraSeleccionada = palabras.ElementAt(aleatorio.Next(palabras.Count));
            string palabraSecreta = palabraSeleccionada.Key; // La palabra a adivinar
            string pista = palabraSeleccionada.Value; // La pista de la palabra
            char[] palabraAdivinada = new string('_', palabraSecreta.Length).ToCharArray(); // Representación oculta de la palabra
            HashSet<char> letrasIncorrectas = new HashSet<char>(); // Conjunto de letras erróneas ingresadas
            int intentosRestantes = 7; // Número de intentos disponibles

            // Bucle del juego
            while (intentosRestantes > 0 && new string(palabraAdivinada) != palabraSecreta)
            {
                Console.Clear();
                DibujarAhorcado(7 - intentosRestantes); // Dibujar el estado actual del ahorcado
                Console.WriteLine("Pista: " + pista);
                Console.WriteLine("Palabra: " + new string(palabraAdivinada));
                Console.WriteLine("Letras incorrectas: " + string.Join(", ", letrasIncorrectas));
                Console.Write("Ingresa una letra: ");
                char intento = Console.ReadKey().KeyChar; // Leer la letra ingresada por el usuario

                // Verificar si la letra está en la palabra secreta
                if (palabraSecreta.Contains(intento))
                {
                    for (int i = 0; i < palabraSecreta.Length; i++)
                    {
                        if (palabraSecreta[i] == intento)
                        {
                            palabraAdivinada[i] = intento; // Reemplazar el guion bajo con la letra adivinada
                        }
                    }
                }
                else
                {
                    letrasIncorrectas.Add(intento); // Agregar la letra al conjunto de errores
                    intentosRestantes--; // Reducir el número de intentos
                }
            }

            Console.Clear();
            DibujarAhorcado(7 - intentosRestantes);
            // Verificar si el usuario ha ganado o perdido
            if (new string(palabraAdivinada) == palabraSecreta)
            {
                Console.WriteLine("\n¡Felicidades! Has adivinado la palabra: " + palabraSecreta);
            }
            else
            {
                Console.WriteLine("\nPerdiste. La palabra era: " + palabraSecreta);
            }
        }

        static void DibujarAhorcado(int errores)
        {
            // Representación gráfica del ahorcado en distintas etapas
            string[] dibujo = new string[]
            {
            "  +---+",
            "  |   |",
            "      |",
            "      |",
            "      |",
            "      |",
            "========="
            };

            // Modificar el dibujo según la cantidad de errores
            switch (errores)
            {
                case 1:
                    dibujo[2] = "  O   |";
                    break;
                case 2:
                    dibujo[2] = "  O   |";
                    dibujo[3] = "  |   |";
                    break;
                case 3:
                    dibujo[2] = "  O   |";
                    dibujo[3] = " /|   |";
                    break;
                case 4:
                    dibujo[2] = "  O   |";
                    dibujo[3] = " /|\\  |";
                    break;
                case 5:
                    dibujo[2] = "  O   |";
                    dibujo[3] = " /|\\  |";
                    dibujo[4] = " /    |";
                    break;
                case 6:
                    dibujo[2] = "  O   |";
                    dibujo[3] = " /|\\  |";
                    dibujo[4] = " / \\  |";
                    break;
                case 7:
                    dibujo[2] = "  O   |";
                    dibujo[3] = " /|\\  |";
                    dibujo[4] = " / \\  |";
                    dibujo[5] = "¡AHORCADO!";
                    break;
            }

            // Imprimir el dibujo del ahorcado
            foreach (var linea in dibujo)
            {
                Console.WriteLine(linea);
            }
        }
    }

}