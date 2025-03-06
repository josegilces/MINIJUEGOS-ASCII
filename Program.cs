using NAudio.Wave; // Importa el espacio de nombres NAudio.Wave para manejar operaciones de audio
using System; // Importa el espacio de nombres System para utilizar las funcionalidades básicas de C#
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks; // Importa el espacio de nombres System.Threading.Tasks, que proporciona tipos y métodos para manejar operaciones asíncronas y paralelas
using System.Windows.Forms;

/*PROYECTO FINAL - V1.0.2
         * Registro de versiones y actualizaciones
         * 
         * Versión: 1.0.2
         * Fecha: 2/2/2025
         * Hora: 20:56
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
            Console.SetCursorPosition(0, 0);

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
                    Laberinto.MenuPrincipal();
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
        // MIEMBROS FUNDAMENTALES PARA EL JUEGO
        //                                           X=100 Columnas y Y=35 filas
        public static byte[] CoordPersonaje = new byte[2]; //Un arreglo de dos posiciones para guardar las coordenadas X y Y de nuestro personaje
        public static ConsoleKeyInfo BotonPresionado = new ConsoleKeyInfo(); // Objeto al que se le pueden asignar valores de lo que se teclea
        public static string EscenarioActual = "Zvezda";

        // MIEMBROS NECESARIOS PARA LA FUNCIONALIDAD DE ENTRADA POR RATÓN
        public static CONSOLE_FONT_INFO consoleFontInfo;
        public static Rectangle consoleWindow; // Nos servirá después para obtener el rectángulo de la ventana de la consola

        //MIEMBROS NECESARIOS PARA LA FUNCIONALIDAD DE MÚSICA
        private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        // Escenarios
        public static string[,] Zvezda = new string[100, 35]; // Conecta directamente con Zarya
        public static string[,] Zarya = new string[100, 35]; // Conecta directamente con Zvezda y por ascensor con Nauka
        public static string[,] Nauka = new string[100, 35]; // Conecta mediante ascensor con Zarya

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
        public static void CargarMenus(string ruta) // Carga e imprime un menú del minijuego indicado en su parámetro de entrada
        {
            Console.Clear();


            // Obtener la ruta base del ejecutable
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            // Combinar la ruta base con la ruta relativa del archivo
            string filePath = Path.Combine(basePath, ruta); // Ruta del archivo de texto, el cual contiene el menú con gráficos ASCII

            // Verificar si el archivo existe
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: No se encontró el archivo en la ruta: {filePath}");
                return;
            }

            // Cargar en memoria el menú gráfico
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

        public static void Musica(CancellationToken token) // Carga y reproduce la música
        {
            try
            {
                while (!token.IsCancellationRequested) // // Bucle infinito para la reproducción continua, se detiene si se solicita la cancelación
                {
                    // Utiliza un bloque using para asegurar que los recursos se liberen correctamente después de usarlos
                    using (var audioFile = new AudioFileReader("assets/Sounds/Interstellar.mp3")) // Crea un objeto AudioFileReader para leer el archivo de audio especificado
                    using (var outputDevice = new WaveOutEvent()) // Crea un dispositivo de salida de audio para reproducir el archivo de audio
                    {
                        outputDevice.Init(audioFile); // Inicializa el dispositivo de salida con el archivo de audio
                        outputDevice.Play(); // Comienza a reproducir el archivo de audio

                        //Console.ReadKey(); // Espera a que el usuario presione una tecla para detener la reproducción y terminar el programa

                        while (outputDevice.PlaybackState == PlaybackState.Playing && !token.IsCancellationRequested)
                        {
                            if (token.IsCancellationRequested)
                            {
                                outputDevice.Stop(); // Detiene la reproducción si se solicita la cancelación
                                outputDevice.Dispose(); // Liberar recursos
                                break; // Sal del bucle externo también
                            }
                            Task.Delay(5000).Wait(); // Mantiene el bucle mientras la música se esté reproduciendo
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la reproducción de música: {ex.Message}");
                throw;
            }

        }
        public static void MenuPrincipal() // Punto de llamada de las funcionalidades para el menú del minijuego
        {
            Debug.WriteLine("Método MenuPrincipal: OK");
            PrepararAmbienteConsola();
            CargarMenus("assets/Menus/MenuPrincipal.txt");

            // Reiniciar el CancellationTokenSource si ya fue cancelado
            if (cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Dispose(); // Liberar recursos del antiguo
                cancellationTokenSource = new CancellationTokenSource(); // Crear uno nuevo
            }


            // Iniciar la tarea de música con el CancellationToken
            var musicaTask = Task.Run(() => Musica(cancellationTokenSource.Token)); // Usa Task para ejecutar la música en segundo plano

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
                        MenuInstrucciones(); // Inicia el Menu de Instrucciones
                        break;
                    }

                    // Coordenadas del "botón" Salir
                    if (posicionConsola.X >= 57 && posicionConsola.X <= 77 && posicionConsola.Y >= 28 && posicionConsola.Y <= 32)
                    {
                        SendKeys.SendWait("{ESCAPE}");
                        string[] args = { }; // Crear un argumento vacío para args


                        // Detener la música al salir
                        cancellationTokenSource.Cancel(); // Solicita la cancelación de la tarea de música
                        musicaTask.Wait(); // Espera a que la tarea de música termine
                        Menú.Main(args);
                        break;
                    }
                }
            }
        }

        public static void MenuInstrucciones() // Punto de llamada de las funcionalidades para el menú del minijuego
        {
            CargarMenus("assets/Menus/MenuInstrucciones.txt");

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

                //Console.SetCursorPosition(0, 0);

                // Verificar si se presionó el botón izquierdo del mouse
                if (Control.MouseButtons == MouseButtons.Left)
                {
                    // Coordenadas del "botón" Jugar
                    if (posicionConsola.X >= 77 && posicionConsola.X <= 93 && posicionConsola.Y >= 28 && posicionConsola.Y <= 32)
                    {
                        SendKeys.SendWait("{ESCAPE}"); // Simular la pulsación de la tecla Escape para no interrumpir la ejecución del programa 
                        Iniciar(); // Inicia el juego
                        break;
                    }
                }
            }
        }
        public static void Iniciar() // Método que "arranca" nuestro minijuego
        {
            // Ajustes previos en la consola
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false; // Ocultamos el cursor de la consola

            // Carga en memoria los mapas para el minijuego

            PrepararEscenario("assets/Mapas/Zvezda.txt", Zvezda); // Ejecuta el método PrepararEscenario para cargar los escenarios en los arreglos
            PrepararEscenario("assets/Mapas/Zarya.txt", Zarya);
            PrepararEscenario("assets/Mapas/Nauka.txt", Nauka);

            PintarEscenario(Zvezda, 50, 28); // Imprimimos por primera vez nuestro primer escenario Zvezda

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
                        Movimiento(Zarya);
                    }
                    if (EscenarioActual == "Nauka")
                    {
                        Movimiento(Nauka);
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
        public static void PintarEscenario(string[,] escenario, byte NuevaCoordXPersonaje, byte NuevaCoordYPersonaje) // Imprime el escenario que recibe como parámetro de entrada y también modifica la posición del jugador
        {
            CoordPersonaje[0] = (byte)(NuevaCoordXPersonaje - 1);
            CoordPersonaje[1] = (byte)(NuevaCoordYPersonaje - 1); // Establece la posición de nuestro personaje y se le resta -1 porque las coords comienzan desde (0,0)

            Console.Clear();

            for (int y = 0; y < 35; y++)
            {
                Console.SetCursorPosition(0, y);
                for (int x = 0; x < 100; x++)
                {
                    Console.Write(escenario[x, y]);
                }
            }
            Console.SetCursorPosition(CoordPersonaje[0], CoordPersonaje[1]);
            Console.Write("O");
        }
        public static void Movimiento(string[,] escenario) // Desplaza el personaje por el mapa, pero solo después de verificar que es posible
        {
            //h
            BotonPresionado = Console.ReadKey(true); // true evita que se muestre la tecla presionada

            byte ActualCoordXPersonaje = CoordPersonaje[0];
            byte ActualCoordYPersonaje = CoordPersonaje[1]; // Guarda la posición (x,y) actual antes de moverse

            if (BotonPresionado.Key == ConsoleKey.DownArrow && ActualCoordYPersonaje < 34 && EsPosicionValida(escenario, new int[] { ActualCoordXPersonaje, ActualCoordYPersonaje + 1 }))
            {
                if (!EventosEscenario(escenario, new int[] { ActualCoordXPersonaje, ActualCoordYPersonaje}))
                {
                    CoordPersonaje[1]++;
                }
            }
            else if (BotonPresionado.Key == ConsoleKey.UpArrow && ActualCoordYPersonaje > 0 && EsPosicionValida(escenario, new int[] { ActualCoordXPersonaje, ActualCoordYPersonaje - 1 }))
            {
                if (!EventosEscenario(escenario, new int[] { ActualCoordXPersonaje, ActualCoordYPersonaje}))
                {
                    CoordPersonaje[1]--;
                }
            }
            else if (BotonPresionado.Key == ConsoleKey.LeftArrow && ActualCoordXPersonaje > 0 && EsPosicionValida(escenario, new int[] { ActualCoordXPersonaje - 1, ActualCoordYPersonaje }))
            {
                if (!EventosEscenario(escenario, new int[] { ActualCoordXPersonaje, ActualCoordYPersonaje}))
                {
                    CoordPersonaje[0]--;
                }
            }
            else if (BotonPresionado.Key == ConsoleKey.RightArrow && ActualCoordXPersonaje < 99 && EsPosicionValida(escenario, new int[] { ActualCoordXPersonaje + 1, ActualCoordYPersonaje }))
            {
                if (!EventosEscenario(escenario, new int[] { ActualCoordXPersonaje, ActualCoordYPersonaje}))
                {
                    CoordPersonaje[0]++;
                }
            }
            else if (BotonPresionado.Key == ConsoleKey.W)
            {
                EventosEscenario(escenario, new int[] { ActualCoordXPersonaje, ActualCoordYPersonaje});
            }
            else if (BotonPresionado.Key == ConsoleKey.S)
            {
                EventosEscenario(escenario, new int[] { ActualCoordXPersonaje, ActualCoordYPersonaje });
            }

            ActualizarEscenario(escenario, ActualCoordXPersonaje, ActualCoordYPersonaje); // Enviamos la posición anterior para que se restaure bien
        }
        public static bool EsPosicionValida(string[,] escenario, int[] coordenadas) // Retorna un true si la celda a desplazarse no tiene colisión
        {
            string celda = escenario[coordenadas[0], coordenadas[1]];
            return celda != "#" && celda != "║" && celda != "(" && celda != ")" && celda != " ";
        }
        public static bool EventosEscenario(string[,] escenario, int[] coordenadas) // Retorna un true si la celda a desplazarse tiene un evento
        {
            string celda = escenario[coordenadas[0], coordenadas[1]]; // Celda a analizar
            bool evento = false; // Nos servirá para saber si ocurrió un evento

            if (celda == "S")
            {
                if (EscenarioActual == "Zvezda")
                {
                    PintarEscenario(Zarya, 50, 33);
                    EscenarioActual = "Zarya";
                    evento = true; // Efectivamente ocurrió un evento
                }
                else if (EscenarioActual == "Zarya")
                {
                    PintarEscenario(Zvezda, 50, 2);
                    EscenarioActual = "Zvezda";
                    evento = true;
                }
            }
            if (celda == "A") // Comprueba si el jugador está sobre una 'A' en el mapa
            {
                if (BotonPresionado.Key == ConsoleKey.S)
                {
                    PintarEscenario(Nauka, 50, 30);
                    EscenarioActual = "Nauka";
                    evento = true;
                }
                else if (BotonPresionado.Key == ConsoleKey.W)
                {
                    if (EscenarioActual == "Nauka")
                    {
                        PintarEscenario(Zarya, 50, 30);
                        EscenarioActual = "Zarya";
                        evento = true;
                    }
                }
            }
            return evento;
        }
        public static void ActualizarEscenario(string[,] escenario, int AnteriorCoordXPersonaje, int AnteriorCoordYPersonaje) // Vuelve a imprimir la celda en la que estaba el personaje e imprime el personaje en su nueva posición
        {
            Console.SetCursorPosition(AnteriorCoordXPersonaje, AnteriorCoordYPersonaje);
            Console.Write(escenario[AnteriorCoordXPersonaje, AnteriorCoordYPersonaje]); // Imprime lo que había en la celda antes de que el personaje estuviera allí

            // Dibujar el personaje en la nueva posición
            Console.SetCursorPosition(CoordPersonaje[0], CoordPersonaje[1]);
            Console.Write("O");
        }
        public static void Mision()
        {
            Task.Delay(100).Wait(); // Mantiene el bucle
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
            MostrarInstrucciones(); // Mostrar instrucciones del juego
            Console.Clear(); // Limpiar la pantalla de la consola

            while (true)
            {
                // Mostrar las opciones numeradas
                Console.Clear();
                Console.WriteLine("Seleccione una opción con el número correspondiente:");
                for (int i = 0; i < opciones.Length; i++)
                {
                    Console.SetCursorPosition(10, 5 + i * 5);
                    Console.WriteLine($"{i + 1}. {opciones[i]}");
                    DibujarOpcion(opciones[i], 10, 6 + i * 5);
                }

                // Leer la opción del usuario
                int eleccionUsuario;
                if (int.TryParse(Console.ReadLine(), out eleccionUsuario) && eleccionUsuario >= 1 && eleccionUsuario <= 3)
                {
                    Jugar(opciones[eleccionUsuario - 1]); // Jugar con la opción seleccionada
                }
                else
                {
                    Console.WriteLine("INGRESAR UNA OPCION CORRECTA.");
                    Console.ReadKey(); // Pausar antes de continuar el bucle
                    continue; // Volver al principio del ciclo si la opción es inválida
                }

                // Preguntar al usuario si desea jugar nuevamente o salir
                Console.Clear();
                Console.WriteLine("DESEAS SEGUIR");
                Console.WriteLine("1. Sí");
                Console.WriteLine("2. No");
                string respuesta = Console.ReadLine();

                if (respuesta == "2")
                {
                    Console.WriteLine("GRACIAS");
                    break; // Salir del bucle y terminar el juego
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

            Console.WriteLine("Presiona cualquier tecla para continuar...");
            Console.ReadKey();
        }

        private static void DibujarOpcion(string opcion, int x, int y)
        {
            Console.SetCursorPosition(x, y);


            switch (opcion)
            {
                case "Piedra":
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("     █████     ");
                    Console.SetCursorPosition(x, y + 1);
                    Console.WriteLine("    ███████    ");
                    Console.SetCursorPosition(x, y + 2);
                    Console.WriteLine("    ███████    ");
                    Console.SetCursorPosition(x, y + 3);
                    Console.WriteLine("     █████     ");
                    break;

                case "Papel":
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.WriteLine("   █████████   ");
                    Console.SetCursorPosition(x, y + 1);
                    Console.WriteLine("   █████████   ");
                    Console.SetCursorPosition(x, y + 2);
                    Console.WriteLine("   █████████   ");
                    Console.SetCursorPosition(x, y + 3);
                    Console.WriteLine("   █████████   ");
                    break;

                case "Tijeras":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine("     █   █     ");
                    Console.SetCursorPosition(x, y + 1);
                    Console.WriteLine("     ████      ");
                    Console.SetCursorPosition(x, y + 2);
                    Console.WriteLine("      █        ");
                    Console.SetCursorPosition(x, y + 3);
                    Console.WriteLine("     ██████    ");
                    break;
            }

            // Restaurar colores a los predeterminados después de dibujar
            Console.ResetColor();
        }

        private static void MostrarInstrucciones()
        {
            Console.WriteLine("Instrucciones:");
            Console.WriteLine("1. Elige una opción entre Piedra, Papel o Tijeras.");
            Console.WriteLine("2. Usa el número correspondiente para seleccionar la opción.");
            Console.WriteLine("3. Piedra vence a Tijeras.");
            Console.WriteLine("4. Tijeras vencen a Papel.");
            Console.WriteLine("5. Papel vence a Piedra.");
            Console.WriteLine("6. Si ambos eligen la misma opción, es un empate.");
            Console.WriteLine("Presiona cualquier tecla para continuar...");

            Console.ReadKey();
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
            Console.Clear();

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

            Random aleatorio = new Random();
            var palabraSeleccionada = palabras.ElementAt(aleatorio.Next(palabras.Count));
            string palabraSecreta = palabraSeleccionada.Key;
            string pista = palabraSeleccionada.Value;
            char[] palabraAdivinada = new string('_', palabraSecreta.Length).ToCharArray();
            HashSet<char> letrasIncorrectas = new HashSet<char>();
            int usosDeAyuda = 0;
            int intentosRestantes = 7;

            while (intentosRestantes > 0 && new string(palabraAdivinada) != palabraSecreta)
            {
                Console.Clear();
                DibujarAhorcado(7 - intentosRestantes);
                Console.WriteLine($"Pista: {pista}");
                Console.WriteLine($"Palabra: {new string(palabraAdivinada)}");
                Console.WriteLine($"Letras incorrectas: {string.Join(", ", letrasIncorrectas)}");
                Console.WriteLine($"\nEscribe una letra o 'ayuda' para revelar una letra (máx. 2 veces).");
                Console.Write("Ingresa una opción: ");

                string entrada = Console.ReadLine()?.Trim().ToLower();

                if (entrada == "ayuda")
                {
                    int letrasFaltantes = palabraAdivinada.Count(c => c == '_');

                    if (usosDeAyuda < 2 && intentosRestantes > 1 && letrasFaltantes > 1)
                    {
                        RevelarLetra(palabraSecreta, palabraAdivinada);
                        intentosRestantes--;
                        usosDeAyuda++;


                        if (new string(palabraAdivinada) == palabraSecreta)
                            break;
                    }
                    else
                    {
                        Console.WriteLine("No puedes pedir más ayuda.");
                        Console.ReadKey();
                    }
                    continue;
                }

                if (entrada.Length != 1 || !char.IsLetter(entrada[0]))
                {
                    Console.WriteLine("Entrada inválida. Ingresa solo una letra.");
                    Console.ReadKey();
                    continue;
                }

                char intento = entrada[0];

                if (palabraSecreta.Contains(intento))
                {
                    for (int i = 0; i < palabraSecreta.Length; i++)
                    {
                        if (palabraSecreta[i] == intento)
                        {
                            palabraAdivinada[i] = intento;
                        }
                    }
                }
                else if (!letrasIncorrectas.Contains(intento))
                {
                    letrasIncorrectas.Add(intento);
                    intentosRestantes--;
                }
            }



            Console.Clear();
            DibujarAhorcado(7 - intentosRestantes);

            if (new string(palabraAdivinada) == palabraSecreta)
            {
                Console.WriteLine($"\n¡Felicidades! Has encontrado la palabra: {palabraSecreta}");
            }
            else
            {
                Console.WriteLine($"\nPerdiste. La palabra era: {palabraSecreta}");

                Console.Beep(400, 500);
                Console.Beep(300, 500);
                Console.Beep(200, 800);
            }

            Console.WriteLine("\nPresiona cualquier tecla para salir...");
            Console.ReadKey();
        }

        static void DibujarAhorcado(int errores)
        {
            string[] dibujo =
            {
            "  +---+",
            "  |   |",
            "      |",
            "      |",
            "      |",
            "      |",
            "========="
        };

            switch (errores)
            {
                case 1: dibujo[2] = "  O   |"; break;
                case 2: dibujo[2] = "  O   |"; dibujo[3] = "  |   |"; break;
                case 3: dibujo[2] = "  O   |"; dibujo[3] = " /|   |"; break;
                case 4: dibujo[2] = "  O   |"; dibujo[3] = " /|\\  |"; break;
                case 5: dibujo[2] = "  O   |"; dibujo[3] = " /|\\  |"; dibujo[4] = " /    |"; break;
                case 6: dibujo[2] = "  O   |"; dibujo[3] = " /|\\  |"; dibujo[4] = " / \\  |"; break;
                case 7: dibujo[2] = "  X   |"; dibujo[3] = " /|\\  |"; dibujo[4] = " / \\  |"; dibujo[5] = "¡AHORCADO!"; break;
            }

            foreach (var linea in dibujo)
            {
                Console.WriteLine(linea);
            }
        }
        static void RevelarLetra(string palabraSecreta, char[] palabraAdivinada)
        {
            Random rand = new Random();
            List<int> posicionesOcultas = new List<int>();

            for (int i = 0; i < palabraSecreta.Length; i++)
            {
                if (palabraAdivinada[i] == '_')
                {
                    posicionesOcultas.Add(i);
                }
            }

            if (posicionesOcultas.Count > 0)
            {
                int indice = posicionesOcultas[rand.Next(posicionesOcultas.Count)];
                palabraAdivinada[indice] = palabraSecreta[indice];
            }
        }

    }

}