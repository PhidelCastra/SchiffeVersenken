using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchiffeVersenken.Data
{
    class Messages
    {
        private static ConsoleColor badFGColor = ConsoleColor.DarkRed;

        private static ConsoleColor goodFGColor = ConsoleColor.DarkGreen;

        private static ConsoleColor defaultFGColor = Console.ForegroundColor;

        public static readonly string CoordinatesNotValidMsg = "The entered {0}{1} coordinates was out of range.";

        public static readonly string ShipIsSunkenMsg = "Ship {0} with length of {1} is sunken!";

        public static readonly string AllShipsAreSunkenMsg = "Game finished - All ships on field was sunken!";

        public static readonly string ShipWasHitMsg = "Ship part hit on quadrant {0}{1}!";

        public static readonly string PlayerHasWonMsg = "You have won against '{0}'!";

        public static readonly string PlayerHasLost = "You have lost against '{0}'!";

        public static readonly string PlayerEndInformations = "You has needed {0} moves, {1} of this was fail moves.";

        /// <summary>
        /// Print message on console independent of "GoodNews" boolean.
        /// </summary>
        /// <param name="message">Message -object</param>
        public static void ShowMessage(Message message)
        {
            Console.ForegroundColor = message.IsGoodNews ? goodFGColor : badFGColor;
            Console.WriteLine();
            Console.WriteLine(message.Msg);
            Console.ForegroundColor = defaultFGColor;
        }
    }
    class Message
    {
        public string Msg { get; private set; }

        public bool IsGoodNews { get; private set; }

        public Message(string msg, bool goodNews)
        {
            Msg = msg;

            IsGoodNews = goodNews;
        }
    }

}
