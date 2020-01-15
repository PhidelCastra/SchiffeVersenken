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
        private Field field;

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
        private int tryCounter = 0;

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
            if(nextRoundMove != null)
            {
                // Find next ship part.
            }

            var index = moveCreator.Next(0, possibleMoves.Length);

            // Find tupel and set index null.
            var result = possibleMoves[index];
            possibleMoves[index] = null;

            // Remove null -indicies.
            UpdateMoveArray();

            var rowChar = Constants.Alphabet[result.Item1];

            GameState = field.UpdateField(rowChar, result.Item2);
            if(GameState == Rules.GameStates.GameIsFinished)
            {
                HasWon = true; 
            }

            return Tuple.Create(rowChar, result.Item2);
        }

        /// <summary>
        /// Search for tuple in moves -array. Remove the corresponding tuple from array (Set index of this null).
        /// </summary>
        /// <param name="row">Should be a value for item1.</param>
        /// <param name="cell">Should be a value for item2.</param>
        /// <returns>Tuple if it was found, otherwise null.</returns>
        private Tuple<int, int> FindTupel(int row, int cell)
        {
            // Check passed value before search them.
            if(row >= possibleMoves.Length || row < 0 || cell >= possibleMoves.Length || cell < 0)
            {
                throw new ArgumentOutOfRangeException($"A tupel with row index '{row}' and cell index '{cell}' is not include.");
            }

            var result = possibleMoves.FirstOrDefault(t => t.Item1 == row && t.Item2 == cell);
            var index = Array.IndexOf(possibleMoves, result);
            possibleMoves[index] = null;
            return result;
        }

        /// <summary>
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

        private Tuple<int, int> GetNeihghbor(Tuple<int, int> lastMove)
        {
            var lastRow = lastMove.Item1;
            var lastColumn = lastMove.Item2;
            switch (tryCounter)
            {
                case 0:
                    tryCounter++;
                    return Tuple.Create(lastRow - 1, lastColumn);
                case 1:
                    tryCounter++;
                    return Tuple.Create(lastRow, lastColumn + 1);
                case 2:
                    tryCounter++;
                    return Tuple.Create(lastRow + 1, lastColumn);
                case 3:
                    tryCounter++;
                    return Tuple.Create(lastRow, lastColumn - 1);
                default:
                    throw new InvalidOperationException($"Try counter is with value '{tryCounter}' out of range! Valid is range is 0 - 3.");
            }
        }
    }
}
