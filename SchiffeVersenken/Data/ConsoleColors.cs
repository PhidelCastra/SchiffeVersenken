using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchiffeVersenken.Data
{
    class ConsoleColors
    {
        /// <summary>
        /// Field main -color for quadrants.
        /// </summary>
        public static ConsoleColor MainColor = ConsoleColor.Blue;

        /// <summary>
        /// Border color for a quadrant.
        /// </summary>
        public static ConsoleColor BorderColor = ConsoleColor.DarkBlue;

        /// <summary>
        /// Color for hited quadrant.
        /// </summary>
        public static ConsoleColor WaterHitColor = ConsoleColor.Cyan;

        /// <summary>
        /// Color for quadrant which includes ship part.
        /// </summary>
        public static ConsoleColor ShipPartColor = ConsoleColor.DarkGray;

        /// <summary>
        /// Color of part of a sunken ship.
        /// </summary>
        public static ConsoleColor ShipIsSunkColor = ConsoleColor.DarkYellow;
    }
}
