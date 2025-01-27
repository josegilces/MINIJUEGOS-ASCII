using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MINIJUEGOS_ASCII
{
    /*PROYECTO FINAL - V1.0.1
         * Registro de versiones y actualizaciones
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
            Console.WriteLine("Menú:");
            Console.WriteLine("1. Laberinto");
            Console.WriteLine("2. Pidra, papel y tijeras");
            Console.WriteLine("3. El ahorcado");
            Console.WriteLine("Seleccione una opción:");

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
    }
    internal class Laberinto
    {
        //                                           X=80 Columnas y Y=25 filas
        public static string[,] Pantalla = new string[80, 25]; //Tamaño de la pantalla que usaremos, entendiendo que la pantalla en consola es una matriz (n, n)
        public static int[] CoordPersonaje = new int[2]; //Un arreglo de dos posiciones para guardar las coordenadas X y Y de nuestro personaje
        public static ConsoleKeyInfo BotonPresionado = new ConsoleKeyInfo(); //Objeto al que se le pueden asignar valores de lo que se teclea

        public static void Iniciar() //Método que "arranca" nuestro minijuego
        {
            //Ajustes previos para una mejor estética visual
            Console.CursorVisible = false; //Ocultamos el cursor para que no moleste

            //Ejecución de métodos fundamentales para el juego
            PrepararEscenario(); //Ejecutamos el método PrepararEscenario para cargar nuestro "escenario" en el arreglo Pantalla
            PintarEscenario(); //Imprimimos por primera vez nuestro escenario
            Console.SetCursorPosition(0, 0); //Colocamos el cursor invisible en la posición (0, 0) para no desorientar la pantalla

            while (true) //Ciclo while infinito que hace "correr" el juego y no se detenga
            {
                if (Console.KeyAvailable)  //Comprobamos si se lee una tecla
                {
                    Movimiento();
                }
            }
        }

        public static void PrepararEscenario() //Llenará nuestro arreglo 
        {
            Random r = new Random(); //Asignamos un objeto que nos dé un valor aleatorio (random)
            int n = r.Next(1, 4);//Calculamos un valor aleatorio de entre 1 y 4, o sea, solo nos puede dar 1, 2, 3 o 4 como resultado
            for (int x = 0; x < 25; x++)  //Ciclo for principal que recorre las posiciones de y
            {
                for (int y = 0; y < 80; y++) //Ciclo for secundario que recorrerá las posiciones de x
                {
                    Pantalla[y, x] = " "; //Asignará un espacio como valor a cada posición del arreglo
                }
            }
            //Tenemos 4 paredes, por así decirlo
            /* 
             *      3
                _______
                |      |
               1|      |2
                |______|
                    4
             */
            switch (n) //Comprobará el random usado anteriormente
            {
                //Cada caso asignará un valor inicial para nuestra figura, cada caso es una pared de la pantalla
                //Considerando la FIGURA TRAZADA ALLÁ ARRIBA
                case 1: CoordPersonaje[0] = 0; CoordPersonaje[1] = r.Next(0, 24); break;
                case 2: CoordPersonaje[0] = 79; CoordPersonaje[1] = r.Next(0, 24); break;
                case 3: CoordPersonaje[0] = r.Next(0, 79); CoordPersonaje[1] = 0; break;
                case 4: CoordPersonaje[0] = r.Next(0, 79); CoordPersonaje[1] = 24; break;
            }
            Pantalla[CoordPersonaje[0], CoordPersonaje[1]] = "O"; //El carácter que tendrá nuestra figura en el arreglo string que la diferenciará

        }

        public static void PintarEscenario() //Imprimirá en pantalla nuestro escenario que se encuentra en el arreglo Pantalla
        {
            for (int x = 0, y = 0; y < 25; x++) //Un ciclo for realmente raro porque incrementa la variable x pero la condición no la incluye
            {
                if (x != 80)//Si x no es 80
                {
                    Console.SetCursorPosition(x, y); //Colocará el cursor en la posición (x, y)
                    if (Pantalla[x, y] != "O")//Si dicha posición no contiene nuestro carácter 
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen; //El color de fondo será azul
                    }
                    else//Si sí lo contiene
                    {
                        Console.BackgroundColor = ConsoleColor.White; //Nuestro fondo será cyan
                        Console.ForegroundColor = ConsoleColor.White; //Y de igual forma los carácteres en esa posición serán cyan, para simular que no hay nada
                    }
                    Console.Write(Pantalla[x, y]); //Imprime lo que se encuentre en la posición (x, y) de nuestro arreglo
                }
                else //Cuando x sea 80, entonces
                {
                    x = -1;  //Su valor será -1, para que cuando inicie el ciclo de nuevo se convierta en 0
                    y++; //Y nuestra "y" aumentará para que el ciclo no sea inifinito
                }
            }
        }

        public static void ActualizarEscenario() //Imprime el escenario pero solamente lo que está alrededor del jugadore, esto optimiza más el rendimiento del juego
        {
            //Imprime nuestro personaje en su posición actual no si antes primero hacer unos ajustes
            Console.SetCursorPosition(CoordPersonaje[0], CoordPersonaje[1]);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(Pantalla[CoordPersonaje[0], CoordPersonaje[1]]);

            //Ahora pintamos la posición que tenia anteriormente nuestro personaje
            if (BotonPresionado.Key == ConsoleKey.DownArrow)
            {
                Console.SetCursorPosition(CoordPersonaje[0], CoordPersonaje[1] - 1); //Se ubicará y pintará la posición (x, y+1)
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.Write(Pantalla[CoordPersonaje[0], CoordPersonaje[1] - 1]);
            }
            else if (BotonPresionado.Key == ConsoleKey.UpArrow)
            {
                Console.SetCursorPosition(CoordPersonaje[0], CoordPersonaje[1] + 1); //Colocará el cursor en la posición (x, y-1)
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.Write(Pantalla[CoordPersonaje[0], CoordPersonaje[1] + 1]);
            }
            else if (BotonPresionado.Key == ConsoleKey.LeftArrow)
            {
                Console.SetCursorPosition(CoordPersonaje[0] + 1, CoordPersonaje[1]); //Se ubicará y pintará la posición (x-1, y)
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.Write(Pantalla[CoordPersonaje[0] + 1, CoordPersonaje[1]]);
            }
            else if (BotonPresionado.Key == ConsoleKey.RightArrow)
            {
                Console.SetCursorPosition(CoordPersonaje[0] - 1, CoordPersonaje[1]); //Se ubicará y pintará la posición (x+1, y)
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.Write(Pantalla[CoordPersonaje[0] - 1, CoordPersonaje[1]]);
            }
        }

        public static void Movimiento() //Se encarga de desplazar nuestro personaje
        {
            BotonPresionado = Console.ReadKey(); //Nuestra variable toma el valor de lo que hemos tecleado
            if (BotonPresionado.Key == ConsoleKey.DownArrow)  //Comprobamos si la tecla fue la flecha que va hacia abajo
            {
                //Y (MAX 25 ELEMENTOS) != 24
                if (CoordPersonaje[1] != 24)  //Comprobamos que al bajar no se exceda el tamaño de nuestra pantalla, la cual contiene nuestro escenario
                {
                    Pantalla[CoordPersonaje[0], CoordPersonaje[1]] = " "; //Si no se excede, en la posición actual se colocará un espacio vacío
                    CoordPersonaje[1]++; //Se incrementará la coordenada "y", para que baje
                    Pantalla[CoordPersonaje[0], CoordPersonaje[1]] = "O"; //Y en la nueva posición se colocará el valor de nuestra figura
                }
            }
            else if (BotonPresionado.Key == ConsoleKey.UpArrow) //Si no es la anterior, comprobaremos si la tecla es la flecha hacia arriba
            {
                if (CoordPersonaje[1] != 0) //Comprobamos que no exceda los límites y hacemos lo mismo que en la condición anterior
                {
                    Pantalla[CoordPersonaje[0], CoordPersonaje[1]] = " ";
                    CoordPersonaje[1]--; //Pero aquí restamos 
                    Pantalla[CoordPersonaje[0], CoordPersonaje[1]] = "O";
                }
            }
            else if (BotonPresionado.Key == ConsoleKey.LeftArrow)
            {
                if (CoordPersonaje[0] != 0)
                {
                    Pantalla[CoordPersonaje[0], CoordPersonaje[1]] = " ";
                    CoordPersonaje[0]--; //Lo mismo anterior, pero como se puede observar ahora usamos la posición CoordPersonaje[0], para trabajar con nuestra coordenada "x"
                    Pantalla[CoordPersonaje[0], CoordPersonaje[1]] = "O";
                }
            }
            else if (BotonPresionado.Key == ConsoleKey.RightArrow)
            {
                if (CoordPersonaje[0] != 79)
                {
                    Pantalla[CoordPersonaje[0], CoordPersonaje[1]] = " ";
                    CoordPersonaje[0]++;
                    Pantalla[CoordPersonaje[0], CoordPersonaje[1]] = "O";
                }
            }

            ActualizarEscenario(); //Volvemos a imprimir nuestro escenario usando este método
            Console.SetCursorPosition(0, 0); //Y colocamos el cursor en la posición (0, 0) para que la pantalla no se mueva
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
