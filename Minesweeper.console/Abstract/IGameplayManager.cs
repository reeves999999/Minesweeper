namespace Minesweeper.console
{
    public interface IGameplayManager
    {
        void Init();
        Game Game { get; set; }
    }
}
