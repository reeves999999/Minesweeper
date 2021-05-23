namespace Minesweeper.console
{
    public class Game
    {
        public Game(GameOptions options)
        {
            Lives = options.Lives;
        }

        public int[][] Board { get; set; }

        public int Lives { get; set; }

        public int Score { get; set; }

        public int BombsHit { get; set; }

        public bool GameEnded { get; set; }

        public int XPosition { get; set; }
        
        public int YPosition { get; set; }        
    }
}
