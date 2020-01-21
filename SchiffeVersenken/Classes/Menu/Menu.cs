using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SchiffeVersenken.Classes.Menu
{
    class Menu
    {
        /// <summary>
        /// Module parts for pointer/arrow. 
        /// For drawing in view.
        /// </summary>
        private readonly string UpperArrowPart = "______|\\ ";
        private readonly string MiddleArrowPart = "|       >";
        private readonly string BottomArrowPart = "¯¯¯¯¯¯|/ ";

        /// <summary>
        /// Module parts to draw start option in console.
        /// </summary>
        private readonly string UpperStartBtnPart = " /¯¯¯¯||¯¯¯¯¯¯| /¯¯¯¯¯¯||¯¯¯¯¯||¯¯¯¯¯¯|";
        private readonly string MiddleStartBtnPart = "|   ¯¯| ¯|  |¯ |  |¯|  ||  || | ¯|  |¯ ";
        private readonly string BottomStartBtnPart = "|¯¯  /   |__|  |__|¯|__||__|\\_\\  |__|  ";

        private readonly string UpperVSComputerPart = "\\¯\\  /¯/ /¯¯¯¯|  ____    |¯¯\\   /¯¯||¯¯¯¯¯¯||¯¯¯¯¯\\|¯¯¯¯¯|";
        private readonly string MiddleVSComputerPart = " \\ \\/ / |   ¯¯| |____|   |   \\_/   || |¯¯| || |¯¯\\||  ¯¯¯|";
        private readonly string BottomVSComputerPart = "  \\__/  |¯¯  /           |__|___|__||______||  ¯¯¯/|  ¯¯¯|";

        /// <summary>
        /// Module parts to draw info option in console.
        /// </summary>
        private readonly string upperInfoBtnPart =  "|¯¯||¯¯¯¯|¯¯||¯¯¯¯¯||¯¯¯¯¯¯|";
        private readonly string middleInfoBtnPart = "|  ||  | |  ||  ¯¯¯|| |¯¯| |";
        private readonly string bottomInfoBtnPart = "|__||__|____||__|¯¯ |______|";

        /// <summary>
        /// Module parts to draw quit option in console.
        /// </summary>
        private readonly string UpperEndBtnPart = "|¯¯¯¯¯||¯¯¯¯|¯¯||¯¯¯¯¯\\";
        private readonly string MiddleEndBtnPart = "|  ¯¯¯||  | |  || |¯¯\\|";
        private readonly string BottomEndBtnPart = "|  ¯¯¯||__|____||  ¯¯¯/";

        private readonly ConsoleColor arrowColor = ConsoleColor.DarkRed;
        private readonly ConsoleColor moduleColor = ConsoleColor.DarkGreen;
        private readonly ConsoleColor defaultColor = ConsoleColor.Black;

        /// <summary>
        /// Time to close program after exit.
        /// </summary>
        private readonly int exitTime = 2000;

        public enum States { Start, VS, Info, Exit }
        public States CurrentState { get; private set; }

        private string[] arrow;

        private string[,] modules;

        private bool pressedEnter;

        /// <summary>
        /// Pointer which mark a index of modules array.
        /// </summary>
        private int pointer;

        /* Constructor */
        public Menu()
        {
            pressedEnter = false;

            arrow = new string[3];
            arrow[0] = UpperArrowPart;
            arrow[1] = MiddleArrowPart;
            arrow[2] = BottomArrowPart;

            modules = new string[,] { 
                { UpperStartBtnPart, MiddleStartBtnPart, BottomStartBtnPart }, 
                { UpperVSComputerPart, MiddleVSComputerPart, BottomVSComputerPart },
                { upperInfoBtnPart, middleInfoBtnPart, bottomInfoBtnPart}, 
                { UpperEndBtnPart, MiddleEndBtnPart, BottomEndBtnPart }
            };
        }

        public void ProcessInput(ConsoleKey input)
        {
            switch (input)
            {
                case ConsoleKey.UpArrow:
                    Update(--pointer);
                    break;
                case ConsoleKey.DownArrow:
                    Update(++pointer);
                    break;
                case ConsoleKey.Enter:
                    if(CurrentState == States.Start)
                    {
                        pressedEnter = true;
                        CloseMenu();
                    }
                    else if(CurrentState == States.VS)
                    {
                        pressedEnter = true;
                        CloseMenu();
                    }
                    else if(CurrentState == States.Info)
                    {
                        Console.WriteLine("Info: not implemented yet...");
                    }
                    else if(CurrentState == States.Exit) {
                        Console.WriteLine("Quit program...");
                        Thread.Sleep(exitTime);
                        Environment.Exit(0); 
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Update pointer and console view - Change arrow to next module.
        /// </summary>
        /// <param name="points">Next pointer position.</param>
        public void Update(int points)
        {
            SetPointer(points);

            Console.Clear();

            var arrowCharSize = arrow[0].Length;

            // First array dimension...
            for (var i = 0; i < modules.GetLongLength(0); i++)
            {
                // ...second array dimension.
                for (var j = 0; j < modules.GetLength(1); j++)
                {
                    // Draw 'Intro' (arrow or spacing) part before drawing module part.
                    for(var spacing = 0; spacing < arrowCharSize; spacing++)
                    {
                        if (pointer == i)
                        {
                            Console.Write(arrow[j][spacing]);
                            continue;
                        }
                        Console.Write(' ');
                    }

                    DrawModule(i, j);
                }
            }
        }
        
        /// <summary>
        /// Set pointer on corresponding index of modules array.
        /// </summary>
        /// <param name="position">Should be the changed pointer index.</param>
        private void SetPointer(int position)
        {
            var modLength = (modules.GetLength(0) - 1);

            pointer = position > modLength ? 0
                : position < 0 ? modLength
                : position;

            // Set corresponding State value.
            CurrentState = pointer == 0 ? States.Start
                : pointer == 1 ? States.VS
                : pointer == 2 ? States.Info
                : States.Exit;
        }

        public bool GetPressEnter() {
            var pe = pressedEnter;
            pressedEnter = false;

            return pe;
        }

        private void CloseMenu()
        {
            Console.Clear();
        }

        private void DrawModule(int moduleNr, int modulePart)
        {
            for(var m = 0; m < modules[moduleNr, modulePart].Length; m++)
            {
                var ch = modules[moduleNr, modulePart][m];
                if(ch == '|' || ch == '¯' || ch == ' ')
                {
                    //Console.BackgroundColor = defaultColor;
                    Console.Write(ch);
                    continue;
                }
                //Console.BackgroundColor = defaultColor;
                Console.Write(ch);
            }
            // Console.WriteLine(modules[moduleNr, modulePart]);
            //Console.BackgroundColor = defaultColor;
            Console.WriteLine();
        }

        private void DrawInfoView()
        {
            Console.Clear();
            Console.WriteLine("");
        }
    }
}
