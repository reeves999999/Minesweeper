using Minesweeper.console;
using Moq;
using System.Linq;
using Xunit;

namespace Minesweeper.tests
{
    //Mixture of basic tests to cover key points
    //Not an extensive suite by any means

    public class GameTests : IClassFixture<GameFixture>
    {
        GameFixture _fixture;

        public GameTests(GameFixture fixture)
        {
            this._fixture = fixture;
        }

        [Fact]
        public void Game_Lives_Count_Matches_Config()
        {
            var sut = _fixture.SUT;
            Assert.True(sut.Game.Lives == _fixture.GameOptions.Lives);
        }

        [Fact]
        public void Bomb_Count_Is_Higher_Than_Lives_Count()
        {
            GameOptions options = new GameOptions
            {
                Lives = 2,
                BombCount = 1,
                GridSize = 8
            };

            var SUT = new GameplayManager(options,_fixture.MessageWriter);
            SUT.Game = new Game(options);
            SUT.Init();

            Assert.True(SUT.Game.BombCount > SUT.Game.Lives);
        }

        [Fact]
        public void When_Player_Is_Positioned_Centrally_Moving_In_Any_Direction_Works()
        {
            var sut = _fixture.SUT;
            sut.Init();
            sut.Game.XPosition = 4;
            sut.Game.YPosition = 4;

            sut.Move(GameMovesEnum.Up);
            Assert.True(sut.Game.YPosition == 5);

            sut.Game.XPosition = 4;
            sut.Game.YPosition = 4;

            sut.Move(GameMovesEnum.Down);
            Assert.True(sut.Game.YPosition == 3);

            sut.Game.XPosition = 4;
            sut.Game.YPosition = 4;

            sut.Move(GameMovesEnum.Left);
            Assert.True(sut.Game.XPosition == 3);

            sut.Game.XPosition = 4;
            sut.Game.YPosition = 4;

            sut.Move(GameMovesEnum.Right);
            Assert.True(sut.Game.XPosition == 5);
        }

        [Fact]
        public void When_Player_Is_Positioned_At_Boundaries_Move_Does_Nothing()
        {
            var sut = _fixture.SUT;
            sut.Init();
            sut.Game.XPosition = 0;
            sut.Game.YPosition = 0;

            sut.Move(GameMovesEnum.Left);
            Assert.True(sut.Game.XPosition == 0);

            sut.Move(GameMovesEnum.Down);
            Assert.True(sut.Game.YPosition == 0);

            sut.Game.YPosition = 7;
            sut.Move(GameMovesEnum.Up);
            Assert.True(sut.Game.YPosition == 7);
        }

        [Fact]
        public void When_Player_Reaches_Last_Column_7_Game_Ends()
        {
            GameOptions options = new GameOptions
            {
                Lives = 2,
                BombCount = 1,
                GridSize = 8
            };

            var mock = new Mock<GameplayManager>(options, new BlankMessageProcessor());
            var sut = mock.Object;
            sut.Init();

            sut.Game.XPosition = 7;
            sut.Game.YPosition = 7;

            sut.Move(GameMovesEnum.Right);
            mock.Verify(x => x.SetGameSuccess(), Times.Once());
        }

        [Fact]
        public void When_Player_Has_Hit_All_Bombs_Game_Ends()
        {
            GameOptions options = new GameOptions
            {
                Lives = 1,
                BombCount = 2,
                GridSize = 8
            };

            var mock = new Mock<GameplayManager>(options, new BlankMessageProcessor());
            var sut = mock.Object;
            sut.Init();

            sut.Game.XPosition = 0;
            sut.Game.YPosition = 0;

            //set grid to all bombs
            for (int i = 0; i < sut.Game.GridSize; i++)
            {
                sut.Game.Board[i] = Enumerable.Repeat(-1, sut.Game.GridSize).ToArray();
            }

            sut.Move(GameMovesEnum.Right);
            Assert.True(sut.Game.GameEnded);
        }
    }

}
