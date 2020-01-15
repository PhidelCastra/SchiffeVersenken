using SchiffeVersenken.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchiffeVersenken.Classes
{
    class Ship
    {
        /// <summary>
        /// Id of ship which should also passed to all corresponding ship parts.
        /// </summary>
        private static int staticID = 0;
        public int ID = -1;

        public ShipPart[] Parts;

        private char FrontRightPart = '/';
        private char borderPart = '¯';

        public Dictionary<ShipPart, Quadrant> partDict = new Dictionary<ShipPart, Quadrant>();

        /* Constructor */
        public Ship(int parts)
        {
            // Set ship id and add one to static id for the next ship.
            ID = staticID;
            staticID++;

            Parts = Build(parts);

            if(ID < 0) { throw new Exception($"No valid ship id - Ship ID is with {ID} lesser than 0!"); }
        }

        public bool IsSunk()
        {
            var hitParts = Parts.AsEnumerable().Count(p => p.IsHit);
            return hitParts >= Parts.Length;
        }

        public int Length()
        {
            return Parts.Length;
        }

        /// <summary>
        /// Prepare ship parts in array / Set front part.
        /// </summary>
        /// <param name="partNr">Length of parts array.</param>
        /// <returns>Array with Ship parts.</returns>
        private ShipPart[] Build(int partNr)
        {
            var shipParts = new ShipPart[partNr];
            for(var place = 0; place < partNr; place++)
            {
                shipParts[place] = new ShipPart(ID);
            }

            return shipParts;
        }
    }

    class ShipPart
    {
        public bool IsHit { get; set; }

        public int PartID { get; private set; }

        /* Constructor */
        public ShipPart(int id)
        {
            PartID = id;
            IsHit = false;
        }
    }
}
