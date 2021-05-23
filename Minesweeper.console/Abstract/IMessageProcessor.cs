using System;

namespace Minesweeper.console
{
    public interface IMessageProcessor
    {
        void PrintMessage(string message, ConsoleColor colour);

        void Pause();
    }
}
