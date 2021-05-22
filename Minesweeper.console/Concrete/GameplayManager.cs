using System;

namespace Minesweeper.console
{
    public class GameplayManager : IGameplayManager
    {
        private readonly GameOptions _gameOptions;

        public GameplayManager(GameOptions gameOptions)
        {
            _gameOptions = gameOptions;
        }

        Game Game { get; set; }

        public void Init(Game game)
        {
            Game = game;
            Game.Lives = _gameOptions.Lives;

            //set start point where no bomb exists
            var firstColumn = Game.Board[0];
            for (int i = 0; i < firstColumn.Length; i++)
            {
                if (firstColumn[i] > -1) //no bomb
                {
                    Game.CurrentXPosition = 0;
                    Game.CurrentYPosition = i;
                    break;
                }
            }

            Console.WriteLine("Starting game...");
            Console.WriteLine($"Current position is: {GameHelper.CurrentXPositionLetter(Game.CurrentXPosition)}{Game.CurrentYPosition + 1}");

            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey();
                HandleMove(keyInfo);
            }
            while (keyInfo.Key != ConsoleKey.Escape);

        }

        private void HandleMove(ConsoleKeyInfo keyInfo)
        {
            Game.Message = string.Empty;
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    if (Game.CurrentYPosition < (_gameOptions.GridSize - 1))
                    {
                        Game.CurrentYPosition++;

                        var column = Game.Board[Game.CurrentYPosition];
                        for (int i = 0; i < column.Length; i++)
                        {
                            if (column[i] == -1) //bomb!
                            {
                                //clear bomb
                                column[i] = i;

                                //reduce lives
                                Game.Lives--;
                                Game.Message = $"BOMB EXPLODED at position {GameHelper.CurrentXPositionLetter(column[i])}{Game.CurrentYPosition}";
                                break;
                            }
                        }

                    }
                    break;

                case ConsoleKey.DownArrow:
                    if (Game.CurrentYPosition > 0)
                    {
                        Game.CurrentYPosition--;
                    }
                    break;

                case ConsoleKey.LeftArrow:
                    if (Game.CurrentXPosition > 0 )
                    {
                        Game.CurrentXPosition--;
                    }
                    break;

                case ConsoleKey.RightArrow:
                    if (Game.CurrentXPosition < (_gameOptions.GridSize-1))
                    {
                        Game.CurrentXPosition++;
                    }
                    break;
            }

            Console.WriteLine($"Current position is: {GameHelper.CurrentXPositionLetter(Game.CurrentXPosition)}{Game.CurrentYPosition +1}");
            Console.WriteLine(Game.Message);
            Console.WriteLine(Game.Lives);



        }

        //private bool ValidateMove(GameMovesEnum direction)
        //{
        //    return 
        //}
    }
}
