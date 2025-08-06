using NAudio.Wave; // Importa el espacio de nombres NAudio.Wave para manejar operaciones de audio
using System; // Importa el espacio de nombres System para utilizar las funcionalidades básicas de C#
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks; // Importa el espacio de nombres System.Threading.Tasks, que proporciona tipos y métodos para manejar operaciones asíncronas y paralelas
using System.Windows.Forms;


namespace MINIJUEGOS_ASCII
{
    internal class Menú // Mi método Main
    {
        private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(); // Miembro necesario para la funcionalidad de la música
        private static Task musicaTask; // Objeto para controlar la operación asincrónica que se ejecuta en segundo plano. Es una forma de manejar operaciones que toman tiempo (como la reproducción de música o el acceso a archivos) sin bloquear el hilo principal de ejecución.

        // Variables necesarias para cargar en memoria el menú gráfico
        static string filePath = Path.Combine(Application.StartupPath, "bin", "Debug", "assets", "Menus", "MenuMinijuegos.txt");
        static string[] lines = File.ReadAllLines(filePath); // Leer todas las líneas del archivo
        static string[,] escenario = new string[100, 35]; // Declara un arreglo para "llenarlo" con el menú más adelante

        public static void Main(string[] args)
        {
            // Configuración de la consola antes de la ejecución del menú
            ConsoleHelper.DisableQuickEdit(); // Deshabilitar QuickEdit Mode
            Console.CursorVisible = false;

            Console.SetWindowSize(100, 36); // Ajusta el tamaño de la ventana de la consola para todo el programa
            Console.SetBufferSize(100, 36); // Evita el scroll

            // Iniciar la tarea de música con el CancellationToken
            musicaTask = Task.Run(() => Musica(cancellationTokenSource.Token)); // Usa Task para ejecutar la música en segundo plano

            CargarMenu(true); // Cargar el menú gráfico por primera vez

            // Funcionalidad del menú
            while (true) // Mantiene el menú activo siempre
            {
                Point posicionConsola = FuncionalidadMouse.ObtenerPosicionMouseEnConsola(); // Parte de la funcionalidad del mouse
                Debug.WriteLine("Posición X: " + posicionConsola.X + ", Posición Y: " + posicionConsola.Y); // Info. para Desarrollador

                // Verificar si se presionó el botón izquierdo del mouse
                if (Control.MouseButtons == MouseButtons.Left)
                {
                    // Coordenadas del botón Laberinto
                    if (posicionConsola.X >= 35 && posicionConsola.X <= 63 && posicionConsola.Y >= 21 && posicionConsola.Y <= 23)
                    {
                        // Detener la música antes de iniciar el minijuego
                        cancellationTokenSource.Cancel(); // Solicita la cancelación de la tarea de música
                        musicaTask.Wait(); // Espera a que la tarea de música termine

                        // Ejecutar el minijuego de Laberinto
                        ConfiguracionConsola(true); // Configura la consola para el minijuego
                        Laberinto.MenuPrincipal(); // Se ejecuta el minijuego

                        // Volver al menú principal después de terminar el minijuego
                        ConfiguracionConsola(false); // Restaura la configuración de la consola
                        CargarMenu(false); // Volver a cargar el menú gráfico
                    }

                    // Coordenadas del botón Piedra, Papel o Tijeras
                    if (posicionConsola.X >= 35 && posicionConsola.X <= 63 && posicionConsola.Y >= 25 && posicionConsola.Y <= 27)
                    {
                        // Detener la música antes de iniciar el minijuego
                        cancellationTokenSource.Cancel(); // Solicita la cancelación de la tarea de música
                        musicaTask.Wait(); // Espera a que la tarea de música termine

                        // Ejecutar el minijuego de Piedra, Papel o Tijeras
                        ConfiguracionConsola(true);
                        PiedraPapelYTijeras.Iniciar();

                        // Volver al menú principal después de terminar el minijuego
                        ConfiguracionConsola(false); // Restaurar la configuración de la consola
                        CargarMenu(false); // Volver a cargar el menú gráfico
                    }

                    // Coordenadas del botón Ahorcado
                    if (posicionConsola.X >= 35 && posicionConsola.X <= 63 && posicionConsola.Y >= 29 && posicionConsola.Y <= 31)
                    {
                        // Detener la música antes de iniciar el minijuego
                        cancellationTokenSource.Cancel(); // Solicita la cancelación de la tarea de música
                        musicaTask.Wait(); // Espera a que la tarea de música termine

                        // Ejecutar el minijuego de Ahorcado
                        ConfiguracionConsola(true);
                        Ahorcado.Ejecutar();

                        // Volver al menú principal después de terminar el minijuego
                        ConfiguracionConsola(false); // Restaurar la configuración de la consola
                        CargarMenu(false); // Volver a cargar el menú gráfico
                    }

                    // Coordenadas del "botón" Salir
                    if (posicionConsola.X >= 78 && posicionConsola.X <= 92 && posicionConsola.Y >= 29 && posicionConsola.Y <= 31)
                    {
                        // Detener la música al salir
                        cancellationTokenSource.Cancel(); // Solicita la cancelación de la tarea de música
                        musicaTask.Wait(); // Espera a que la tarea de música termine

                        // Mostrar mensaje de salida
                        Console.SetCursorPosition(8, 31);
                        Console.Write("Saliendo...      ");
                        Console.SetCursorPosition(49, 30);
                        Thread.Sleep(2000); // Hace una pausa del programa por 2 segundos
                        return; // Sale del programa
                    }

                    // Limpiar el estado del mouse después del clic
                }
            }
        }

