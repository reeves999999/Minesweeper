using System;
using System.Linq;

namespace Minesweeper.console
{
    public class GameCreator
    {
        private readonly GameOptions _gameOptions;
        private readonly IGameplayManager _gameplayManager;

        public GameCreator(GameOptions gameOptions, IGameplayManager gameplayManager)
        {
            _gameOptions = gameOptions;
            _gameplayManager = gameplayManager;
        }
        
        public Game Game { get; set; } = new Game();

        public void Run()
        {
            EnforceGameRules();
            GenerateGrid(_gameOptions.GridSize);
            SetBombs(_gameOptions.BombCount);

            //show instructions
            //logger
            //SetStartPoint
            //Set key listeners
            //track move
            //show score
            //show bombs hit/remaining
            //check for end of game
            //gameplay manager service
            //grid as class
            //all code commented
            //static helper to extract letter for int on y axis a-h
            //testing
            //readme.md
            //github deploy

            _gameplayManager.Init();
        }

        private void EnforceGameRules()
        {
            //bomb count must be lower than grid size else start point will exceed first column
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
