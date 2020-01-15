using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchiffeVersenken.RulesSet
{
    static class Rules
    {
        /// <summary>
        /// Quadrant is free -value.
        /// </summary>
        public static readonly int QuadrantIsFree = 0;

        /// <summary>
        /// Quadrant includes ship part -value.
        /// </summary>
        public static readonly int QuadrantIncludesShip = 1;

        /// <summary>
        /// Quadrant includes hit.
        /// </summary>
        public static readonly int QuadrantIncludesHit = 2;

        /// <summary>
        /// Quadrant includes ship and hit -value.
        /// </summary>
        public static readonly int QuadrantIncludesHitAndShip = 3;

        /// <summary>
        /// Quadrant includes ship which should be visible at start.
        /// </summary>
        public static readonly int QuadrantIncludesShipWhichShouldBeViewable = 4;
    }
}