        static void Musica(CancellationToken token) // Carga y reproduce la música
        {
            try
            {
                while (!token.IsCancellationRequested) // // Bucle infinito para la reproducción continua, se detiene si se solicita la cancelación
                {
                    // Utiliza un bloque using para asegurar que los recursos se liberen correctamente después de usarlos
                    using (var audioFile = new AudioFileReader("bin/Debug/assets/Sounds/Lobby.mp3")) // Crea un objeto AudioFileReader para leer el archivo de audio especificado
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
                            Task.Delay(1000).Wait(); // Mantiene el bucle mientras la música se esté reproduciendo
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write($"Error en la reproducción de música: {ex.Message}");
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

                    // Pinta los botones
                    if (x >= 35 && x <= 63 && y >= 21 && y <= 23) // Coordenadas del Botón Laberinto
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    if (x >= 35 && x <= 63 && y >= 25 && y <= 27) // Coordenadas del Botón Piedra...
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    if (x >= 35 && x <= 63 && y >= 29 && y <= 31) // Coordenadas del Botón El Ahorcado y Salir
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    if (x >= 78 && x <= 92 && y >= 29 && y <= 31) // Coordenadas del Botón Salir
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    
                    Console.Write(escenario[x, y]);
                }
            }
        }
        static void ConfiguracionConsola(bool decision) // Configuración de la consola antes y después de la ejecución del minijuego
        {
            if (decision) // Configuración para iniciar un minijuego
            {
                // Instrucciones destinadas a la estética
                Console.SetCursorPosition(8, 32);
                Console.Write("Cargando...      ");
                Console.SetCursorPosition(49, 31);

                cancellationTokenSource.Cancel(); // Solicita la cancelación de la tarea de música
                musicaTask.Wait(); // Espera a que la tarea de música termine

                // Dejar todo limpio y listo para la ejecución del minijuego
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Console.ResetColor();
            }
            else // Configuración para volver al menú principal
            {
                // Restaurar la configuración de la consola
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Console.BackgroundColor = ConsoleColor.White; // Restablecer el color de fondo
                Console.ForegroundColor = ConsoleColor.Black; // Restablecer el color del texto
                // Resturar la música
                cancellationTokenSource = new CancellationTokenSource();
                musicaTask = Task.Run(() => Musica(cancellationTokenSource.Token));
            }
        }
    }
    internal class Laberinto
    {
        // MIEMBROS FUNDAMENTALES PARA EL JUEGO
        //                                           X=100 Columnas y Y=35 filas
        public static byte[] CoordPersonaje = new byte[2]; //Un arreglo de dos posiciones para guardar las coordenadas X y Y de nuestro personaje
        public static ConsoleKeyInfo BotonPresionado = new ConsoleKeyInfo(); // Objeto al que se le pueden asignar valores de lo que se teclea
        public static string EscenarioActual = "Zvezda";
        public static bool evento = false; // Nos servirá para saber si ocurrió un evento

        // MIEMBROS NECESARIOS PARA LA FUNCIONALIDAD DE MÚSICA
        private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        // Escenarios

        public static string[,] Zvezda = new string[100, 35]; // Conecta directamente con Zarya
        public static string[,] Zarya = new string[100, 35]; // Conecta directamente con Zvezda y por ascensor con Nauka
        public static string[,] Nauka = new string[100, 35]; // Conecta mediante ascensor con Zarya
        public static string[,] Rassvet = new string[100, 35]; // Conecta mediante ascensor con Zarya

        //Personajes
        static bool[] Dialogos = new bool[3];

        public static void ConfigurarConsola(bool decision) // Ajustes previos para asegurarnos que el minijuego se vea bien visualmente (true = Viene del menú de minijuegos, false = Regresa al menú de minijuegos)
        {
            if (decision) // Instrucciones cuando viene del Menú de minijuegos
            {
                Console.CursorVisible = false; // Ocultar el cursor de la consola
                Console.ResetColor();
            }
            else // Instrucciones cuando regresa del propio minijuego
            {
                Console.CursorVisible = true; // Mostrar el cursor de la consola
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
            }
        }
        public static void CargarMenus(string ruta) // Carga e imprime un menú del minijuego indicado en su parámetro de entrada
        {
            Console.Clear(); // Limpia previamente porque se llamará este método varias veces

            // Obtener la ruta base del ejecutable
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            // Combinar la ruta base con la ruta relativa del archivo
            string filePath = Path.Combine(basePath, ruta); // Ruta del archivo de texto, el cual contiene el menú con gráficos ASCII

            // Verificar si el archivo existe
            if (File.Exists(filePath))
            {
                // Leer y mostrar el contenido del archivo
                string contenido = File.ReadAllText(filePath);
                Console.Write(contenido);
            }
            else
            {
                Console.Write("El archivo no existe.");
            }
        }

