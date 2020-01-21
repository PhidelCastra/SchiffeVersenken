using SchiffeVersenken.Classes.RuleSet;
using SchiffeVersenken.Classes.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchiffeVersenken.Data;

namespace SchiffeVersenken.Classes
{
    class Game
    {
        // True if a running game should be finish, otherwise false.
        bool roundEnds;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="gameNr">0 for single player, 1 for vs.computer, 2 vs.other player.</param>
        public Game(int gameNr)
        {
            switch (gameNr)
            {
                case 0:
                    InitializeSingleGame();
                    break;
                case 1:
                    InitializeGameVSComputer();
                    break;
                case 2:
                    InitializeMultiplayer();
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"The game number {gameNr} is not available.");
            }
        }

        private void InitializeSingleGame()
        {
            var ships = new Ship[] { new Ship(5), new Ship(4), new Ship(4), new Ship(3), new Ship(3), new Ship(3), new Ship(2), new Ship(2), new Ship(2), new Ship(2) };
            var field = InitializeField(10, ships);

            while (!roundEnds)
            {
                Console.WriteLine();
                Console.WriteLine("Drücke Taste ESC um Programm zu beenden...");
                Console.Write("Eingabe Zeile: ");
                var input = Console.ReadKey(true);
                if (input.Key == ConsoleKey.Escape)
                {
                    roundEnds = true;
                    continue;
                }
                var rowInput = char.ToUpper(input.KeyChar);

                Console.Write($", Row:'{rowInput}' , Cell:");
                var cellInput = ConvertStringToNumber(Console.ReadLine());

                var currentGameState = field.UpdateField(rowInput, cellInput);
                
                // Check game state and set roundends true if state says game is fin.
                IsGameFin(currentGameState, field, rowInput, cellInput);
            }
        }

        private void InitializeGameVSComputer()
        {
            // Initialize player field.
            var ownShips = new Ship[] { new Ship(5), new Ship(4), new Ship(4), new Ship(3), new Ship(3), new Ship(3), new Ship(2), new Ship(2), new Ship(2), new Ship(2) };
            var ownField = InitializeField(10, ownShips);

            // Initialize computer enemy field.
            var cShips = new Ship[] { new Ship(5), new Ship(4), new Ship(4), new Ship(3), new Ship(3), new Ship(3), new Ship(2), new Ship(2), new Ship(2), new Ship(2) };
            var enemyField = InitializeField(10, cShips);

            var enemy = new Enemy(ownField);
            enemy.PlayerName = "Shrimply Pibbels";

            var playerIs = true;

            while (!roundEnds)
            {
                if (playerIs)
                {
                    // Own move.
                    Console.WriteLine();
                    Console.WriteLine("Press ESC to quit game...");
                    Console.WriteLine("Your move!");
                    Console.WriteLine("Input row: ");
                    var input = Console.ReadKey(true);
                    if (input.Key == ConsoleKey.Escape)
                    {
                        roundEnds = true;
                        continue;
                    }
                    var rowInput = char.ToUpper(input.KeyChar);

                    Console.WriteLine($"Row: '{rowInput}'");
                    Console.Write($"Cell: ");
                    var cellInput = ConvertStringToNumber(Console.ReadLine());

                    // Computer moves.
                    var currentGameState = enemyField.UpdateField(rowInput, cellInput);
                    var playerWins = IsGameFin(currentGameState, enemyField, rowInput, cellInput);
                    if (playerWins)
                    {
                        var lostMsg = new Message(string.Format(Messages.PlayerHasWonMsg, enemy.PlayerName), true);
                        Messages.ShowMessage(lostMsg);
                        roundEnds = true;
                        continue;
                    }

                    playerIs = false;
                    Console.WriteLine("Press any key for computer move...");
                    Console.ReadKey();
                    continue;
                }
                //// DELETE IT LATER.....
                //enemy.field.ShowField(true);

                var move = enemy.MakeMove();
                var enemyHasWon = IsGameFin(enemy.GameState, ownField, move.Item1, move.Item2);
                if (enemyHasWon)
                {
                    roundEnds = true;
                    var lostMsg = new Message(string.Format(Messages.PlayerHasLost, enemy.PlayerName), false);
                    Messages.ShowMessage(lostMsg);
                }
                var defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"'{enemy.PlayerName}' has set to '{move.Item1}{move.Item2}' to your field.");
                Console.ForegroundColor = defaultColor;

                Console.WriteLine("Press any key for your move...");
                Console.ReadKey(true);
                Console.Clear();

                playerIs = true;
                enemyField.ShowField(true);
            }

            // End informations.
            var playerInfo = string.Format(Messages.PlayerEndInformations, enemyField.InputCount, enemyField.FalseInputCount);
            Console.WriteLine(playerInfo);
            Console.WriteLine();
            Console.WriteLine("Press any key to get back to menu...");
            Console.ReadKey();
        }
        private void InitializeMultiplayer()
        {
            // Comming soon...
        }

        private Field InitializeField(int size, Ship[] ships)
        {
            var field = new Field(size);
            field.SetShips(ships);
            field.InitialzeField();

            roundEnds = false;

            return field;
        }

        /// <summary>
        /// Managed current game states (End of game, move not allowed) and set corresponding values.
        /// </summary>
        /// <param name="state">The current game state.</param>
        /// <param name="row">Last choosen row char.</param>
        /// <param name="cell">Last choosen cell number.</param>
        /// <returns>True if the game state has corresponding value for game fin, otherwise null.</returns>
        private bool IsGameFin(Rules.GameStates state, Field field, char row, int cell)
        {
            if (state == Rules.GameStates.GameIsFinished)
            {
                field.ShowField(true);

                Console.Write("Press key to return to menu... ");
                Console.ReadKey();
                return true;
            }
            else if (state != Rules.GameStates.MoveNoAllowed)
            {
                var bgMemory = Console.BackgroundColor;
                var letterMemory = Console.ForegroundColor;

                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkRed;

                Console.WriteLine($"Added '{row}{cell}'");

                Console.BackgroundColor = bgMemory;
                Console.ForegroundColor = letterMemory;
            }

            return false;
        }

        static int ConvertStringToNumber(string nr)
        {
            int n;
            if (!int.TryParse(nr, out n))
            {
                return -1;
            }

            return n;
        }
    }
}
