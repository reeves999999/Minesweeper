using System;
using System.Linq;

namespace Minesweeper.console
{
    public class GameplayManager : IGameplayManager
    {
        private readonly GameOptions _gameOptions;
        private readonly IMessageProcessor _messageProcessor;

        public GameplayManager(GameOptions gameOptions, IMessageProcessor messageProcessor)
        {
            _gameOptions = gameOptions;
            _messageProcessor = messageProcessor;
        }

        public Game Game { get; set; }

        public void Init()
        {
            Game = new Game(_gameOptions);
            EnforceGameRules();
            GenerateGrid();
            SetBombs(_gameOptions.BombCount);
            SetStartPosition();
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
            _messageProcessor.PrintMessage($"Game started. Use arrow keys to navigate. Please escape key to exit, or 'R' to restart.", ConsoleColor.Magenta);
            _messageProcessor.PrintMessage($"(Bomb free) start position is: {GameHelper.CurrentXPositionLetter(Game.XPosition)}{Game.YPosition + 1}",ConsoleColor.White);
        }

        public void Start()
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
                    HandleConsoleInput(keyInfo);
                }

            }
            while (keyInfo.Key != ConsoleKey.Escape && !Game.GameEnded);
        }

        /// <summary>
        /// This needs to just capture UI events and delegate as not easily testable otherwise
        /// </summary>
        /// <param name="keyInfo"></param>
        private void HandleConsoleInput(ConsoleKeyInfo keyInfo)
        {
            //validate moves
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    Move(GameMovesEnum.Up);
                    break;

                case ConsoleKey.DownArrow:
                    Move(GameMovesEnum.Down);
                    break;

                case ConsoleKey.LeftArrow:
                    Move(GameMovesEnum.Left);
                    break;

                case ConsoleKey.RightArrow:
                    Move(GameMovesEnum.Right);
                    break;
            }
        }

        public void Move(GameMovesEnum direction)
        {
            switch (direction)
            {
                case GameMovesEnum.Up:
                    if (Game.YPosition < _gameOptions.GridSize - 1)
                    {
                        Game.YPosition++;
                    }
                    break;

                case GameMovesEnum.Down:
                    if (Game.YPosition > 0)
                    {
                        Game.YPosition--;
                    }
                    break;

                case GameMovesEnum.Left:
                    if (Game.XPosition > 0)
                    {
                        if (Game.XPosition > 0)
                        {
                            Game.XPosition--;
                        }
                    }
                    break;

                case GameMovesEnum.Right:
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
                _messageProcessor.PrintMessage($"\nBOMB EXPLODED at position {GameHelper.CurrentXPositionLetter(Game.XPosition)}{Game.YPosition + 1}\t{Game.Lives} lives remaining.\n", ConsoleColor.Red);
            }
            else
            {
                Game.Score++;
            }
        }

        public void GameStateCheck()
        {
            BombCheck();

            if (Game.Lives == 0)
            {
                SetGameOver();
                _messageProcessor.Pause();
            }

            if (Game.XPosition == _gameOptions.GridSize - 1)
            {
                SetGameSuccess();
                _messageProcessor.Pause();
            }

            if (!Game.GameEnded)
            {
                _messageProcessor.PrintMessage($"Position: {GameHelper.CurrentXPositionLetter(Game.XPosition)}{Game.YPosition + 1}\tLives:{Game.Lives}\tBombs hit/remaining: {Game.BombsHit }/{_gameOptions.BombCount - Game.BombsHit}\tScore: {Game.Score}", ConsoleColor.White);
            }

        }

        public void SetGameOver()
        {
            Game.GameEnded = true;
            _messageProcessor.PrintMessage($"\n\nGAME OVER - You lost all your lives.\n", ConsoleColor.Red);
        }

        public virtual void SetGameSuccess()
        {
            Game.GameEnded = true;
            _messageProcessor.PrintMessage($"\n\nYou made it! With {Game.Lives} {GameHelper.LivesText(Game.Lives)} remaining.\tScore: {Game.Score}\n", ConsoleColor.Green);
        }

        private void EnforceGameRules()
        {
            //less bombs are needed than grid size so user has a valid start position in column A
            if (Game.BombCount >= Game.GridSize)
            {
                Game.BombCount = Game.GridSize - 1;
            }

            //there must be more bombs than lives else the user can never lose
            if (Game.Lives > Game.BombCount)
            {
                Game.BombCount = Game.Lives + 1;
            }
        }

        private void GenerateGrid()
        {
            //typically, 8 x 8 grid (of int)
            int[][] grid = new int[Game.GridSize][];

            for (int i = 0; i < Game.GridSize; i++)
            {
                grid[i] = Enumerable.Range(0, Game.GridSize).ToArray();
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
                _messageProcessor.PrintMessage($"Bomb added at position: {GameHelper.CurrentXPositionLetter(targetColumn)}{targetRow + 1}", ConsoleColor.Gray);
            }
        }
    }
}
