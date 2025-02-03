using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.LinkLabel;


namespace MINIJUEGOS_ASCII
{
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
         *          Escenario1 de tipo string
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
    internal class Menú
    {
        static void Main(string[] args)
        {
            //MaximizarVentana();

            Console.WriteLine("Menú:");
            Console.WriteLine("1. Laberinto");
            Console.WriteLine("2. Pidra, papel y tijeras");
            Console.WriteLine("3. El ahorcado");
            Console.WriteLine("Seleccione una opción:");

            //Info.de desarrollador
            Console.SetBufferSize(120, 35);
            //Console.WriteLine($"Buffer: {Console.BufferWidth}x{Console.BufferHeight}");

            var Opcion = Console.ReadLine();

            switch (Opcion)
            {
                case "1":
                    Laberinto.Iniciar();
                    break;
                case "2":
                    PiedraPapelYTijeras.Iniciar();
                    break;
                case "3":
                    // Lógica para el Minijuego 3
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }
        static void MaximizarVentana() //Ajustar el tamaño de la ventana de la consola al máximo tamaño posible
        {
            int maxColumns = Console.LargestWindowWidth;
            int maxRows = Console.LargestWindowHeight;

            Console.SetWindowSize(maxColumns, maxRows);
            Console.SetBufferSize(maxColumns, maxRows);
        }
    }
    internal class Laberinto
    {
        //MIEMBROS FUNDAMENTALES PARA EL JUEGO
        //                                           X=100 Columnas y Y=35 filas
        public static int[] CoordPersonaje = new int[2]; //Un arreglo de dos posiciones para guardar las coordenadas X y Y de nuestro personaje
        public static ConsoleKeyInfo BotonPresionado = new ConsoleKeyInfo(); //Objeto al que se le pueden asignar valores de lo que se teclea

        //Escenarios
        public static string[,] Escenario1 = new string[100, 35];

        public static void Iniciar() //Método que "arranca" nuestro minijuego
        {
            //Ajustes previos para una mejor estética visual
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false; //Ocultamos el cursor para que no moleste

            //Ejecución de funciones fundamentales para el juego
            PrepararEscenario("escenario1.txt", Escenario1); //Ejecutamos el método PrepararEscenario para cargar nuestro "escenario" en el arreglo Pantalla
            PintarEscenario(Escenario1, 50, 0); //Imprimimos por primera vez nuestro escenario

            while (true) //Ciclo while infinito que hace "correr" el juego y no se detenga
            {
                if (Console.KeyAvailable)  //Comprobamos si se lee una tecla
                {
                    Movimiento(Escenario1);
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
        public static void PintarEscenario(string[,] escenario, int xP, int yP) //Imprimirá en pantalla nuestro escenario que se encuentra en el arreglo Pantalla
        {
            CoordPersonaje[0] = xP; CoordPersonaje[1] = yP; //Establece la posición de nuestro personaje

            for (int y = 0; y < 35; y++)
            {
                Console.SetCursorPosition(0, y);
                for (int x = 0; x < 100; x++)
                {
                    Console.Write(escenario[x, y]);
                }
            }
            Console.SetCursorPosition(xP, yP);
            Console.Write("P");


        }
        public static void Movimiento(string[,] escenario)
        {
            BotonPresionado = Console.ReadKey(true); // Evita que se muestre la tecla presionada

            int xAnterior = CoordPersonaje[0];
            int yAnterior = CoordPersonaje[1]; // Guarda la posición actual antes de moverse

            if (BotonPresionado.Key == ConsoleKey.DownArrow && yAnterior < 49 && EsPosicionValida(escenario, new int[] { xAnterior, yAnterior + 1 }))
            {
                CoordPersonaje[1]++;
            }
            else if (BotonPresionado.Key == ConsoleKey.UpArrow && yAnterior > 0 && EsPosicionValida(escenario, new int[] { xAnterior, yAnterior - 1 }))
            {
                CoordPersonaje[1]--;
            }
            else if (BotonPresionado.Key == ConsoleKey.LeftArrow && xAnterior > 0 && EsPosicionValida(escenario, new int[] { xAnterior - 1, yAnterior }))
            {
                CoordPersonaje[0]--;
            }
            else if (BotonPresionado.Key == ConsoleKey.RightArrow && xAnterior < 99 && EsPosicionValida(escenario, new int[] { xAnterior + 1, yAnterior }))
            {
                CoordPersonaje[0]++;
            }

            ActualizarEscenario(escenario, xAnterior, yAnterior); // Enviamos la posición anterior para que se restaure bien
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
    }

    internal class PiedraPapelYTijeras
    {
        public static ConsoleKeyInfo BotonPresionado = new ConsoleKeyInfo();
        public static void Iniciar()
        {
            Console.Clear();
            // Asegúrate de que la aplicación tenga un contexto de Windows Forms
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Obtener la posición actual del cursor del mouse
            Point posicionCursor = new Point();

            Console.WriteLine("Seleccione una opción:");
            Console.SetCursorPosition(10, 10);
            Console.Write("PIEDRA     PAPEL     TIJERA");

            while (true)
            {
                posicionCursor = Cursor.Position;

                // Convertir las coordenadas de pantalla a coordenadas de consola
                Point posicionConsola = new Point(
                    posicionCursor.X / Console.LargestWindowHeight,
                    posicionCursor.Y / Console.LargestWindowHeight
                );

                // Mostrar la posición del cursor
                Console.WriteLine("Posición del cursor: X = " + posicionConsola.X + ", Y = " + posicionConsola.Y);
                //if (posicionConsola.X == 10 && posicionConsola.Y == 10)
                //{
                //    Console.WriteLine("EUREKA");
                //}
            }
        }
    }

}
