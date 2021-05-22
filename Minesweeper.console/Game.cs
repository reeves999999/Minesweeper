using System;

namespace Minesweeper.console
{
    public class Game
    {
        public Game(int lives)
        {
            Lives = lives;
        }

        public int[][] Board { get; set; }

        public int Lives { get; set; }

        public int BombsHit { get; set; }

        public bool GameEnded { get; set; }

        public int CurrentXPosition { get; set; }
        
        public int CurrentYPosition { get; set; }
    }
}
