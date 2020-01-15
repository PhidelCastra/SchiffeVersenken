using SchiffeVersenken.Classes.RuleSet;
using SchiffeVersenken.Data;
using SchiffeVersenken.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchiffeVersenken.Classes
{
    /// <summary>
    /// A part of the field -object.
    /// </summary>
    class Quadrant
    {
        /// <summary>
        /// Index value to named this object.
        /// </summary>
        private QuadrantIndex index;

        public int State;
        public ShipPart ShipPart { get; set; }

        /// <summary>
        /// Value for width / Characters from left to right.
        /// </summary>
        private int width;

        /// <summary>
        /// All ASCII -parts to build a viewable square in the console.
        /// </summary>
        private char sideBorder = '|';
        private char upperBoder = '¯';
        private char bottomBorder = '_';
        private char middlePart = ' ';

        /* Constructor */
        public Quadrant(int width, QuadrantIndex index)
        {
            State = Rules.QuadrantIsFree;

            this.width = width;

            this.index = index;
        }

        /// <summary>
        /// Draw upper part of a square in console.
        /// </summary>
        public void DrawUpperBorder()
        {
            DrawInLine(upperBoder);
        }
        /// <summary>
        /// Draw the middle part of a square in console.
        /// </summary>
        public void DrawMiddlePart()
        {
            DrawInLine(middlePart);
        }
        /// <summary>
        /// Draw bottom part of a square in console.
        /// </summary>
        public void DrawBottom()
        {
            DrawInLine(bottomBorder);
        }

        public void DrawInLine(char specialPart)
        {
            SetColor(ConsoleColors.BorderColor);
            Console.Write(sideBorder);
            switch (State)
            {
                // Default state - no hit yet.
                case 0:
                    SetColor(ConsoleColors.MainColor);
                    break;
                // Hit water.
                case 1:
                    SetColor(ConsoleColors.WaterHitColor);
                    break;
                case 2:
                    Console.WriteLine("State 2 is not available yet!");
                    // SetColor(shipPartColor);
                    break;
                // Hit ship part.
                case 3:
                    SetColor(ConsoleColors.ShipPartColor);
                    break;
                case 4:
                    SetColor(ConsoleColors.ShipIsSunkColor);
                    break;
                default:
                    SetColor(ConsoleColors.MainColor);
                    if (ShipPart != null) { SetColor(ConsoleColors.ShipPartColor); }
                    break;
            }
            DrawWhidth(specialPart);
            SetColor(ConsoleColors.BorderColor);
            Console.Write(sideBorder);
        }

        /// <summary>
        /// Draw a char sequence from left to right site in console.
        /// </summary>
        /// <param name="buildPart">Char to draw in console.</param>
        private void DrawWhidth(char buildPart)
        {
            for (var i = 0; i < width; i++) { Console.Write(buildPart); }
        }

        /// <summary>
        /// Set the corresponding border color.
        /// </summary>
        /// <param name="color">Color to set in console.</param>
        private void SetColor(ConsoleColor color)
        {
            Console.BackgroundColor = color;
        }

        /// <summary>
        /// Get quadrant index of this object.
        /// </summary>
        /// <returns>QuadrantIndex of this object.</returns>
        public QuadrantIndex GetIndex()
        {
            return index;
        }
    }
}
