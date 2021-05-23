using System;
using System.Linq;

namespace Minesweeper.console
{
    //This class controls the game 
    public class GameplayManager : IGameplayManager
    {
        //Inject GameOptions (from singleton)
        //Inject message processor
        private readonly GameOptions _gameOptions;
        private readonly IMessageProcessor _messageProcessor;

        public GameplayManager(GameOptions gameOptions, IMessageProcessor messageProcessor)
        {
            _gameOptions = gameOptions;
            _messageProcessor = messageProcessor;
        }

        //Logical Game unit
        public Game Game { get; set; }

        //Set up the game
        public void Init()
        {
            Game = new Game(_gameOptions);
            EnforceGameRules();
            GenerateGrid();
            SetBombs(_gameOptions.BombCount);
            SetStartPosition();
        }

        //Find a start position in the first column that does not contain a bomb. There is no fun starting life sat on a bomb, after all.
        //The maximum allowed bomb count must be at least one less than the size of a board axis (Typically 8)
        //Otherwise, additional check would have been required in more columns than just the first.
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

            //Set messaging
            _messageProcessor.PrintMessage($"Game started. Use arrow keys to navigate. Please escape key to exit, or 'R' to restart.", ConsoleColor.Magenta);
            _messageProcessor.PrintMessage($"(Bomb free) start position is: {GameHelper.CurrentXPositionLetter(Game.XPosition)}{Game.YPosition + 1}",ConsoleColor.White);
        }

        public void Start()
        {
            //Capture UI key events from the console
            //This will be ignored in tests, hence the corresponding "Move" method
            //Escape (Quit) and R (Restart) keys added
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

            }
            while (keyInfo.Key != ConsoleKey.Escape && !Game.GameEnded);
        }
            

        public void Move(GameMovesEnum direction)
        {
            //Ensure the attempted move lies within the boundary of the game grid.
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

            
        public void GameStateCheck()
        {
            //check possible game outcomes

            //check bomb
            BombCheck();

            if (Game.Lives == 0)
            {
                SetGameOver();
                _messageProcessor.Pause();
                //it's over I'm afraid
            }

            if (Game.XPosition == _gameOptions.GridSize - 1)
            {
                SetGameSuccess();
                _messageProcessor.Pause();
                //legend
            }

            if (!Game.GameEnded)
            {
                _messageProcessor.PrintMessage($"Position: {GameHelper.CurrentXPositionLetter(Game.XPosition)}{Game.YPosition + 1}\tLives:{Game.Lives}\tBombs hit/remaining: {Game.BombsHit }/{_gameOptions.BombCount - Game.BombsHit}\tScore: {Game.Score}", ConsoleColor.White);
                //set message
                //play on
            }
        }

        private void BombCheck()
        {
            //Check to see if player has landed on a bomb
            //Decrease lives if so
            //Set bomb back to normal cell value as only one thing is worse than sitting on a bomb - sitting on the same bomb twice. The world can do without such problems.
            var column = Game.Board[Game.XPosition];
            if (column[Game.YPosition] == -1) //bomb!
            {
                Game.BombsHit++;
                //clear bomb
                column[Game.YPosition] = Game.YPosition;

                //reduce lives
                Game.Lives--;

                //set message
                //no point scored for such a reckless move
                _messageProcessor.PrintMessage($"\nBOMB EXPLODED at position {GameHelper.CurrentXPositionLetter(Game.XPosition)}{Game.YPosition + 1}\t{Game.Lives} lives remaining.\n", ConsoleColor.Red);
            }
            else
            {
                //bomb avoided - increase score
                Game.Score++;
            }
        }

        public void SetGameOver()
        {
            //As name
            Game.GameEnded = true;
            _messageProcessor.PrintMessage($"\n\nGAME OVER - You lost all your lives.\n", ConsoleColor.Red);
        }

        public virtual void SetGameSuccess()
        {
            //As name
            Game.GameEnded = true;
            _messageProcessor.PrintMessage($"\n\nYou made it! With {Game.Lives} {GameHelper.LivesText(Game.Lives)} remaining.\tScore: {Game.Score}\n", ConsoleColor.Green);
        }

        private void EnforceGameRules()
        {
            //Purely restricted for use of letters A-Z
            //Wooden restriction
            if(Game.GridSize > 26)
            {
                Game.GridSize = 26;
            }

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
            //Typically, 8 x 8 grid (of ints)
            //There are more ways to do this than there are types of hot dinner. Variety used.
            int[][] grid = new int[Game.GridSize][];

            for (int i = 0; i < Game.GridSize; i++)
            {
                grid[i] = Enumerable.Range(0, Game.GridSize).ToArray();
            }

            //Set the game board
            Game.Board = grid;
        }

        private void SetBombs(int count)
        {
            //Place these nasty critters at random locations (before the user get to move)
            //Use -1 value for bombs
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
