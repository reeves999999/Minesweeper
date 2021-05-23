using System;
using System.Linq;

namespace Minesweeper.console
{
    public class GameplayManager : IGameplayManager
    {
        private readonly GameOptions _gameOptions;

        public GameplayManager(GameOptions gameOptions)
        {
            _gameOptions = gameOptions;
        }

        public Game Game { get; set; }

        public void Init()
        {
            Console.Clear();
            Game = new Game(_gameOptions);

            EnforceGameRules();
            GenerateGrid(_gameOptions.GridSize);
            SetBombs(_gameOptions.BombCount);
            SetStartPosition();
            ListenForMoves();
        }

        private void SetStartPosition()
        {
            //set start point where no bomb exists
            var firstColumn = Game.Board[0];
            for (int i = 0; i < firstColumn.Length; i++)
            {
                if (firstColumn[i] > -1) //no bomb, place user here
                {
                    Game.XPosition = 0;
                    Game.YPosition = i;
                    break;
                }
            }
            PrintMessage($"Game started. Use arrow keys to navigate. Please escape key to exit, or 'R' to restart.", ConsoleColor.Magenta);
            PrintMessage($"(Bomb free) start position is: {GameHelper.CurrentXPositionLetter(Game.XPosition)}{Game.YPosition + 1}");
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
            //validate moves
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    if (Game.YPosition < _gameOptions.GridSize - 1)
                    {
                        Game.YPosition++;
                    }
                    break;

                case ConsoleKey.DownArrow:
                    if (Game.YPosition > 0)
                    {
                        Game.YPosition--;
                    }
                    break;

                case ConsoleKey.LeftArrow:
                    if (Game.XPosition > 0)
                    {
                        if (Game.XPosition > 0)
                        {
                            Game.XPosition--;
                        }
                    }
                    break;

                case ConsoleKey.RightArrow:
                    if (Game.XPosition < _gameOptions.GridSize - 1)
                    {
                        Game.XPosition++;
                    }
                    break;
            }

            GameStateCheck();
        }

        private void BombCheck()
        {
            var column = Game.Board[Game.XPosition];
            if (column[Game.YPosition] == -1) //bomb!
            {
                Game.BombsHit++;
                //clear bomb
                column[Game.YPosition] = Game.YPosition;

                Game.Lives--;
                PrintMessage($"\nBOMB EXPLODED at position {GameHelper.CurrentXPositionLetter(Game.XPosition)}{Game.YPosition + 1}\t{Game.Lives} lives remaining.\n", ConsoleColor.Red);
            }
            else
            {
                Game.Score++;
            }
        }

        private void GameStateCheck()
        {
            BombCheck();

            if (Game.Lives == 0)
            {
                Game.GameEnded = true;
                PrintMessage($"\n\nGAME OVER - You lost all your lives.\n", ConsoleColor.Red);
                Console.ReadLine();
            }

            if (Game.XPosition == _gameOptions.GridSize - 1)
            {
                Game.GameEnded = true;
                PrintMessage($"\n\nYou made it! With {Game.Lives} {GameHelper.LivesText(Game.Lives)} remaining.\tScore: {Game.Score}\n", ConsoleColor.Green);
                Console.ReadLine();
            }

            if (!Game.GameEnded)
            {
                PrintMessage($"Position: {GameHelper.CurrentXPositionLetter(Game.XPosition)}{Game.YPosition + 1}\tLives:{Game.Lives}\tBombs hit/remaining: {Game.BombsHit }/{_gameOptions.BombCount - Game.BombsHit}\tScore: {Game.Score}");
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
            //less bombs are needed than grid size so user has a valid start position in column A
            if (_gameOptions.BombCount >= _gameOptions.GridSize)
            {
                _gameOptions.BombCount = _gameOptions.GridSize - 1;
            }

            //there must be more bombs than lives else the user can never lose
            if (_gameOptions.Lives > _gameOptions.BombCount)
            {
                _gameOptions.BombCount = _gameOptions.Lives - 1;
            }
        }

        private void GenerateGrid(int size)
        {
            //typically, 8 x 8 grid (of int)
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
                PrintMessage($"Bomb added at position: {GameHelper.CurrentXPositionLetter(targetColumn)}{targetRow + 1}", ConsoleColor.DarkGray);
            }
        }
    }
}
