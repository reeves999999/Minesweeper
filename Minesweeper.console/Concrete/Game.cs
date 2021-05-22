namespace Minesweeper.console
{
    public class Game
    {
        public int[][] Board { get; set; }

        public int Lives { get; set; }

        public int BombCount { get; set; }

        public int CurrentXPosition { get; set; }
        public int CurrentYPosition { get; set; }

        public string Message { get; set; }
    }
}
