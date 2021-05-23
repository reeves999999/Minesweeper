using System;

namespace Minesweeper.console
{
    //This is purely to abstract away a non-functioning message processor as the console version cannot easily be tested
    public class BlankMessageProcessor : IMessageProcessor
    {

        public void PrintMessage(string message, ConsoleColor colour)
        {
            //do nothing
        }
        public void Pause()
        {
            //do nothing
        }
    }
}
