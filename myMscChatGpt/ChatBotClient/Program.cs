using ChatBotProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChatBotClient
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<Form1>();
                   
                    services.AddSingleton<IChatBotClient, ChatBotProxy.ChatBotClient>();

                    // keep as last
                    services.AddHttpClient<IChatBotClient, ChatBotProxy.ChatBotClient>(c =>
                    {
                        c.BaseAddress = new Uri("https://localhost:7144");

                    });
                });
            var host = builder.Build();
            using var scope = host.Services.CreateScope();
            var form = scope.ServiceProvider.GetRequiredService<Form1>();
            Application.Run(form);
        }
    }
}