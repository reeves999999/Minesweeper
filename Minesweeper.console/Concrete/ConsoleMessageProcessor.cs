using System;

namespace Minesweeper.console
{
    public class ConsoleMessageProcessor : IMessageProcessor
    {
        public void PrintMessage(string message, ConsoleColor colour)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine(message,Console.ForegroundColor=colour);
            }
        }

        public void Pause()
        {
            Console.ReadLine();
        }
    }
}
