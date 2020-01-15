using SchiffeVersenken.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchiffeVersenken.Classes.RuleSet
{
    /// <summary>
    /// 0.QuadrantIsFree
    /// 1.QuadrantIncludesHit
    /// 2.QuadrantIncludesShip
    /// 3.QuadrantIncludesHitAndShip
    /// 4.QuadrantIncludesSunkShip
    /// </summary>
    static class Rules
    {
        /// <summary>
        /// State value for qudrant drawing - Value for default state (no hit, no ship).
        /// </summary>
        public static readonly int QuadrantIsFree = 0;

        /// <summary>
        /// State value for qudrant drawing - State value for hited Quadrant.
        /// </summary>
        public static readonly int QuadrantIncludesHit = 1;

        /// <summary>
        /// State value for qudrant drawing - State value for Quadrant which includs ship part.
        /// </summary>
        public static readonly int QuadrantIncludesShip = 2;

        /// <summary>
        /// State value for qudrant drawing - State value for hited Quadrant which includes also a ship part.
        /// </summary>
        public static readonly int QuadrantIncludesHitAndShip = 3;

        /// <summary>
        /// State value for qudrant drawing - State value for Quadrant which includes a sunken ship part.
        /// </summary>
        public static readonly int QuadrantIncludesSunkShip = 4;

        /// <summary>
        /// Enum with all possible game states. 
        /// </summary>
        public enum GameStates { MoveIsAllowed, MoveNoAllowed, GameIsFinished, GameExit }

        public static bool IsCoordinateValid(char x, int y, int maxRowSize)
        {
            var alphabet = Constants.Alphabet;
            var sign = alphabet.FirstOrDefault(s => s == x);
            if(sign == 0 || y < 0 || Array.IndexOf(alphabet, sign) > maxRowSize || y > maxRowSize)
            {
                return false;
            }

            return true;
        }


    }
}
