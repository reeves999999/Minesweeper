using System;

namespace Minesweeper.console
{
    //Allow writing of messages to the console window. Coulured text is supported.
    public class ConsoleMessageProcessor : IMessageProcessor
    {
        public void PrintMessage(string message, ConsoleColor colour)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine(message,Console.ForegroundColor=colour);
            }
        }

        //logical pause point that probably should not be here!
        public void Pause()
        {
            Console.ReadLine();
        }
    }
}
