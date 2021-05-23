using Microsoft.Extensions.Configuration;
using Minesweeper.console;
using System;
using System.IO;
using Xunit;

namespace Minesweeper.tests
{
        //game starts
        //moves from central position fire ok
        //lives are less than bombs
        //game ends
        //bomb count is less than grid size (8)(Chess board mentioned in spec.)

    public class GameTests:IClassFixture<GameFixture>
    {
        GameFixture _fixture;

        public GameTests(GameFixture fixture)
        {
            this._fixture = fixture;
        }

        [Fact]
        public void Game_Lives_Count_Matches_Config()
        {
            var gameplayManager = _fixture.SUT;
            Assert.True(gameplayManager.Game.Lives == _fixture.GameOptions.Lives);
        }

        
    }

    public class GameFixture
    {        
        IConfiguration _config;
        public GameplayManager SUT { get; private set; }

        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            return builder.Build();
        }

        public GameOptions GameOptions { get; set; }

        public GameFixture()
        {
            _config = LoadConfiguration();
            GameOptions = new GameOptions();
            _config.GetSection(nameof(GameOptions)).Bind(GameOptions);
            SUT = new GameplayManager(GameOptions);
            SUT.Game = new Game(GameOptions);
        }
    }
}
