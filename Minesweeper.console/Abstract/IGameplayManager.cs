namespace Minesweeper.console
{
    public interface IGameplayManager
    {
        void Init();

        void Start();

        void Move(GameMovesEnum direction);

        void GameStateCheck();

        Game Game { get; set; }
    }
}
