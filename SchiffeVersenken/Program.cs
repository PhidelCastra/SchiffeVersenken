using SchiffeVersenken.Classes;
using SchiffeVersenken.Classes.Menu;
using SchiffeVersenken.Classes.RuleSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchiffeVersenken
{
    class Program
    {
        static void Main(string[] args)
        {
            var menuShouldInitialize = true;
            var roundEnds = true;

            Field field1 = null;

            Console.WindowHeight = 50;
            var menu = new Menu();

            while (menu.CurrentState != Menu.States.Exit || !menu.GetPressEnter())
            {
                // Draw area...
                if (menuShouldInitialize)
                {
                    menu.Update(0);
                    menuShouldInitialize = false;
                }

                var k = Console.ReadKey();
                menu.ProcessInput(k.Key);
                
                if(menu.CurrentState == Menu.States.Start && menu.GetPressEnter())
                {
                    field1 = new Field(10);
                    var shipsToSet = new Ship[] { new Ship(5), new Ship(4), new Ship(4), new Ship(3), new Ship(3), new Ship(3), new Ship(2), new Ship(2), new Ship(2), new Ship(2) };
                    field1.SetShips(shipsToSet);
                    field1.InitialzeField();

                    roundEnds = false;
                }

                else if (menu.CurrentState == Menu.States.VS && menu.GetPressEnter())
                {
                    var game = new Game(1);
                    menuShouldInitialize = true;
                }

                // Play loop.
                while (!roundEnds)
                {
                    Console.WriteLine();
                    Console.WriteLine("Drücke Taste ESC um Programm zu beenden...");
                    Console.Write("Eingabe Zeile: ");
                    var input = Console.ReadKey(true);
                    if (input.Key == ConsoleKey.Escape)
                    {
                        roundEnds = true;

                        menuShouldInitialize = true;

                        continue;
                    }

                    var rowInput = char.ToUpper(input.KeyChar);

                    Console.Write($", Row:'{rowInput}' , Cell:");
                    var cellInput = ConvertStringToNumber(Console.ReadLine());

                    var currentGameState = field1.UpdateField(rowInput, cellInput);
                    if (currentGameState == Rules.GameStates.GameIsFinished)
                    {
                        roundEnds = true;

                        menuShouldInitialize = true;

                        Console.Write("Press key to return : ");
                        Console.WriteLine();
                    }
                    else if (currentGameState != Rules.GameStates.MoveNoAllowed)
                    {
                        var bgMemory = Console.BackgroundColor;
                        var letterMemory = Console.ForegroundColor;

                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.DarkRed;

                        Console.WriteLine($"Added '{rowInput}{cellInput}'");

                        Console.BackgroundColor = bgMemory;
                        Console.ForegroundColor = letterMemory;
                    }
                }
            }
        }


        static int ConvertStringToNumber(string nr)
        {
            int n;
            if(!int.TryParse(nr, out n))
            {
                return -1;
            }

            return n;
        }
    }
}