        public static void Musica(CancellationToken token) // Carga y reproduce la música
        {
            try
            {
                while (!token.IsCancellationRequested) // // Bucle infinito para la reproducción continua, se detiene si se solicita la cancelación
                {
                    // Utiliza un bloque using para asegurar que los recursos se liberen correctamente después de usarlos
                    using (var audioFile = new AudioFileReader("bin/Debug/assets/Sounds/Interstellar.mp3")) // Crea un objeto AudioFileReader para leer el archivo de audio especificado
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
                Console.Write($"Error en la reproducción de música: {ex.Message}");
                throw;
            }

        }
        public static void MenuPrincipal() // Punto de llamada de las funcionalidades para el menú del minijuego
        {
            Debug.WriteLine("Método MenuPrincipal: Running...");

            ConfigurarConsola(true); // Ajustes previos para asegurarnos que el minijuego se vea bien visualmente (true = Viene del menú de minijuegos, false = Regresa al menú de minijuegos)

            CargarMenus("bin/Debug/assets/Menus/MenuPrincipal.txt"); // Carga e imprime un menú del minijuego indicado en su parámetro de entrada

            // Reiniciar el CancellationTokenSource si ya fue cancelado, debido a que se puede regresar a este MenuPrincipal y debe de reproducirse la música nuevamente
            if (cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Dispose(); // Liberar recursos del antiguo
                cancellationTokenSource = new CancellationTokenSource(); // Crear uno nuevo
            }

            // Iniciar la tarea de música con el CancellationToken
            var musicaTask = Task.Run(() => Musica(cancellationTokenSource.Token)); // Usa Task para ejecutar la música en segundo plano

            while (true) // Bluce encargado de mantener ejecutando las funcionalidades para el menú hasta que se elija una de las dos opciones
            {
                Point posicionConsola = FuncionalidadMouse.ObtenerPosicionMouseEnConsola();

                // Verificar si se presionó el botón izquierdo del mouse
                if (Control.MouseButtons == MouseButtons.Left)
                {
                    // Coordenadas del "botón" Iniciar
                    if (posicionConsola.X >= 55 && posicionConsola.X <= 79 && posicionConsola.Y >= 20 && posicionConsola.Y <= 24)
                    {
                        MenuInstrucciones(); // Inicia el Menu de Instrucciones
                        ConfigurarConsola(true);
                        break;
                    }

                    // Coordenadas del "botón" Salir
                    if (posicionConsola.X >= 57 && posicionConsola.X <= 77 && posicionConsola.Y >= 28 && posicionConsola.Y <= 32)
                    {
                        // Detener la música al salir
                        ConfigurarConsola(false);
                        cancellationTokenSource.Cancel(); // Solicita la cancelación de la tarea de música
                        musicaTask.Wait(); // Espera a que la tarea de música termine
                        break; // Sale del ciclo
                    }
                }
            }
        }
        public static void MenuInstrucciones() // Punto de llamada de las funcionalidades para el menú del minijuego
        {
            CargarMenus("bin/Debug/assets/Menus/MenuInstrucciones.txt");

            while (true) // Bluce encargado de mantener ejecutando las funcionalidades para el menú hasta que se elija una de las dos opciones
            {
                Point posicionConsola = FuncionalidadMouse.ObtenerPosicionMouseEnConsola();

                // Verificar si se presionó el botón izquierdo del mouse
                if (Control.MouseButtons == MouseButtons.Left)
                {
                    // Coordenadas del "botón" Jugar
                    if (posicionConsola.X >= 77 && posicionConsola.X <= 93 && posicionConsola.Y >= 28 && posicionConsola.Y <= 32)
                    {
                        Iniciar(); // Inicia el juego
                        break;
                    }
                }
            }
        }
        public static void Iniciar() // Método que "arranca" nuestro minijuego
        {
            // Carga en memoria los mapas para el minijuego
            PrepararEscenario("bin/Debug/assets/Mapas/Zvezda.txt", Zvezda); // Ejecuta el método PrepararEscenario para cargar los escenarios en los arreglos
            PrepararEscenario("bin/Debug/assets/Mapas/Zarya2.txt", Zarya);
            PrepararEscenario("bin/Debug/assets/Mapas/Nauka.txt", Nauka);
            PrepararEscenario("bin/Debug/assets/Mapas/Rassvet.txt", Rassvet);

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
                    if (Dialogos[2])
                    {
                        return;
                    }
                    if (BotonPresionado.Key == ConsoleKey.Escape)
                    {
                        ConfigurarConsola(false);
                        cancellationTokenSource.Cancel(); // Solicita la cancelación de la tarea de música
                        return;
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
            ConfigurarConsola(true);

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
                    PintarEscenario(Zvezda, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));
                    Console.SetCursorPosition(2, 2);
                    Console.ForegroundColor = ConsoleColor.Red; // Cambiar el color del mensaje
                    Console.Write("Aviso: Debes restaurar la energía");
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
                                if (CoordPersonaje[0] == 49 && CoordPersonaje[1] == 4 && misiones[2]) // Traslado de escenario para ir de Zarya a Rassvet mediante ascensor (A)
                                {
                                    PintarEscenario(Rassvet, 50, 5);
                                    EscenarioActual = "Rassvet";
                                    evento = true;
                                }
                                if (CoordPersonaje[0] == 49 && CoordPersonaje[1] == 4 && !misiones[2])
                                {
                                    PintarEscenario(Zarya, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));
                                    Console.SetCursorPosition(2, 2);
                                    Console.ForegroundColor = ConsoleColor.Red; // Cambiar el color del mensaje
                                    Console.Write("Aviso: Sin energía en Rassvet");
                                    Console.ResetColor();
                                }
                            }
                            else
                            {
                                PintarEscenario(Zarya, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));
                                Console.SetCursorPosition(2, 2);
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
                        PintarDialogo("bin/Debug/assets/Dialogos/ZvezdaDialogo1-1.txt", 27, 20);
                        Console.ReadKey();
                        PintarEscenario(escenario, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1)); // Le sumo +1 porque el método PintarEscenario por defecto me resta -1
                        PintarDialogo("bin/Debug/assets/Dialogos/ZvezdaDialogo1-2.txt", 27, 20);
                        Console.ReadKey();
                        PintarEscenario(escenario, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));
                        PintarDialogo("bin/Debug/assets/Dialogos/ZvezdaDialogo1-3.txt", 27, 20);
                        Console.ReadKey();
                        PintarEscenario(escenario, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));
                        PintarDialogo("bin/Debug/assets/Dialogos/ZvezdaDialogo1-4.txt", 27, 20);
                        Console.ReadKey();
                        PintarEscenario(escenario, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));

                        Dialogos[0] = true;
                        evento = true;
                    }
                    break;
                case "Zarya":
                    if (coordenadas[0] > 45 && coordenadas[0] < 54 && coordenadas[1] == 27 && !Dialogos[1])
                    {
                        PintarDialogo("bin/Debug/assets/Dialogos/ZaryaDialogo1-1.txt", 25, 20);
                        Console.ReadKey();
                        PintarEscenario(escenario, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1)); // Le sumo +1 porque el método PintarEscenario por defecto me resta -1
                        PintarDialogo("bin/Debug/assets/Dialogos/ZaryaDialogo1-2.txt", 25, 20);
                        Console.ReadKey();
                        PintarEscenario(escenario, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));
                        PintarDialogo("bin/Debug/assets/Dialogos/ZaryaDialogo1-3.txt", 25, 20);
                        Console.ReadKey();
                        PintarEscenario(escenario, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));
                        PintarDialogo("bin/Debug/assets/Dialogos/ZaryaDialogo1-4.txt", 25, 20);
                        Console.ReadKey();
                        PintarEscenario(escenario, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));
                        PintarDialogo("bin/Debug/assets/Dialogos/ZaryaDialogo1-5.txt", 25, 20);
                        Console.ReadKey();
                        PintarEscenario(escenario, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));
                        PintarDialogo("bin/Debug/assets/Dialogos/ZaryaDialogo1-6.txt", 25, 20);
                        Console.ReadKey();
                        PintarEscenario(escenario, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));

                        Dialogos[1] = true;
                        evento = true;
                    }
                    break;
                case "Rassvet":
                    if (coordenadas[0] > 39 && coordenadas[0] < 57 && coordenadas[1] == 20 && !Dialogos[2])
                    {
                        PintarDialogo("bin/Debug/assets/Dialogos/RassvetDialogo1-1.txt", 18, 20);
                        Console.ReadKey();
                        PintarEscenario(escenario, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1)); // Le sumo +1 porque el método PintarEscenario por defecto me resta -1
                        PintarDialogo("bin/Debug/assets/Dialogos/RassvetDialogo1-2.txt", 18, 20);
                        Console.ReadKey();

                        Dialogos[2] = true;
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

                PintarEscenario(Zarya, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));
                Console.SetCursorPosition(2, 2); // Fuera del área del escenario
                Console.WriteLine($"Suministros recolectados: {SuministrosRecolectados}/{SuministrosTotales}");

                if (SuministrosRecolectados == SuministrosTotales)
                {
                    PintarEscenario(Zarya, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));
                    Console.SetCursorPosition(2, 2);
                    Console.ForegroundColor = ConsoleColor.Green; // Cambiar el color del mensaje
                    Console.Write("Aviso: ¡Todos los suministros recolectados!");
                    Console.ResetColor();
                    // Desbloquear acceso a Nauka
                    misiones[1] = true; // Suponiendo que misiones[1] controla el acceso a Nauka
                }

                evento = true; // Hubo un evento (recolectar suministro)
            }

            // Resolver acertijos en Nauka
            if (EscenarioActual == "Nauka" && celda == "?" && !misiones[2])
            {
                Console.SetCursorPosition(2, 2);
                Console.Write("Memoriza esta secuencia: ");

                Random rand = new Random();
                int[] secuencia = new int[5]; // Longitud de la secuencia

                for (int i = 0; i < secuencia.Length; i++)
                {
                    secuencia[i] = rand.Next(0, 9); // Números del 0 al 9
                    Console.Write(secuencia[i] + " ");
                }

                Thread.Sleep(2000); // Espera 3 segundos

                PintarEscenario(Nauka, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));
                Console.SetCursorPosition(2, 2);
                Console.Write("Ingresa la secuencia: ");

                string respuesta = Console.ReadLine();

                // Validar respuesta
                string secuenciaCorrecta = string.Join("", secuencia);
                if (respuesta == secuenciaCorrecta)
                {
                    PintarEscenario(Nauka, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));
                    Console.SetCursorPosition(2, 2);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("¡Correcto! Se ha desbloqueado el acceso a Rassvet.");
                    Console.ResetColor();
                    misiones[2] = true; // Desbloquear Rassvet
                }
                else
                {
                    PintarEscenario(Nauka, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));
                    Console.SetCursorPosition(2, 2);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Incorrecto. Inténtalo de nuevo.");
                    Console.ResetColor();
                }

                Console.ReadKey();
                evento = true;
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


        // M I E M B R O S   P A R A   L A S   T A R E A S   D E L   M I N I J U E G O

        // Miembros que necesita el método ManejarInterruptores
        static bool NoActualizarEscenario = false; // Conecta con Movimiento()
        static bool[] interruptores = new bool[3]; // Guarda el estado de los interruptores
        static int[] ordenActivacion = new int[3] { -1, -1, -1 }; // Guarda el orden en que se activan
        static byte siguienteInterruptor = 0; // Indica qué interruptor se debe presionar en el orden correcto
        static bool[] misiones = new bool[3];
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
                        
                        if (ordenActivacion[0] == 0 && ordenActivacion[1] == 1 && ordenActivacion[2] == 2)
                        {
                            PintarEscenario(Zvezda, (byte)(CoordPersonaje[0]+1), (byte)(CoordPersonaje[1]+1));
                            Console.SetCursorPosition(2, 2);
                            Console.ForegroundColor = ConsoleColor.Green; // Cambiar el color del mensaje
                            Console.Write("Aviso: Energía restaurada en Zvezda");
                            misiones[0] = true;
                        }
                        else
                        {
                            // Si el orden no es el correcto, reiniciar todo y permitir reintentar
                            ResetearInterruptores();
                            PintarEscenario(Zvezda, (byte)(CoordPersonaje[0] + 1), (byte)(CoordPersonaje[1] + 1));
                            Console.SetCursorPosition(2, 2);
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

        // Miembros que necesita el método EventosEscenario para el sistema de Recolectar suministros en Zarya
        public static int SuministrosRecolectados = 0; // Cantidad de suministros recolectados
        public static int SuministrosTotales = 5; // Total de suministros en Zarya (ajusta según tu mapa)


    }
    internal static class ConsoleHelper // Es una clase que se encarga de deshabilitar el Modo de Selección Rápida (QuickEdit Mode) 
    {
        const uint ENABLE_QUICK_EDIT = 0x0040;
        const uint ENABLE_EXTENDED_FLAGS = 0x0080;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        public static void DisableQuickEdit()
        {
            IntPtr consoleHandle = GetStdHandle(-10); // STD_INPUT_HANDLE
            uint consoleMode;

            if (!GetConsoleMode(consoleHandle, out consoleMode))
            {
                Console.WriteLine("Error al obtener el modo de la consola.");
                return;
            }

            consoleMode &= ~ENABLE_QUICK_EDIT; // Deshabilitar QuickEdit Mode
            consoleMode |= ENABLE_EXTENDED_FLAGS; // Habilitar flags extendidos

            if (!SetConsoleMode(consoleHandle, consoleMode))
            {
                Console.WriteLine("Error al establecer el modo de la consola.");
            }
        }
    }
    internal class PiedraPapelYTijeras
    {
        private static readonly string[] opciones = { "Piedra", "Papel", "Tijeras" };
        private static Random random = new Random();

        public static void Iniciar()
        {
            bool instruccionesMostradas = false;

            while (true)
            {
                Console.Clear();

                if (!instruccionesMostradas)
                {
                    MostrarInstrucciones();
                    instruccionesMostradas = true;
                }

                MostrarOpciones();

                int eleccionUsuario = LeerOpcionUsuario();

                string eleccionUsuarioStr = opciones[eleccionUsuario - 1];
                string eleccionComputadora = opciones[random.Next(3)];

                Console.Clear();
                Console.WriteLine($"Tú elegiste: {eleccionUsuarioStr}");
                Console.WriteLine($"La computadora eligió: {eleccionComputadora}");
                DibujarOpcion(eleccionUsuarioStr, 5, 5);
                DibujarOpcion(eleccionComputadora, 25, 5);

                MostrarResultado(eleccionUsuarioStr, eleccionComputadora);

                // Ajustar la posición del mensaje de continuación
                Console.SetCursorPosition(10, 22); // Posición más abajo
                Console.WriteLine("Presiona cualquier tecla para continuar...");
                Console.ReadKey();

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("¿DESEAS CONTINUAR JUGANDO?");
                Console.ResetColor();
                Console.WriteLine("1. Sí");
                Console.WriteLine("2. No");

                string respuestaSalir = Console.ReadLine();

                while (respuestaSalir != "1" && respuestaSalir != "2")
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("¿DESEAS CONTINUAR JUGANDO?");
                    Console.ResetColor();
                    Console.WriteLine("1. Sí");
                    Console.WriteLine("2. No");

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("¡ERROR! Ingrese una opción correcta");
                    Console.ResetColor();
                    respuestaSalir = Console.ReadLine();
                }

                if (respuestaSalir == "2")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("GRACIAS, VUELVE PRONTO!");
                    Console.ResetColor();
                    break;
                }

                instruccionesMostradas = false;
            }
        }

        private static void MostrarInstrucciones()
        {
            int startX = 5; // Ajusta la posición horizontal
            int startY = 2; // Ajusta la posición vertical

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition(startX, startY);
            Console.WriteLine("Instrucciones:");
            Console.ResetColor();

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
                Console.ForegroundColor = (ConsoleColor)(i + 2);
                Console.WriteLine(instrucciones[i]);
            }

            Console.ResetColor();
        }


        private static void MostrarOpciones()
        {
            int opcionesStartY = 10; // Ajusta la posición vertical para que esté debajo de las instrucciones

            Console.SetCursorPosition(10, opcionesStartY);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Seleccione una opción con el número correspondiente:");
            Console.ResetColor();

            // Imprime solo el texto de las opciones
            for (int i = 0; i < opciones.Length; i++)
            {
                Console.SetCursorPosition(10, opcionesStartY + 2 + i);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{i + 1}. {opciones[i]}");
            }

            // Ahora imprime los dibujos ASCII, bien separados
            DibujarOpcion("Piedra", 10, opcionesStartY + 6);
            DibujarOpcion("Papel", 35, opcionesStartY + 6);
            DibujarOpcion("Tijeras", 60, opcionesStartY + 6);
        }


        private static int LeerOpcionUsuario()
        {
            int eleccionUsuario;
            int promptY = 25; // Ajusta la posición vertical para que esté más abajo de los dibujos

            do
            {
                Console.SetCursorPosition(10, promptY);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Selecciona una opción: ");
                Console.ResetColor();

                string respuesta = Console.ReadLine();
                Console.SetCursorPosition(10, promptY);
                Console.Write(new string(' ', Console.WindowWidth - 10)); // Limpia la línea después de leer

                if (int.TryParse(respuesta, out eleccionUsuario) && eleccionUsuario >= 1 && eleccionUsuario <= 3)
                    break;
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.SetCursorPosition(10, promptY + 1);
                    Console.WriteLine("¡ERROR! Ingrese una opción correcta");
                    Console.ResetColor();
                    Console.SetCursorPosition(10, promptY + 1);
                    Console.Write(new string(' ', Console.WindowWidth - 10)); // Limpia el mensaje de error
                }

            } while (true);

            return eleccionUsuario;
        }


        private static void MostrarResultado(string eleccionUsuario, string eleccionComputadora)
        {
            int resultadoY = 20; // Posición vertical para el mensaje de resultado

            Console.SetCursorPosition(10, resultadoY); // Ajusta la posición del mensaje
            if (eleccionUsuario == eleccionComputadora)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("¡Es un empate!");
            }
            else if ((eleccionUsuario == "Piedra" && eleccionComputadora == "Tijeras") ||
                     (eleccionUsuario == "Papel" && eleccionComputadora == "Piedra") ||
                     (eleccionUsuario == "Tijeras" && eleccionComputadora == "Papel"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("¡Ganaste!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Perdiste. Inténtalo de nuevo.");
            }

            Console.ResetColor();
        }


        private static void DibujarOpcion(string opcion, int x, int y)
        {
            if (opcion == "Piedra")
            {
                // Fondo oscuro, piedra en tonos grises y detalles en blanco
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(x, y);
                Console.Write("    ▄████▄     ");
                Console.SetCursorPosition(x, y + 1);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("  ▄█      █▄   ");
                Console.SetCursorPosition(x, y + 2);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" █  ▄▄▄▄▄  █  ");
                Console.SetCursorPosition(x, y + 3);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" █ █     █ █  ");
                Console.SetCursorPosition(x, y + 4);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("▀█ █     █ █▀ ");
                Console.SetCursorPosition(x, y + 5);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("  ▀█▄▄▄▄▄█▀   ");
                Console.SetCursorPosition(x, y + 6);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("    ▀▀▀▀▀     ");
            }
            else if (opcion == "Papel")
            {
                // Fondo blanco, líneas y texto en azul y detalles en cyan
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.BackgroundColor = ConsoleColor.White;
                Console.SetCursorPosition(x, y);
                Console.Write("╔══════════╗");
                Console.SetCursorPosition(x, y + 1);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("║ ████████ ║");
                Console.SetCursorPosition(x, y + 2);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("║ ████████ ║");
                Console.SetCursorPosition(x, y + 3);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("║ ████████ ║");
                Console.SetCursorPosition(x, y + 4);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("║ ████████ ║");
                Console.SetCursorPosition(x, y + 5);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("║ ████████ ║");
                Console.SetCursorPosition(x, y + 6);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("╚══════════╝");
            }
            else if (opcion == "Tijeras")
            {
                // Fondo negro, tijeras en amarillo y detalles en rojo
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(x, y);
                Console.Write("   \\   /   ");
                Console.SetCursorPosition(x, y + 1);
                Console.Write("    \\ /    ");
                Console.SetCursorPosition(x, y + 2);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("     X     ");
                Console.SetCursorPosition(x, y + 3);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("    / \\    ");
                Console.SetCursorPosition(x, y + 4);
                Console.Write("   /   \\   ");
                Console.SetCursorPosition(x, y + 5);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("  ╔═════╗  ");
                Console.SetCursorPosition(x, y + 6);
                Console.Write("  ╚═════╝  ");
            }
            Console.ResetColor();
        }
    }

    internal class Ahorcado
    {
        private static SoundPlayer player;

        public static void Ejecutar()
        {
            ReproducirMusicaFondo();
            MostrarMenu();
        }

        static void MostrarMenu()
        {
            while (true)
            {
                Console.Clear();
                string asciiArt = @"
   ___  _           ____  __ __   ___   ____      __   ____  ___     ___      
  /  _]| |         /    ||  |  | /   \ |    \    /  ] /    ||   \   /   \     
 /  [_ | |        |  o  ||  |  ||     ||  D  )  /  / |  o  ||    \ |     |    
|    _]| |___     |     ||  _  ||  O  ||    /  /  /  |     ||  D  ||  O  |    
|   [_ |     |    |  _  ||  |  ||     ||    \ /   \_ |  _  ||     ||     |    
|     ||     |    |  |  ||  |  ||     ||  .  \\     ||  |  ||     ||     |    
|_____||_____|    |__|__||__|__| \___/ |__|\_| \____||__|__||_____| \___/     
";

                Console.WriteLine(asciiArt);
                Console.WriteLine("\nINSTRUCCIONES:");
                Console.WriteLine("- Adivina la palabra oculta antes de que se complete el ahorcado.");
                Console.WriteLine("- Ingresa una letra por turno.");
                Console.WriteLine("- Si aciertas, la letra se mostrará en su posición.");
                Console.WriteLine("- Si fallas, perderás un intento.");
                Console.WriteLine("- Pierdes si llegas a 7 errores.");
                Console.WriteLine("- Tienes una opción de ayuda, la cual te mostrará una letra, pero al utilizarla perderás un intento (la ayuda solo se puede usar 2 veces y no cuando solo falta una letra).");
                Console.WriteLine("\n1. Iniciar Juego");
                Console.WriteLine("2. Salir");
                Console.Write("Selecciona una opción: ");

                string opcion = Console.ReadLine();

                if (opcion == "1")
                    Iniciar();
                else if (opcion == "2")
                    break;
                else
                {
                    Console.WriteLine("Opción inválida. Presiona una tecla para intentar de nuevo...");
                    Console.ReadKey();
                }
            }
        }

        public static void Iniciar()
        {
            Console.Clear();
            Dictionary<string, string> palabras = new Dictionary<string, string>
        {
            { "compilador", "Traduce código fuente a código máquina." },
            { "algoritmo", "Conjunto de pasos para resolver un problema." },
            { "variable", "Espacio en memoria que almacena un valor." }
        };

            Random aleatorio = new Random();
            var palabraSeleccionada = palabras.ElementAt(aleatorio.Next(palabras.Count));
            string palabraSecreta = palabraSeleccionada.Key;
            string pista = palabraSeleccionada.Value;
            char[] palabraAdivinada = new string('_', palabraSecreta.Length).ToCharArray();
            HashSet<char> letrasIncorrectas = new HashSet<char>();
            int intentosRestantes = 7;
            int usosAyuda = 0;

            while (intentosRestantes > 0 && new string(palabraAdivinada) != palabraSecreta)
            {
                Console.Clear();
                DibujarAhorcado(7 - intentosRestantes);
                Console.WriteLine($"Pista: {pista}");
                Console.WriteLine($"Palabra: {new string(palabraAdivinada)}");
                Console.WriteLine($"Letras incorrectas: {string.Join(", ", letrasIncorrectas)}");
                Console.WriteLine($"Usos de ayuda restantes: {2 - usosAyuda}");
                Console.Write("Ingresa una letra o 'ayuda' para recibir una letra extra: ");

                string entrada = Console.ReadLine()?.Trim().ToLower();
                if (entrada == "ayuda")
                {
                    if (usosAyuda < 2 && new string(palabraAdivinada).Contains("_"))
                    {
                        if (new string(palabraAdivinada).Count(c => c == '_') == 1)
                        {
                            Console.WriteLine("No puedes usar la ayuda cuando falta solo una letra.");
                            Console.ReadKey();
                            continue;
                        }

                        usosAyuda++;
                        char letraAyuda = palabraSecreta.First(c => palabraAdivinada.Contains(c) == false);
                        for (int i = 0; i < palabraSecreta.Length; i++)
                        {
                            if (palabraSecreta[i] == letraAyuda)
                                palabraAdivinada[i] = letraAyuda;
                        }

                        intentosRestantes--;
                    }
                    else
                    {
                        Console.WriteLine("Ya has usado la ayuda dos veces o no puedes usarla ahora.");
                        Console.ReadKey();
                        continue;
                    }
                }
                else if (entrada.Length != 1 || !char.IsLetter(entrada[0]))
                {
                    Console.WriteLine("Entrada inválida. Presiona una tecla para continuar...");
                    Console.ReadKey();
                    continue;
                }
                else
                {
                    char intento = entrada[0];
                    if (palabraSecreta.Contains(intento))
                    {
                        for (int i = 0; i < palabraSecreta.Length; i++)
                            if (palabraSecreta[i] == intento)
                                palabraAdivinada[i] = intento;
                    }
                    else if (!letrasIncorrectas.Contains(intento))
                    {
                        letrasIncorrectas.Add(intento);
                        intentosRestantes--;
                    }
                }
            }

            Console.Clear();
            DibujarAhorcado(7 - intentosRestantes);
            Console.WriteLine(new string(palabraAdivinada) == palabraSecreta ?
                $"\n¡Felicidades! Has encontrado la palabra: {palabraSecreta}" :
                $"\nPerdiste. La palabra era: {palabraSecreta}");

            Console.WriteLine("\nPresiona cualquier tecla para volver al menú...");
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

        static void ReproducirMusicaFondo()
        {
            try
            {
                player = new SoundPlayer(@"C:\Users\proga\source\repos\ConsoleApp1\ConsoleApp1\musicaproyecto.wav");
                player.PlayLooping();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error al cargar la música: {e.Message}");
            }
        }
    }
    internal class FuncionalidadMouse
    {
        public static Point ObtenerPosicionMouseEnConsola()
        {
            Point posicionCursor = Cursor.Position; // Obtener la posición actual del cursor del ratón

            // Obtener la ventana de la consola
            IntPtr consoleHandle = GetConsoleWindow();
            RECT consoleRect;
            GetWindowRect(consoleHandle, out consoleRect);

            // Obtener información sobre la fuente de la consola
            CONSOLE_FONT_INFO consoleFontInfo;
            IntPtr outputHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            GetCurrentConsoleFont(outputHandle, false, out consoleFontInfo);

            // Ajuste de coordenadas basado en la posición y tamaño de la consola
            int consoleLeft = consoleRect.Left + 8; // Ajuste por borde izquierdo
            int consoleTop = consoleRect.Top + 30; // Ajuste por borde superior

            // Convertir coordenadas de pantalla a coordenadas de la consola
            return new Point(
                (posicionCursor.X - consoleLeft) / consoleFontInfo.dwFontSize.X,
                (posicionCursor.Y - consoleTop) / consoleFontInfo.dwFontSize.Y
            );
        }

        // Definiciones necesarias para obtener información de la ventana de la consola y su fuente
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetCurrentConsoleFont(IntPtr hConsoleOutput, bool bMaximumWindow, out CONSOLE_FONT_INFO lpConsoleCurrentFont);

        private const int STD_OUTPUT_HANDLE = -11;

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CONSOLE_FONT_INFO
        {
            public uint nFont;
            public COORD dwFontSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct COORD
        {
            public short X;
            public short Y;
        }
    }
}