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
        /// If a ship was hit, set move for next round - Nearly at the hit.
        /// </summary>
        private Tuple<int, int> nextRoundMove;

        /// <summary>
        /// Should be add one if start to try to find the next ship part.
        /// </summary>
        private int neighborPointer = 0;
        private bool searchForNextHit;

        /// <summary>
        /// First hit coordinate of a ship. 
        /// </summary>
        private Tuple<int, int> originShipHit;
        private Tuple<int, int> currentHitField;
        private Tuple<int, int> neighborField;

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
                if(neighborField != null)
                {
                    //TEST STREAK
                    if(neighborField.Item1 < currentHitField.Item1)
                    {
                        direction = 2;
                        isMoveUp = true;
                        // SENK
                        // GO UP
                    }
                    else if(neighborField.Item1 > currentHitField.Item1) 
                    {
                        direction = 2;
                        isMoveUp = false;
                        // SENK
                        // GO DOWN
                    }
                    else if(neighborField.Item2 < currentHitField.Item2)
                    {
                        direction = 1;
                        isMoveRight = false;
                        // WAG
                        // GO LEFT
                    }
                    else if(neighborField.Item2 > currentHitField.Item2)
                    {
                        direction = 1;
                        isMoveRight = true;
                        // WAG
                        // GO RIGHT
                    }
                    // - - - - - - 

                    currentHitField = neighborField;
                    neighborPointer = 0;
                }

                neighborField = GetNextMoveAfterHit(currentHitField);
                var indexToDelete = Array.IndexOf(possibleMoves, neighborField);
                possibleMoves[indexToDelete] = null;
                UpdateMoveArray();

                var rowChar = field.ConvertRowToChar(neighborField.Item1);
                var cell = neighborField.Item2;
                field.UpdateField(rowChar, cell);

                lastMoveInfo = field.LastMoveInfo;

                finRow = field.ConvertRowToChar(currentHitField.Item1);
                finCell = currentHitField.Item2;
            }

            // If the last shot wasn´t a hit.
            else if (!searchForNextHit || lastMoveInfo == Rules.QuadrantIncludesSunkShip)
            {
                searchForNextHit = false;
                neighborField = null;
                originShipHit = null;
                direction = 0;

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
                // - - -  -
                neighborField = GetNextMoveAfterHit(currentHitField);
                var indexToDelete = Array.IndexOf(possibleMoves, neighborField);
                possibleMoves[indexToDelete] = null;
                UpdateMoveArray();
                lastMoveInfo = field.LastMoveInfo;
                var r = field.ConvertRowToChar(neighborField.Item1);
                var c = neighborField.Item2;
                GameState = field.UpdateField(r, c);
                
                finRow = field.ConvertRowToChar(neighborField.Item1);
                finCell = neighborField.Item2;
            }
            
            return Tuple.Create(finRow, finCell);
        }

        // TEST:
        private void ManagedHit(Tuple<int, int> lastHitField)
        {
            // Next Hit
            currentHitField = lastHitField;
            nextRoundMove = GetNextMoveAfterHit(currentHitField);
        }

        private void ManageFirstHit(Tuple<int, int> hitField)
        {
            // Managed first hit on a ship.
            var lastMoveInfo = field.LastMoveInfo;

            if (lastMoveInfo == Rules.QuadrantIncludesSunkShip) { searchForNextHit = false; }
            if (searchForNextHit || lastMoveInfo == Rules.QuadrantIncludesHitAndShip)
            {
                if (currentHitField == null)
                {
                    currentHitField = hitField;
                }
                searchForNextHit = true;
                nextRoundMove = GetNextMoveAfterHit(currentHitField);
            }
        }

        /// <summary>
        /// Managed the "possible moves" -array.
        /// Search for null value -indicies, count them and create new array without null -indicies and set as new "possible move" -array.
        /// Result is an array with reduced length and without null -indicies.
        /// </summary>
        private void UpdateMoveArray() {

            for(var i = 0; i < possibleMoves.Length - 1; i++)
            {
                if(possibleMoves[i] == null)
                {
                    possibleMoves[i] = possibleMoves[i + 1];
                    possibleMoves[i + 1] = null;
                }
            }

            var nullCounter = 0;
            for(var i = possibleMoves.Length - 1; i >= 0; i--)
            {
                if(possibleMoves[i] != null)
                {
                    break;
                }
                nullCounter++;
            }

            var newPossibleMoves = new Tuple<int, int>[possibleMoves.Length - nullCounter];
            for(var i = 0; i < newPossibleMoves.Length; i++)
            {
                newPossibleMoves[i] = possibleMoves[i];
            }

            possibleMoves = newPossibleMoves;
        }

        int maxNeighborCount = 0;

        /// <summary>
        /// Should be called if a hit was happens.
        /// Get next possible neighbor, dependent from neighbor pointer.
        /// Check for next coordinates range - not lesser then 0 and not higher then size.
        /// </summary>
        /// <param name="lastMove">Last hit coordinates.</param>
        /// <returns>Tupel with next neighbor or, if pointer is heigher then neighbor count, null.</returns>
        private Tuple<int, int> GetNextMoveAfterHit(Tuple<int, int> lastMove)
        {
            var tryCounter = 0;
            Tuple<int, int> nextMove;
            do
            {
                nextMove = GetNeihghbor(lastMove);
                if(nextMove == null)
                {
                    if(direction == 1)
                    {
                        var oneBehindOrigin = isMoveRight == true ? Tuple.Create(originShipHit.Item1, originShipHit.Item2 - 1) : Tuple.Create(originShipHit.Item1, originShipHit.Item2 + 1);
                        currentHitField = originShipHit;
                        isMoveRight = isMoveRight == false ? true : isMoveRight == true ? false : isMoveRight;
                    }
                    if(direction == 2)
                    {
                        var oneBehindHit = isMoveUp == true ? Tuple.Create(originShipHit.Item1 + 1, originShipHit.Item2) : Tuple.Create(originShipHit.Item1 - 1, originShipHit.Item2);
                        currentHitField = originShipHit;
                        isMoveUp = isMoveUp == false ? true : isMoveUp == true ? false : isMoveUp;
                    }
                    tryCounter++;
                }
            } while ((nextMove == null && tryCounter < 4) || nextMove.Item1 < 0 || nextMove.Item1 >= size || nextMove.Item2 < 0 || nextMove.Item2 >= size);

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
            var lastRow = lastMove.Item1;
            var lastColumn = lastMove.Item2;
            
            // All possible neighbors.
            Tuple<int, int>[] neighbors = { 
                Tuple.Create(lastRow - 1, lastColumn), 
                Tuple.Create(lastRow, lastColumn + 1), 
                Tuple.Create(lastRow + 1, lastColumn),
                Tuple.Create(lastRow, lastColumn - 1)
            };

            if(direction == 1)
            {
                neighbors[0] = null;
                neighbors[2] = null;

                neighbors[1] = isMoveRight == false ? null : neighbors[1];
                neighbors[3] = isMoveRight == true ? null : neighbors[3];
                neighborPointer = 0;
            }
            else if(direction == 2)
            {
                neighbors[1] = null;
                neighbors[3] = null;

                neighbors[0] = isMoveUp == false ? null : neighbors[0];
                neighbors[2] = isMoveUp == true ? null : neighbors[2];
                neighborPointer = 0;
            }

            neighbors = RemoveNullIndicies(neighbors);
            maxNeighborCount = neighbors.Length;

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
    }
}
