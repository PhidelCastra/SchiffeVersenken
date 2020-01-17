using SchiffeVersenken.Classes.RuleSet;
using SchiffeVersenken.Data;
using SchiffeVersenken.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SchiffeVersenken.Classes
{
    class Field
    {
        /// <summary>
        /// Size of field by m*m.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Width for Quadrant -objects. 
        /// </summary>
        private int quadrantWidth = 6;

        /// <summary>
        /// Collection of all Quadrant -objects.
        /// </summary>
        public Quadrant[][] quadrants;     // Set private after Debuging!!!

        public bool AllShipsAreCovered { get; private set; }

        private Random randomObj = new Random();

        /// <summary>
        /// String message to viewable in the console.
        /// </summary>
        public Message Message { get; private set; }

        /// <summary>
        /// All Ship objects which are on the field.
        /// </summary>
        private Ship[] ships;

        /// <summary>
        /// Value to count number of inputs.
        /// </summary>
        public int InputCount { get; private set; }

        /// <summary>
        /// Value to count number of false inputs.
        /// </summary>
        public int FalseInputCount { get; private set; }

        /// <summary>
        /// Tuple is just for computer enemy. Includes hit info state. 
        /// </summary>
        public int LastMoveInfo { get; private set; }

        /* Constructor */
        public Field(int mxm)
        {
            Size = mxm;

            // Declare multidimensional Quadrant -array
            quadrants = CreateQuadrantsCollection();

            DrawField();
        }

        /// <summary>
        /// Draw m*m game field on console.
        /// </summary>
        private void DrawField()
        {
            var defaultColor = Console.BackgroundColor;

            // Upper number sequence.
            DrawUpperNumberSequence();

            // Draw matrix
            for (var ri = 0; ri < Size; ri++)
            {
                Console.WriteLine();
                Console.Write(" ");
                for (var ci = 0; ci < Size; ci++)
                {
                    quadrants[ri][ci].DrawUpperBorder();
                }
                Console.WriteLine();
                Console.Write(Constants.Alphabet[ri]);
                for (var ci = 0; ci < Size; ci++)
                {
                    quadrants[ri][ci].DrawMiddlePart();
                }
                Console.WriteLine();
                Console.Write(" ");
                for (var ci = 0; ci < Size; ci++)
                {
                    quadrants[ri][ci].DrawBottom();
                }
            }

            Console.BackgroundColor = defaultColor;
        }

        /// <summary>
        /// Initialize field once after console was started.
        /// </summary>
        /// <returns></returns>
        public void InitialzeField()
        {
            Console.Clear();

            // Draw matrix
            DrawMatrix();
        }

        /// <summary>
        /// Updates field after each shoot.
        /// Clear console at the first step. 
        /// </summary>
        /// <param name="rowHit">Row number of shoot.</param>
        /// <param name="cellHit">Cell number of shoot.</param>
        /// <returns>True if the passed coordinates was valid, otherwise false.</returns>
        public Rules.GameStates UpdateField(char rowHit, int cellHit)
        {
            InputCount++;

            // Check if the passed values are valid (not out of range).
            if(!Rules.IsCoordinateValid(rowHit, cellHit, Size))
            {
                ++FalseInputCount;

                Message = new Message(string.Format(Messages.CoordinatesNotValidMsg, rowHit, cellHit), false);
                Messages.ShowMessage(Message);

                return Rules.GameStates.MoveNoAllowed;
            }

            Message = null;
            
            var rowIndex = Array.IndexOf(Constants.Alphabet, rowHit);

            var currentQuadrant = quadrants[rowIndex][cellHit];
            currentQuadrant.State = currentQuadrant.ShipPart != null ? Rules.QuadrantIncludesHitAndShip
                : currentQuadrant.ShipPart == null ? Rules.QuadrantIncludesHit 
                : currentQuadrant.State;

            // Set it for computer enemy.
            LastMoveInfo = currentQuadrant.State;

            // Set ship part hit state to true.
            if(currentQuadrant.State == Rules.QuadrantIncludesHitAndShip)
            {
                Message = new Message(string.Format(Messages.ShipWasHitMsg, rowHit, cellHit), true);

                currentQuadrant.ShipPart.IsHit = true;
                var ship = ships.FirstOrDefault(s => s.ID == currentQuadrant.ShipPart.PartID);
                if(ship == null)
                {
                    throw new Exception($"Something is wrong! Ship with ID {currentQuadrant.ShipPart.PartID} does not exists.");
                }
                // If sunken, mark this ship.
                if (ship.IsSunk())
                {
                    foreach(var partKey in ship.Parts)
                    {
                        ship.partDict[partKey].State = Rules.QuadrantIncludesSunkShip;
                    }

                    // Just for computer enemy important.
                    LastMoveInfo = Rules.QuadrantIncludesSunkShip;

                    // Check if the game was finished.
                    AllShipsAreCovered = AllShipsAreSunk();
                    if (AllShipsAreCovered )
                    {
                        Message = new Message(Messages.AllShipsAreSunkenMsg, true);
                        Messages.ShowMessage(Message);
                        Console.WriteLine($"Try count:{InputCount}");
                        return Rules.GameStates.GameIsFinished;
                    }

                    Message = new Message(string.Format(Messages.ShipIsSunkenMsg, ship.ID, ship.Length()), true);
                }
            }

            // Clear console.
             Console.Clear();

            // Draw matrix
            DrawMatrix();
            return Rules.GameStates.MoveIsAllowed;
        }

        public void ShowField(bool clearConsole)
        {
            if (clearConsole)
            {
                Console.Clear();
            }

            DrawMatrix();
        }

        /// <summary>
        /// Draw field matrix with all quadrants and marked quadrant -states.
        /// </summary>
        private void DrawMatrix()
        {
            // Upper number sequence.
            DrawUpperNumberSequence();

            for (var ri = 0; ri < Size; ri++)
            {
                Console.WriteLine();
                Console.Write(" ");
                for (var ci = 0; ci < Size; ci++)
                {
                    quadrants[ri][ci].DrawUpperBorder();
                }
                Console.WriteLine();
                Console.Write(Constants.Alphabet[ri]);
                for (var ci = 0; ci < Size; ci++)
                {
                    quadrants[ri][ci].DrawMiddlePart();
                }
                Console.WriteLine();
                Console.Write(" ");
                for (var ci = 0; ci < Size; ci++)
                {
                    quadrants[ri][ci].DrawBottom();
                }
            }

            if (Message != null)
            {
                Messages.ShowMessage(Message);
            }
        }

        /// <summary>
        /// Distribute Ship -objects random of field.
        /// Set passing ships in the corresponding array of this field object.
        /// </summary>
        /// <param name="ships">todo: describe ships parameter on SetShips</param>
        public void SetShips(Ship[] ships)
        {
            AllShipsAreCovered = ships.Length > 0;

            ships.OrderBy(s => s.Length());
            foreach(var s in ships)
            {
                Console.WriteLine($"Set ship with length:{s.Length()}.");
                SetShip(s);
            }

            this.ships = ships; 
        }

        /// <summary>
        /// Check if all ships on field are sunk (check all ship parts of ship objects for 'IsHit' -boolean).
        /// </summary>
        /// <returns>True if all ship parts of all ships are sunk, otherwise false.</returns>
        private bool AllShipsAreSunk()
        {
            foreach(var ship in ships)
            {
                if(ship.Parts.FirstOrDefault(p => !p.IsHit) != null)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Set a Ship -object random at the game field. Calculate for each Ship QuadrantIndices which are not occupied.
        /// </summary>
        /// <param name="ship">Ship which should set to the field.</param>
        private bool SetShip(Ship ship)
        {
            // Search all possible Quadrant series for passed ship, independent from length -value.
            var horizontalSeries = GetFreeQuadrantsFor(ship, true);
            var verticalSeries = GetFreeQuadrantsFor(ship, false);
            var allFreeQuadrantSeries = verticalSeries.Concat(horizontalSeries).ToArray();

            var randomSerieNr = randomObj.Next(0, allFreeQuadrantSeries.Count());
            if(allFreeQuadrantSeries.Length == 0) { Console.WriteLine($"Is not possible to set ship with length of {ship.Length()}."); return false; }
            var finSerie = allFreeQuadrantSeries[randomSerieNr];

            // Set ship parts.
            for(var i = 0; i < finSerie.Length; i++)
            {
                finSerie[i].ShipPart = ship.Parts[i];
                ship.partDict.Add(ship.Parts[i], finSerie[i]);
            }
            return true;
        }

        private bool IsQuadrantAvailable(QuadrantIndex index)
        {
            return quadrants[index.GetRowIndexAsNumber()][index.CellIndex].ShipPart != null ? false : true;
        }

        private void SetFinishSettings()
        {
            // Not implemented yet....
        }

        /// <summary>
        /// Draw upper numbers in top of the field.
        /// </summary>
        private void DrawUpperNumberSequence()
        {
            for (var ei = 0; ei < (quadrantWidth / 2); ei++) { Console.Write(" "); }
            for (var nr = 0; nr < Size; nr++)
            {
                Console.Write($" {nr}");
                for (var i = 0; i < quadrantWidth; i++)
                {
                    Console.Write(" ");
                }
            }
        }

        private Quadrant[][] CreateQuadrantsCollection()
        {
            var quadrants = new Quadrant[Size][];
            for (var i = 0; i < quadrants.Length; i++) { quadrants[i] = new Quadrant[Size]; }

            for (var r = 0; r < Size; r++)
            {
                for (var c = 0; c < Size; c++)
                {
                    quadrants[r][c] = new Quadrant(quadrantWidth, new Structures.QuadrantIndex(Constants.Alphabet[r], c));
                }
            }

            return quadrants;
        }

        private bool IsNaighborIndexFree(QuadrantIndex index)
        {
            if (quadrants.Length > index.GetRowIndexAsNumber())
            {
                // DEBUG::
                var r1 = index.GetRowIndexAsNumber() + 1;
                var r2 = index.GetRowIndexAsNumber() - 1;

                var e1 = (index.GetRowIndexAsNumber() + 1) < Size && quadrants[index.GetRowIndexAsNumber() + 1][index.CellIndex].ShipPart != null;
                var e2 = (index.GetRowIndexAsNumber() - 1) >= 0 && quadrants[index.GetRowIndexAsNumber() - 1][index.CellIndex].ShipPart != null;
                var e3 = (index.CellIndex + 1) < Size && quadrants[index.GetRowIndexAsNumber()][index.CellIndex + 1].ShipPart != null;
                var e4 = (index.CellIndex - 1) >= 0 && quadrants[index.GetRowIndexAsNumber()][index.CellIndex - 1].ShipPart != null;
                var preconditionsList = new List<bool> { e1, e2, e3, e4 };
                var foreignShipInTheNear = 0;

                foreach (var state in preconditionsList)
                {
                    if (state) { foreignShipInTheNear++; }
                    if (foreignShipInTheNear > 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private List<Quadrant[]> GetFreeQuadrantsFor(Ship ship, bool horizontal)
        {
            var shipQuadrants = new List<Quadrant[]>();
            for(var row = 0; row < Size; row++)
            {
                for(var cell = 0; cell < Size; cell++)
                {
                    // Reset series array and counter.
                    var quadrantsSerie = new Quadrant[ship.Length()];
                    var serieCounter = 0;
                    var currentMaxLength = cell + ship.Length() + 1;
                    if(currentMaxLength > Size)
                    {
                        break;
                    }
                    for (var hit = cell; hit < currentMaxLength; hit++)
                    {   
                        // Get current Quadrant and check for availability.
                        var currentQuadrant = horizontal == true ? quadrants[row][hit] : quadrants[hit][row];
                        if(!IsQuadrantAvailable(currentQuadrant.GetIndex()) || !IsNaighborIndexFree(currentQuadrant.GetIndex()))
                        {
                            break;
                        }
                        // If all queries are true....
                        if(serieCounter == ship.Length())
                        {
                            shipQuadrants.Add(quadrantsSerie);
                            break; 
                        }                    
                        quadrantsSerie[serieCounter] = currentQuadrant;
                        serieCounter++;
                    }
                }
            }
            return shipQuadrants;
        }

        /// <summary>
        /// Vieables string on console. 
        /// Changes text color before it displays and after this changes color back again to same color like before...
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="textColor"></param>
        private void DisplaysMessage(string msg, ConsoleColor textColor)
        {
            var colorMemory = Console.ForegroundColor;
            Console.ForegroundColor = textColor;
            Console.WriteLine(msg);
            Console.ForegroundColor = colorMemory;
        }

        public int ConvertRowToNumber(char rowName)
        {
            return Array.IndexOf(Constants.Alphabet, rowName);
        }
        public char ConvertRowToChar(int rowNumber)
        {
            return Constants.Alphabet[rowNumber];
        }
    }
}
