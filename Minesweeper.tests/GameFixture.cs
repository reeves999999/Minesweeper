using Microsoft.Extensions.Configuration;
using Minesweeper.console;
using System.IO;

namespace Minesweeper.tests
{
    //Just for setup/teardown purposes
    public class GameFixture
    {
        IConfiguration _config;
        public GameplayManager SUT { get; private set; }
        public IMessageProcessor MessageWriter { get; private set; }

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
            GameOptions.GridSize = 8;
            MessageWriter = new BlankMessageProcessor();
            SUT = new GameplayManager(GameOptions, MessageWriter);
            SUT.Game = new Game(GameOptions);
        }
    }
}
