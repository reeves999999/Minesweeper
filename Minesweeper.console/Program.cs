using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Minesweeper.console
{
    class Program
    {
        //setup game environment. Env vars are not currently supported. Neither are args.
        //A gameplay manager is created, Initilised, and started. This runs the game
        static void Main(string[] args)
        {
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            var gameplayManager = serviceProvider.GetService<GameplayManager>();
            gameplayManager.Init();
            gameplayManager.Start();
        }

        //setup DI
        //Add GameOptions as singleton so that it can be used anywhere, after it has been bound to a corresponding object
        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            var config = LoadConfiguration();
            services.AddSingleton(config);

            services.AddTransient<GameplayManager>();

            services.AddTransient<IMessageProcessor, ConsoleMessageProcessor>();
            var gameOptions = new GameOptions();
            config.GetSection(nameof(GameOptions)).Bind(gameOptions);
            services.AddSingleton(gameOptions);

            return services;
        }

        //load the configuration from appsettings JSON file
        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }
    }
}
