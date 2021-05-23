namespace Minesweeper.console
{
    //This corresponds with the appsettings file. 
    //Default exist
    public class GameOptions
    {
        public int BombCount { get; set; } = 5;
        public int Lives { get; set; } = 3;
        public int GridSize { get; set; } = 8;
    }
}
