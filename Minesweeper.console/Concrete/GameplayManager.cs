using System;
using System.Linq;

namespace Minesweeper.console
{
    //logger
    //testing
    //readme.md

    public class GameplayManager : IGameplayManager
    {
        private readonly GameOptions _gameOptions;

        public GameplayManager(GameOptions gameOptions)
        {
            _gameOptions = gameOptions;
        }

        Game Game { get; set; }

        public void Init()
        {
            Console.Clear();
            Game = new Game();
            Game.Lives = _gameOptions.Lives;
            EnforceGameRules();
            GenerateGrid(_gameOptions.GridSize);
            SetBombs(_gameOptions.BombCount);

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

            PrintMessage($"Game started. Use arrow keys to navigate. Please escape key to exit.", ConsoleColor.Magenta);
            PrintMessage($"Current position is: {GameHelper.CurrentXPositionLetter(Game.CurrentXPosition)}{Game.CurrentYPosition + 1}");

            ListenForMoves();
        }

        private void ListenForMoves()
        {

            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey();

                if (keyInfo.Key == ConsoleKey.R)
                {
                    Init();
                }
                else
                {
                    HandleArrowMoves(keyInfo);
                }

            }
            while (keyInfo.Key != ConsoleKey.Escape && !Game.GameEnded);
        }

        private void HandleArrowMoves(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    if (Game.CurrentYPosition < _gameOptions.GridSize - 1)
                    {
                        Game.CurrentYPosition++;
                    }
                    break;

                case ConsoleKey.DownArrow:
                    if (Game.CurrentYPosition > 0)
                    {
                        Game.CurrentYPosition--;
                    }
                    break;

                case ConsoleKey.LeftArrow:
                    if (Game.CurrentXPosition > 0)
                    {
                        if (Game.CurrentXPosition > 0)
                        {
                            Game.CurrentXPosition--;
                        }
                    }
                    break;

                case ConsoleKey.RightArrow:
                    if (Game.CurrentXPosition < _gameOptions.GridSize - 1)
                    {
                        Game.CurrentXPosition++;
                    }
                    break;
            }

            GameStateCheck();
        }

        private void BombCheck()
        {
            var column = Game.Board[Game.CurrentXPosition];
            for (int i = 0; i < column.Length; i++)
            {
                if (column[i] == -1) //bomb!
                {
                    Game.BombsHit++;
                    //clear bomb
                    column[i] = i;

                    //reduce lives
                    Game.Lives--;
                    PrintMessage($"\nBOMB EXPLODED at position {GameHelper.CurrentXPositionLetter(Game.CurrentXPosition)}{Game.CurrentYPosition + 1}\t{Game.Lives} lives remaining.\n", ConsoleColor.Red);

                    if (Game.Lives == 0)
                    {
                        Game.GameEnded = true;
                        PrintMessage($"\n\nGAME OVER - You lost all your lives.\n", ConsoleColor.Red);
                        PrintMessage("Hit escape key to exit.");
                    }
                    break;
                }
            }
        }

        private void GameStateCheck()
        {
            BombCheck();

            if (Game.CurrentXPosition == _gameOptions.GridSize - 1)
            {
                Game.GameEnded = true;
                string livesText = Game.Lives > 1 ? "lives" : "life";
                PrintMessage($"\n\nYou made it! With {Game.Lives} {livesText} remaining.\n", ConsoleColor.Green);
                PrintMessage("Hit escape key to exit.");
            }

            if (!Game.GameEnded)
            {
                PrintMessage($"Current position is: {GameHelper.CurrentXPositionLetter(Game.CurrentXPosition)}{Game.CurrentYPosition + 1}\n");
                PrintMessage($"Lives remaining: { Game.Lives}");
                PrintMessage($"Bombs hit/remaining: {Game.BombsHit }/{_gameOptions.BombCount - Game.BombsHit}");
            }
        }

        private void PrintMessage(string message, ConsoleColor colour = ConsoleColor.White)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                Console.ForegroundColor = colour;
                Console.WriteLine(message);
            }
        }


        private void EnforceGameRules()
        {
            if (_gameOptions.BombCount > _gameOptions.GridSize)
            {
                _gameOptions.BombCount = _gameOptions.GridSize - 1;
            }
        }

        private void GenerateGrid(int size)
        {
            int[][] grid = new int[size][];

            for (int i = 0; i < size; i++)
            {
                grid[i] = Enumerable.Range(0, size).ToArray();
            }

            Game.Board = grid;
        }

        private void SetBombs(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Random randomGenerator = new Random();
                int targetColumn = randomGenerator.Next(0, _gameOptions.GridSize);
                int targetRow = randomGenerator.Next(0, _gameOptions.GridSize);

                var gridColumn = Game.Board[targetColumn];
                gridColumn[targetRow] = -1;
            }
        }
    }
}
