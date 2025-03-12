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

namespace MINIJUEGOS_ASCII
{
    internal class Menú
    {
        private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(); // Miembro necesario para la funcionalidad de la música
        private static Task musicaTask; // Objeto para controlar la operación asincrónica que se ejecuta en segundo plano. Es una forma de manejar operaciones que toman tiempo (como la reproducción de música o el acceso a archivos) sin bloquear el hilo principal de ejecución.

        // Variables necesarias para cargar en memoria el menú gráfico
        static string[] lines = File.ReadAllLines("assets/Menus/MenuMinijuegos.txt"); // Leer todas las líneas del archivo
        static string[,] escenario = new string[100, 35]; // Declara un arreglo para "llenarlo" con el menú más adelante

        public static void Main(string[] args)
        {
            // Configuración previa a ejecución del menú
            Console.SetWindowSize(100, 35); // Ajusta el tamaño de la ventana de la consola
            Console.SetBufferSize(100, 35);

            // Configuración previa de la música

            //if (cancellationTokenSource.IsCancellationRequested) // Reiniciar el CancellationTokenSource si ya fue cancelado
            //{
            //    cancellationTokenSource.Dispose(); // Liberar recursos del antiguo
            //    cancellationTokenSource = new CancellationTokenSource(); // Crear uno nuevo
            //}

            // Iniciar la tarea de música con el CancellationToken
            musicaTask = Task.Run(() => Musica(cancellationTokenSource.Token)); // Usa Task para ejecutar la música en segundo plano

            CargarMenu(true);

            // Funcionalidad del menú
            while (true) // Mantiene el menú activo siempre
            {
                bool opcionNoValida = false;
                Console.SetCursorPosition(49, 31);
                var Opcion = Console.ReadLine();

                switch (Opcion)
                {
                    case "1":
                        ConfiguracionPosterior();
                        Laberinto.MenuPrincipal(); // Se ejecuta el minijuego
                        break;
                    case "2":
                        ConfiguracionPosterior();
                        PiedraPapelYTijeras.Iniciar();
                        break;
                    case "3":
                        ConfiguracionPosterior();
                        Ahorcado.Iniciar();
                        break;
                    case "4":
                        Console.SetCursorPosition(8, 32);
                        Console.WriteLine("Saliendo...      ");
                        System.Threading.Thread.Sleep(2000);
                        return; // Sale del programa
                    default:
                        Console.SetCursorPosition(8, 32);
                        Console.WriteLine("Opción no válida.");
                        opcionNoValida = true; // Variable que nos permitirá saber si el flujo del programa pasó por el caso 'default' del switch
                        break;
                }

                // Las siguientes instrucciones deben ejecutarse solamente después de finalizar cada minijuego
                if (!opcionNoValida) // Esto evita que se duplique de nuevo la música después de que se ejecuten las instrucciones por el caso 'default' del switch
                {
                    CargarMenu(false); // Imprimir menú gráfico de nuevo al finalizar un minijuego

                    // Reactivar la música después de jugar
                    cancellationTokenSource = new CancellationTokenSource();
                    musicaTask = Task.Run(() => Musica(cancellationTokenSource.Token));
                }
            }

            //Info. de desarrollador
            //Console.SetBufferSize(120, 35);
            //Console.WriteLine($"Buffer: {Console.BufferWidth}x{Console.BufferHeight}");
        }
        static void Musica(CancellationToken token) // Carga y reproduce la música
        {
            try
            {
                while (!token.IsCancellationRequested) // // Bucle infinito para la reproducción continua, se detiene si se solicita la cancelación
                {
                    // Utiliza un bloque using para asegurar que los recursos se liberen correctamente después de usarlos
                    using (var audioFile = new AudioFileReader("assets/Sounds/Lobby.mp3")) // Crea un objeto AudioFileReader para leer el archivo de audio especificado
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
                            Task.Delay(3000).Wait(); // Mantiene el bucle mientras la música se esté reproduciendo
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
        static void CargarMenu(bool lineaDeEjecucion) // Imprime el menú gráfico a petición (true = Carga e imprime, false = Imprime)
        {
            if (lineaDeEjecucion)
            {
                // Llenar el arreglo bidimensional 'escenario' con los caracteres del archivo previamente guardados en el arreglo 'lines'
                for (int y = 0; y < lines.Length; y++)
                {
                    for (int x = 0; x < lines[y].Length; x++)
                    {
                        escenario[x, y] = lines[y][x].ToString();
                    }
                }
            }

            // Imprimir el arreglo que contiene nuestro menú gráfico
            for (int y = 0; y < 35; y++) // Va imprimiendo columna por columna
            {
                Console.SetCursorPosition(0, y);
                for (int x = 0; x < 100; x++)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;

                    if (x >= 31 && x <= 69 && y >= 3 && y <= 4) Console.ForegroundColor = ConsoleColor.DarkGreen;

                    // Pinta "MINIJUEGOS"
                    if (x >= 24 && x <= 74 && y >= 8 && y <= 10)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    // Pinta "ASCII"
                    if (x >= 24 && x <= 59 && y >= 12 && y <= 14)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;

                        if (escenario[x, y] == "A" || escenario[x, y] == "S" || escenario[x, y] == "C" || escenario[x, y] == "I")
                        {
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                        }
                    }
                    // Pinta las crucetas
                    if (x >= 64 && x <= 69 && y >= 12 && y <= 14)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        if (escenario[x, y] == "^")
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                        }
                    }
                    if (x >= 59 && x <= 74 && y >= 15 && y <= 17)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        if (escenario[x, y] == "<")
                        {
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        }
                        if (escenario[x, y] == ">")
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                        }
                        if (escenario[x, y] == "v")
                        {
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                        }
                    }
                    Console.Write(escenario[x, y]);
                }
            }
        }
        static void ConfiguracionPosterior() // Configuración de la consola posterior a finalización del menú
        {
            // Texto completamente estético
            Console.SetCursorPosition(8, 32);
            Console.WriteLine("Cargando...      ");

            cancellationTokenSource.Cancel(); // Solicita la cancelación de la tarea de música
            musicaTask.Wait(); // Espera a que la tarea de música termine

            Console.Clear();
            Console.SetCursorPosition(0, 0);
        }
    } // Mi método Main
    internal class Laberinto
    {
        // MIEMBROS FUNDAMENTALES PARA EL JUEGO
        //                                           X=100 Columnas y Y=35 filas
        public static byte[] CoordPersonaje = new byte[2]; //Un arreglo de dos posiciones para guardar las coordenadas X y Y de nuestro personaje
        public static ConsoleKeyInfo BotonPresionado = new ConsoleKeyInfo(); // Objeto al que se le pueden asignar valores de lo que se teclea
        public static string EscenarioActual = "Zvezda";
        public static bool evento = false; // Nos servirá para saber si ocurrió un evento

        // MIEMBROS NECESARIOS PARA LA FUNCIONALIDAD DE ENTRADA POR RATÓN
        public static CONSOLE_FONT_INFO consoleFontInfo;
        public static Rectangle consoleWindow; // Nos servirá después para obtener el rectángulo de la ventana de la consola

        //MIEMBROS NECESARIOS PARA LA FUNCIONALIDAD DE MÚSICA
        private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        // Escenarios

        public static string[,] Zvezda = new string[100, 35]; // Conecta directamente con Zarya
        public static string[,] Zarya = new string[100, 35]; // Conecta directamente con Zvezda y por ascensor con Nauka
        public static string[,] Nauka = new string[100, 35]; // Conecta mediante ascensor con Zarya
        public static string[,] Rassvet = new string[100, 35]; // Conecta mediante ascensor con Zarya

        //Personajes
        static bool[] Dialogos = new bool[2];

        public static void PrepararAmbienteConsola() // Forma parte de la funcionalidad de entrada por ratón
        {
            // Ajustes previos para asegurarnos que el minijuego se vea bien visualmente

            MaximizarVentana(); // Maximizar la ventana de la consola
            Console.Clear(); // Limpiar la pantalla de la consola
            Console.CursorVisible = false; // Ocultar el cursor de la consola
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            // Ajustes previos para la funcionalidad encargada de leer la entrada por ratón

            IntPtr hConsole = GetStdHandle(-11); // Obtener el manejador de la consola de salida estándar

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
            if (File.Exists(filePath))
            {
                // Leer y mostrar el contenido del archivo
                string contenido = File.ReadAllText(filePath);
                Console.WriteLine(contenido);
            }
            else
            {
                Console.WriteLine("El archivo no existe.");
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
            PrepararEscenario("assets/Mapas/Zarya2.txt", Zarya);
            PrepararEscenario("assets/Mapas/Nauka.txt", Nauka);
            PrepararEscenario("assets/Mapas/Rassvet.txt", Rassvet);

            PintarEscenario(Zvezda, 50, 35); // Imprimimos por primera vez nuestro primer escenario Zvezda

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
                    if (EscenarioActual == "Rassvet")
                    {
                        Movimiento(Rassvet);
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

            for (int y = 0; y < escenario.GetLength(1); y++) // Altura del escenario
            {
                Console.SetCursorPosition(0, y);
                for (int x = 0; x < escenario.GetLength(0); x++) // Ancho del escenario
                {
                    Console.Write(escenario[x, y]);
                }
            }

            Console.SetCursorPosition(CoordPersonaje[0], CoordPersonaje[1]);
            Console.Write("O");
        }
        public static void PintarDialogo(string rutaEscenario, byte CoordX, byte CoordY)
        {
            string[] lines = File.ReadAllLines(rutaEscenario); // Leer todas las líneas del archivo

            for (int x = 0; x < lines.Length; x++) // Imprimir el diálogo
            {
                Console.SetCursorPosition(CoordX, CoordY + x); // Ubicar el cursor antes de imprimir
                Console.WriteLine(lines[x]);
            }
        }
        public static void Movimiento(string[,] escenario) // Desplaza el personaje por el mapa, pero solo después de verificar que es posible
        {
            BotonPresionado = Console.ReadKey(true); // true evita que se muestre la tecla presionada
            evento = false;

            byte ActualCoordXPersonaje = CoordPersonaje[0];
            byte ActualCoordYPersonaje = CoordPersonaje[1]; // Guarda la posición (x,y) actual antes de moverse

            if (BotonPresionado.Key == ConsoleKey.DownArrow && ActualCoordYPersonaje < 34 && EsPosicionValida(escenario, new int[] { ActualCoordXPersonaje, ActualCoordYPersonaje + 1 }))
            {
                if (!EventosEscenario(escenario, new int[] { ActualCoordXPersonaje, ActualCoordYPersonaje }))
                {
                    CoordPersonaje[1]++;
                }
            }
            else if (BotonPresionado.Key == ConsoleKey.UpArrow && ActualCoordYPersonaje > 0 && EsPosicionValida(escenario, new int[] { ActualCoordXPersonaje, ActualCoordYPersonaje - 1 }))
            {
                if (!EventosEscenario(escenario, new int[] { ActualCoordXPersonaje, ActualCoordYPersonaje }))
                {
                    CoordPersonaje[1]--;
                }
            }
            else if (BotonPresionado.Key == ConsoleKey.LeftArrow && ActualCoordXPersonaje > 0 && EsPosicionValida(escenario, new int[] { ActualCoordXPersonaje - 1, ActualCoordYPersonaje }))
            {
                if (!EventosEscenario(escenario, new int[] { ActualCoordXPersonaje, ActualCoordYPersonaje }))
                {
                    CoordPersonaje[0]--;
                }
            }
            else if (BotonPresionado.Key == ConsoleKey.RightArrow && ActualCoordXPersonaje < 99 && EsPosicionValida(escenario, new int[] { ActualCoordXPersonaje + 1, ActualCoordYPersonaje }))
            {
                if (!EventosEscenario(escenario, new int[] { ActualCoordXPersonaje, ActualCoordYPersonaje }))
                {
                    CoordPersonaje[0]++;
                }
            }
            else if (BotonPresionado.Key == ConsoleKey.W)
            {
                EventosEscenario(escenario, new int[] { ActualCoordXPersonaje, ActualCoordYPersonaje });
            }
            else if (BotonPresionado.Key == ConsoleKey.S)
            {
                EventosEscenario(escenario, new int[] { ActualCoordXPersonaje, ActualCoordYPersonaje });
            }
            else if (BotonPresionado.Key == ConsoleKey.E)
            {
                EventosEscenario(escenario, new int[] { ActualCoordXPersonaje, ActualCoordYPersonaje });
            }

            if (!evento && !NoActualizarEscenario) // Verificamos si hubo un evento par no actualizará el escenario, de lo contrario, nos restaurará la celda del escenario anterior sobre el nuevo escenario
            {
                ActualizarEscenario(escenario, ActualCoordXPersonaje, ActualCoordYPersonaje); // Enviamos la posición anterior para que se restaure bien
            }

            NoActualizarEscenario = false;

        }
        public static bool EsPosicionValida(string[,] escenario, int[] coordenadas) // Retorna un true si la celda a desplazarse no tiene colisión
        {
            string celda = escenario[coordenadas[0], coordenadas[1]];
            return celda != "#" && celda != "║" && celda != "(" && celda != ")" && celda != " " && celda != "≡";
        }
        public static bool EventosEscenario(string[,] escenario, int[] coordenadas) // Retorna un true si la celda a desplazarse tiene un evento
        {
            string celda = escenario[coordenadas[0], coordenadas[1]]; // Celda a analizar

            if (celda == "S")
            {
                if (EscenarioActual == "Zvezda" && misiones[0])
                {
                    PintarEscenario(Zarya, 50, 33);
                    EscenarioActual = "Zarya";
                    evento = true; // Efectivamente ocurrió un evento
                }
                else if (EscenarioActual == "Zvezda" && !misiones[0])
                {
                    Console.SetCursorPosition(67, 33);
                    Console.ForegroundColor = ConsoleColor.Red; // Cambiar el color del mensaje
                    Console.Write("Aviso: Debes restaurar la energía del módulo Zvezda");
                    Console.ResetColor();
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
                    switch (EscenarioActual)
                    {
                        case "Zarya":
                            if (misiones[1] == true)
                            {
                                if (CoordPersonaje[0] == 49 && CoordPersonaje[1] == 29) // Traslado de escenario para ir de Zarya a Nauka mediante ascensor (A)
                                {
                                    PintarEscenario(Nauka, 50, 30);
                                    EscenarioActual = "Nauka";
                                    evento = true;
                                }
                                if (CoordPersonaje[0] == 49 && CoordPersonaje[1] == 4) // Traslado de escenario para ir de Zarya a Rassvet mediante ascensor (A)
                                {
                                    PintarEscenario(Rassvet, 50, 5);
                                    EscenarioActual = "Rassvet";
                                    evento = true;
                                }
                            }
                            else
                            {
                                Console.SetCursorPosition(67, 33);
                                Console.ForegroundColor = ConsoleColor.Red; // Cambiar el color del mensaje
                                Console.Write("Aviso: Debes recolectar todos los suministros");
                                Console.ResetColor();
                            }
                            break;
                        default:
                            break;
                    }
                }

                if (BotonPresionado.Key == ConsoleKey.W)
                {
                    switch (EscenarioActual)
                    {
                        case "Nauka":
                            if (CoordPersonaje[0] == 49 && CoordPersonaje[1] == 29) // Traslado de escenario para ir de Zarya a Nauka mediante ascensor (A)
                            {
                                PintarEscenario(Zarya, 50, 30);
                                EscenarioActual = "Zarya";
                                evento = true;
                            }
                            break;
                        case "Rassvet":
                            if (CoordPersonaje[0] == 49 && CoordPersonaje[1] == 4) // Traslado de escenario para ir de Zarya a Nauka mediante ascensor (A)
                            {
                                PintarEscenario(Zarya, 50, 5);
                                EscenarioActual = "Zarya";
                                evento = true;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            if (celda == "°")
            {
                ManejarInterruptores(escenario, coordenadas);
            }

            // Dialogos
            switch (EscenarioActual)
            {
                case "Zvezda":
                    if (coordenadas[0] > 35 && coordenadas[0] < 63 && coordenadas[1] == 29 && !Dialogos[0])
                    {
                        PintarDialogo("assets/Dialogos/ZvezdaDialogo1-1.txt", 51, 23);
                        Console.ReadKey();
                        PintarEscenario(escenario, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1)); // Le sumo +1 porque el método PintarEscenario por defecto me resta -1
                        PintarDialogo("assets/Dialogos/ZvezdaDialogo1-2.txt", 51, 23);
                        Console.ReadKey();
                        PintarEscenario(escenario, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));
                        PintarDialogo("assets/Dialogos/ZvezdaDialogo1-3.txt", 51, 23);
                        Console.ReadKey();
                        PintarEscenario(escenario, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));
                        PintarDialogo("assets/Dialogos/ZvezdaDialogo1-4.txt", 51, 23);
                        Console.ReadKey();
                        PintarEscenario(escenario, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));

                        Dialogos[0] = true;
                        evento = true;
                    }
                    break;
                case "Zarya":
                    if (coordenadas[0] > 45 && coordenadas[0] < 54 && coordenadas[1] == 27 && !Dialogos[1])
                    {
                        PintarDialogo("assets/Dialogos/ZaryaDialogo1-1.txt", 51, 23);
                        Console.ReadKey();
                        PintarEscenario(escenario, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1)); // Le sumo +1 porque el método PintarEscenario por defecto me resta -1
                        PintarDialogo("assets/Dialogos/ZaryaDialogo1-2.txt", 51, 23);
                        Console.ReadKey();
                        PintarEscenario(escenario, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));
                        PintarDialogo("assets/Dialogos/ZaryaDialogo1-3.txt", 51, 23);
                        Console.ReadKey();
                        PintarEscenario(escenario, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));
                        PintarDialogo("assets/Dialogos/ZaryaDialogo1-4.txt", 51, 23);
                        Console.ReadKey();
                        PintarEscenario(escenario, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));
                        PintarDialogo("assets/Dialogos/ZaryaDialogo1-5.txt", 51, 23);
                        Console.ReadKey();
                        PintarEscenario(escenario, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));
                        PintarDialogo("assets/Dialogos/ZaryaDialogo1-6.txt", 51, 23);
                        Console.ReadKey();
                        PintarEscenario(escenario, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));

                        Dialogos[1] = true;
                        evento = true;
                    }
                    break;
                default:
                    break;
            }

            // Recolectar suministros en Zarya
            if (EscenarioActual == "Zarya" && celda == "@")
            {
                escenario[coordenadas[0], coordenadas[1]] = "░"; // Eliminar el suministro del mapa
                SuministrosRecolectados++;
                Console.SetCursorPosition(67, 31); // Fuera del área del escenario
                Console.WriteLine($"Suministros recolectados: {SuministrosRecolectados}/{SuministrosTotales}");

                if (SuministrosRecolectados == SuministrosTotales)
                {
                    Console.SetCursorPosition(67, 33);
                    Console.ForegroundColor = ConsoleColor.Green; // Cambiar el color del mensaje
                    Console.Write("Aviso: ¡Todos los suministros recolectados!  ");
                    Console.ResetColor();
                    // Desbloquear acceso a Nauka
                    misiones[1] = true; // Suponiendo que misiones[1] controla el acceso a Nauka
                }

                evento = true; // Hubo un evento (recolectar suministro)
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

        //    Task.Delay(100).Wait(); // Mantiene el bucle

        // Variables que necesita el método ManejarInterruptores
        static bool NoActualizarEscenario = false;
        static bool[] interruptores = new bool[3]; // Guarda el estado de los interruptores
        static int[] ordenActivacion = new int[3] { -1, -1, -1 }; // Guarda el orden en que se activan
        static byte siguienteInterruptor = 0; // Indica qué interruptor se debe presionar en el orden correcto
        static bool[] misiones = new bool[2];
        public static void ManejarInterruptores(string[,] escenario, int[] coordenadas)
        {
            string celda = escenario[coordenadas[0], coordenadas[1]];

            // Si la celda tiene un interruptor ("°") y se presiona la tecla "E"
            if (celda == "°" && BotonPresionado.Key == ConsoleKey.E)
            {
                int index = -1;

                // Determinar qué interruptor se está presionando
                if (coordenadas[0] == 42 && coordenadas[1] == 6) index = 0;
                if (coordenadas[0] == 42 && coordenadas[1] == 9) index = 1;
                if (coordenadas[0] == 42 && coordenadas[1] == 12) index = 2;

                if (index != -1)
                {
                    interruptores[index] = !interruptores[index]; // Cambiar el estado del interruptor

                    // Si el interruptor no ha sido activado en el orden correcto, reiniciar
                    if (ordenActivacion[index] == -1)
                    {
                        ordenActivacion[index] = siguienteInterruptor;
                        siguienteInterruptor++;
                    }

                    // Mover el cursor a la posición del interruptor
                    Console.SetCursorPosition(coordenadas[0], coordenadas[1]);

                    // Cambiar el color a verde si el interruptor está activado
                    if (interruptores[index])
                    {
                        Console.ForegroundColor = ConsoleColor.Green; // Establece el color verde
                        Console.Write("°");
                    }
                    else
                    {
                        Console.ResetColor(); // Restablece el color por defecto
                        Console.Write("°");
                    }

                    // Verificar si todos los interruptores están activados
                    if (interruptores[0] && interruptores[1] && interruptores[2])
                    {
                        // Comprobar si se activaron en el orden correcto
                        Console.SetCursorPosition(67, 33);
                        Console.ForegroundColor = ConsoleColor.Green; // Cambiar el color del mensaje
                        if (ordenActivacion[0] == 0 && ordenActivacion[1] == 1 && ordenActivacion[2] == 2)
                        {
                            Console.Write("Aviso: Energía restaurada en Zvezda                ");
                            misiones[0] = true;
                        }
                        else
                        {
                            // Si el orden no es el correcto, reiniciar todo y permitir reintentar
                            ResetearInterruptores();
                            Console.SetCursorPosition(67, 33);
                            Console.ForegroundColor = ConsoleColor.Red; // Cambiar el color del mensaje
                            Console.Write("Aviso: Orden incorrecto, vuelve a intentarlo");
                        }
                    }
                }
                Console.ResetColor(); // Restablece el color por defecto
                NoActualizarEscenario = true; // Evita actualizar la pantalla
                evento = true; // Indica que ocurrió un evento
            }
        }

        public static void ResetearInterruptores()
        {
            // Restablecer el estado de los interruptores y el orden de activación
            interruptores = new bool[3];
            ordenActivacion = new int[3] { -1, -1, -1 };
            siguienteInterruptor = 0;
        }

        // Variables que necesita el método ManejarInterruptores
        public static int SuministrosRecolectados = 0; // Cantidad de suministros recolectados
        public static int SuministrosTotales = 5; // Total de suministros en Zarya (ajusta según tu mapa)
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
        private static Random random = new Random(); // Generador de números aleatorios de la computadora
        private static bool instruccionesMostradas = false;

        public static void Iniciar()
        {
            Console.Clear(); // Limpiar la pantalla de la consola 

            while (true)
            {
                Console.Clear();

                if (!instruccionesMostradas)
                {
                    MostrarInstruccionesLadoDerecho();
                    instruccionesMostradas = true;
                }

                Console.SetCursorPosition(10, 3);
                Console.WriteLine("Seleccione una opción con el número correspondiente:");

                // Mostrar las opciones numeradas
                for (int i = 0; i < opciones.Length; i++)
                {
                    Console.SetCursorPosition(10, 5 + i * 5);
                    Console.WriteLine($"{i + 1}. {opciones[i]}");
                    DibujarOpcion(opciones[i], 10, 6 + i * 5);
                }

                // Leer la opción del usuario
                int eleccionUsuario;
                Console.SetCursorPosition(10, 6 + opciones.Length * 5);
                Console.Write("Selecciona una opción: ");
                if (int.TryParse(Console.ReadLine(), out eleccionUsuario) && eleccionUsuario >= 1 && eleccionUsuario <= 3)
                {
                    Jugar(opciones[eleccionUsuario - 1]); // Jugar con la opción seleccionada
                }
                else
                {

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.SetCursorPosition(10, 6 + opciones.Length * 5 + 1);
                    Console.WriteLine("¡ERROR! Presiona cualquier tecla e ingresa una opción válida.");
                    Console.ResetColor();
                    Console.ReadKey();
                    continue; // Volver al principio del ciclo si la opción es inválida
                }


                Console.Clear();
                Console.WriteLine("DESEAS SEGUIR");
                Console.WriteLine("1. Sí");
                Console.WriteLine("2. No");
                string respuesta = Console.ReadLine();

                while (respuesta != "1" && respuesta != "2")
                {

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("¡ERROR! Ingresa una opción válida");
                    Console.ResetColor();
                    respuesta = Console.ReadLine();
                }

                if (respuesta == "2")
                {
                    Console.WriteLine("GRACIAS");
                    break;
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

        private static void DibujarOpcion(string opcion, int x, int y) // Mueve el cursor a la siguiente línea
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
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.WriteLine("     █   █    ");
                    Console.SetCursorPosition(x, y + 1);
                    Console.WriteLine("      █ █     ");
                    Console.SetCursorPosition(x, y + 2);
                    Console.WriteLine("       █      ");
                    Console.SetCursorPosition(x, y + 3);
                    Console.WriteLine("     ██ ██    ");
                    break;
            }


            Console.ResetColor();
        }

        private static void MostrarInstruccionesLadoDerecho()
        {
            int startX = 40;
            int startY = 5;


            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition(startX, startY);
            Console.WriteLine("Instrucciones:");
            Console.ForegroundColor = ConsoleColor.White;


            string[] instrucciones = new string[]
            {
            "1. Elige una opción entre Piedra, Papel o Tijeras.",
            "2. Usa el número correspondiente para seleccionar la opción.",
            "3. Piedra vence a Tijeras.",
            "4. Tijeras vencen a Papel.",
            "5. Papel vence a Piedra.",
            "6. Si ambos eligen la misma opción, es un empate."
            };


            for (int i = 0; i < instrucciones.Length; i++)
            {
                Console.SetCursorPosition(startX, startY + i + 1);
                if (i == 0)
                    Console.ForegroundColor = ConsoleColor.Green;
                else if (i == 1)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else if (i == 2)
                    Console.ForegroundColor = ConsoleColor.Red;
                else if (i == 3)
                    Console.ForegroundColor = ConsoleColor.Magenta;
                else if (i == 4)
                    Console.ForegroundColor = ConsoleColor.Cyan;
                else
                    Console.ForegroundColor = ConsoleColor.Gray;

                Console.WriteLine(instrucciones[i]);
            }


            Console.ResetColor();
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