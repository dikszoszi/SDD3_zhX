using System;
using System.Threading;
using FlowerUtils;

[assembly: CLSCompliant(false)]
namespace FlowerGarden
{
    /// <summary>
    /// Console app: the player should be controlled by a random number generator.
    /// While there is no keypress, the console screen should refresh once in every 100ms, and the player (represented by the text cursor in the console window) should be moved randomly, and a new flower should be placed once in every 5 seconds.
    /// After the loop, ask the tasks to cancel.
    /// Save the garden into the xml, and when the program restarts, load the state from the xml file.
    /// </summary>
    internal class Program
    {
        private static readonly Random Rnd = new ();

        private static void Main()
        {
            const int timeout = 100;
            int timer = 0;
            CancellationTokenSource cts = new ();
            Garden garden = new (Console.WindowHeight - 1, Console.WindowWidth - 1, cts);
            Console.CursorVisible = true;

            while (!Console.KeyAvailable)
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                garden.MovePlayer(Rnd.Next(), Rnd.Next());
                Console.WriteLine(garden.ToString());
                //Console.SetCursorPosition(garden.PlayerPosition.X, garden.PlayerPosition.Y);
                Thread.Sleep(timeout);
                timer += timeout;
                if (timer >= 5000)
                {
                    timer = 0;
                    try
                    {
                        garden.CollectFlower();
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.SetCursorPosition(0, garden.Size.Y + 1);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(ex.Message);
                        Console.ResetColor();
                        Thread.Sleep(1000);
                    }
                    finally
                    {
                        garden.PlantFlower();
                        Console.Clear();
                        Console.SetCursorPosition(0, 0);
                        Console.WriteLine(garden.ToString());
                        Thread.Sleep(timeout);
                    }
                }
            }
            garden.CancelAll();
            cts.Dispose();
            Console.Clear();
            Console.WriteLine(garden.ToString());
        }
    }
}
