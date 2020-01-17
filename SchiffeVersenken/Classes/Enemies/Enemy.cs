using SchiffeVersenken.Classes.RuleSet;
using SchiffeVersenken.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchiffeVersenken.Classes.Enemies
{
    class Enemy
    {
        /// <summary>
        /// Field of the player (My field - not the field of enemy).
        /// </summary>
        public Field field { get; private set; }

        /// <summary>
        /// Number of rows/columns.
        /// </summary>
        private int size;

        /// <summary>
        /// Should be includes all possible Tuples, with row and cell -numbers, which can set.
        /// </summary>
        Tuple<int, int>[] possibleMoves;

        /// <summary>
        /// Length of possible moves -array before the first move.
        /// </summary>
        int startLength;

        /// <summary>
        /// Random -object to get random numbers for next move (cell and row).
        /// </summary>
        Random moveCreator = new Random(DateTime.Now.Millisecond);

        public Rules.GameStates GameState { get; private set; }

        /// <summary>
        /// True if a ship is hit, otherwise false also if a ship is sunken.
        /// </summary>
        private bool searchForNextHit;

        /// <summary>
        /// First hit coordinate of a ship. 
        /// </summary>
        private Tuple<int, int> originShipHit;
        private Tuple<int, int> currentHitField;
        private Tuple<int, int> neighborField;

        bool originNeighborsDeleted;

        /// <summary>
        /// 0.Default
        /// 1.Horizontal
        /// 2.Vertical
        /// </summary>
        private int direction;

        private bool? isMoveRight;
        private bool? isMoveUp;

        private int lastMoveInfo;

        /// <summary>
        /// Should be true if at the players field are all ships sunken, otherwise false.
        /// </summary>
        public bool HasWon { get; private set; }

        public Enemy(Field playerField)
        {
            field = playerField;

            size = field.Size;

            startLength = field.quadrants.Length * field.quadrants.Length;

            possibleMoves = new Tuple<int, int>[startLength];
            var counter = 0;
            for(var row = 0; row < size; row++)
            {
                for(var cell = 0; cell < size; cell++)
                {
                    possibleMoves[counter] = Tuple.Create(row, cell);
                    counter += 1;
                }
            }
        }

        List<Tuple<int, int>> parts = new List<Tuple<int, int>>();
        int lastLastMoveInfo;
        /// <summary>
        /// Set a move dependent from the random result and remaining moves.
        /// Updates also the field.
        /// </summary>
        public Tuple<char, int> MakeMove()
        {
            // Values to show on console.
            char finRow = ' ';
            int finCell = -1;

            if (lastMoveInfo == Rules.QuadrantIncludesHitAndShip)
            {
                searchForNextHit = true;
                originShipHit = originShipHit == null ? currentHitField : originShipHit;

                if (neighborField != null)
                {
                    if (originShipHit.Item1 > neighborField.Item1)
                    {
                        direction = 2;
                        isMoveUp = true;
                    }
                    else if(originShipHit.Item1 < neighborField.Item1) 
                    {
                        direction = 2;
                        isMoveUp = false;
                    }
                    else if(originShipHit.Item2 > neighborField.Item2)
                    {
                        direction = 1;
                        isMoveRight = false;
                    }
                    else if(originShipHit.Item2 < neighborField.Item2)
                    {
                        direction = 1;
                        isMoveRight = true;
                    }

                    currentHitField = neighborField;
                }

                // Collect fields to remove neighbors from moves.
                if (!parts.Contains(currentHitField))
                {
                    parts.Add(currentHitField);
                }

                neighborField = GetNextMoveAfterHit(currentHitField);
                var indexToDelete = Array.IndexOf(possibleMoves, neighborField);
                possibleMoves[indexToDelete] = null;
                UpdateMoveArray();

                var rowChar = field.ConvertRowToChar(neighborField.Item1);
                var cell = neighborField.Item2;
                GameState = field.UpdateField(rowChar, cell);

                lastLastMoveInfo = lastMoveInfo;
                lastMoveInfo = field.LastMoveInfo;

                finRow = field.ConvertRowToChar(currentHitField.Item1);
                finCell = currentHitField.Item2;
            }

            // If the last shot wasn´t a hit.
            else if (!searchForNextHit || lastMoveInfo == Rules.QuadrantIncludesSunkShip)
            {
                if(parts != null && parts.Count > 0)
                {
                    parts.Add(neighborField);       // Add also the last hit field.
                    parts.ForEach(n => RemoveNeighbors(n)); 
                }

                // Reset all hit properties.
                parts = new List<Tuple<int, int>>();
                searchForNextHit = false;
                neighborField = null;
                originShipHit = null;
                direction = 0;
                originNeighborsDeleted = false;

                // Choose new random index.
                var index = moveCreator.Next(0, possibleMoves.Length);
                
                // Find tupel and set index null.
                currentHitField = possibleMoves[index];
                possibleMoves[index] = null;

                // Remove null -indicies.
                UpdateMoveArray();

                var rowChar = Constants.Alphabet[currentHitField.Item1];
                GameState = field.UpdateField(rowChar, currentHitField.Item2);
                lastMoveInfo = field.LastMoveInfo;
                if (GameState == Rules.GameStates.GameIsFinished)
                {
                    HasWon = true; 
                }

                return Tuple.Create(rowChar, currentHitField.Item2);
            }
            else if (searchForNextHit) 
            {
                // Get neighbor.
                neighborField = GetNextMoveAfterHit(currentHitField);
                var indexToDelete = Array.IndexOf(possibleMoves, neighborField);
                possibleMoves[indexToDelete] = null;
                UpdateMoveArray();
                
                // Update field.
                var r = field.ConvertRowToChar(neighborField.Item1);
                var c = neighborField.Item2;
                GameState = field.UpdateField(r, c);
                lastMoveInfo = field.LastMoveInfo;

                finRow = field.ConvertRowToChar(neighborField.Item1);
                finCell = neighborField.Item2;
            }
            
            return Tuple.Create(finRow, finCell);
        }

        /// <summary>
        /// Managed the "possible moves" -array.
        /// Search for null value -indicies, count them and create new array without null -indicies and set as new "possible move" -array.
        /// Result is an array with reduced length and without null -indicies.
        /// </summary>
        private void UpdateMoveArray() {
            possibleMoves = possibleMoves.AsEnumerable().Where(m => m != null).ToArray();
        }

        /// <summary>
        /// Should be called if a hit was happens.
        /// Get next possible neighbor, dependent from neighbor pointer.
        /// Check for next coordinates range - not lesser then 0 and not higher then size.
        /// </summary>
        /// <param name="lastMove">Last hit coordinates.</param>
        /// <returns>Tupel with next neighbor or, if pointer is heigher then neighbor count, null.</returns>
        private Tuple<int, int> GetNextMoveAfterHit(Tuple<int, int> lastMove)
        {
            var nextMove = GetNeihghbor(lastMove);

            // If neighbor is null - returns to origin hit coordinates with one field more.
            if(nextMove == null)
            {
                if(direction == 1)
                {
                    var oneBehindOrigin = isMoveRight == true ? Tuple.Create(originShipHit.Item1, originShipHit.Item2 - 1) : Tuple.Create(originShipHit.Item1, originShipHit.Item2 + 1);
                    currentHitField = originShipHit;
                    isMoveRight = isMoveRight == false ? true : isMoveRight == true ? false : isMoveRight;
                    nextMove = oneBehindOrigin;
                }
                if(direction == 2)
                {
                    var oneBehindOrigin = isMoveUp == true ? Tuple.Create(originShipHit.Item1 + 1, originShipHit.Item2) : Tuple.Create(originShipHit.Item1 - 1, originShipHit.Item2);
                    currentHitField = originShipHit;
                    isMoveUp = isMoveUp == false ? true : isMoveUp == true ? false : isMoveUp;
                    nextMove = oneBehindOrigin;
                }
            }

            return nextMove;
        }

        /// <summary>
        /// Should call after a hit.
        /// Gets neighbor of passed last coordinates. Dependent from tryCounter -value (points to value from 0 - 3).
        /// </summary>
        /// <param name="lastMove">Tuple with last coordinates.</param>
        /// <returns></returns>
        private Tuple<int, int> GetNeihghbor(Tuple<int, int> lastMove)
        {
            // Get all possible neighbors.
            var neighbors = GetNeighbors(lastMove);

            if(direction == 1)
            {
                neighbors[0] = null;
                neighbors[2] = null;

                neighbors[1] = isMoveRight == false ? null : neighbors[1];
                neighbors[3] = isMoveRight == true ? null : neighbors[3];
            }
            else if(direction == 2)
            {
                neighbors[1] = null;
                neighbors[3] = null;

                neighbors[0] = isMoveUp == false ? null : neighbors[0];
                neighbors[2] = isMoveUp == true ? null : neighbors[2];
            }

            neighbors = RemoveNullIndicies(neighbors);

            return neighbors.Length < 1 ? null : neighbors[0];
        }

        /// <summary>
        /// Set values, which no longer exists in the possible moves -array, null.
        /// As second step delete all places from passed array which are null.
        /// </summary>
        /// <param name="neighbors">Array with <int, int> -Tuple.</param>
        /// <returns>Updated passed Tuple -array.</returns>
        private Tuple<int, int>[] RemoveNullIndicies(Tuple<int, int>[] neighbors)
        {
            // Set not available field null.
            for(var i = 0; i < neighbors.Length; i++)
            {
                if(neighbors[i] != null && possibleMoves.FirstOrDefault(m => m != null && m.Item1 == neighbors[i].Item1 && m.Item2 == neighbors[i].Item2) == null)
                {
                    neighbors[i] = null;
                }
            }

            // Delete all places with null -value from array.
            return neighbors.AsQueryable().Where(n => n != null).ToArray();
        }

        /// <summary>
        /// Search and remove neighbor tuples of passed tuple from Possible moves.
        /// </summary>
        /// <param name="hitField">Tuple from which should be removed neighbor fields.</param>
        private void RemoveNeighbors(Tuple<int, int> hitField)
        {
            var neighborFields = GetNeighbors(hitField);
            for(var i = 0; i < neighborFields.Length; i++)
            {
                var currentNeughbor = neighborFields[i];
                for(var j = 0; j < possibleMoves.Length; j++)
                {
                    var currentMove = possibleMoves[j];
                    if(currentMove == null || currentMove.Item1 != currentNeughbor.Item1 || currentMove.Item2 != currentNeughbor.Item2)
                    {
                        continue;
                    }

                    possibleMoves[j] = null;
                }
            }
            // Remove all null indicies.
            UpdateMoveArray();
        }

        /// <summary>
        /// Get array with neighbor tuples of a tuple.
        /// </summary>
        /// <param name="hit">Tuple from which will get the neighbor tuples.</param>
        /// <returns>Array with neighbors from the passed tuple.</returns>
        private Tuple<int, int>[] GetNeighbors(Tuple<int, int> hit)
        {
            var hitRow = hit.Item1;
            var hitColumn = hit.Item2;

            // All possible neighbors.
            Tuple<int, int>[] neighbors = {
                Tuple.Create(hitRow - 1, hitColumn),
                Tuple.Create(hitRow, hitColumn + 1),
                Tuple.Create(hitRow + 1, hitColumn),
                Tuple.Create(hitRow, hitColumn - 1)
            };

            return neighbors;
        }
    }
}
