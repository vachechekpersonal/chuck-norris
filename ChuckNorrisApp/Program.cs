using ChuckNorrisApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = new HostBuilder()
               .ConfigureServices((hostContext, services) =>
               {
                   services.AddHttpClient();
                   services.AddSingleton<IJokesApp, JokesApp>();
                   services.AddSingleton<IJokesClient, JokesClient>();
               }).UseConsoleLifetime();

var host = builder.Build();

using (var serviceScope = host.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;

    try
    {
        var jokesApp = services.GetRequiredService<IJokesApp>();

        void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("Welcome to Chuck Norris jokes app.");
            Console.WriteLine("Choose your command from the list below to continue:");
            Console.WriteLine("Press j to get a new joke");
            Console.WriteLine("Press p to see the previous joke");
            Console.WriteLine("Press n to see the next joke");
            Console.WriteLine("Press q to exit");
            Console.WriteLine();
        }

        ShowMenu();

        var keyPressed = Console.ReadKey();

        while (keyPressed.KeyChar != 'q')
        {
            ShowMenu();

            var result = keyPressed.KeyChar switch
            {
                'j' => await jokesApp.New(),
                'p' => jokesApp.Previous(),
                'n' => jokesApp.Next(),
                _ => "Invalid command",
            };

            Console.WriteLine(result);

            keyPressed = Console.ReadKey();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error Occured! {ex.Message}");
    }
}

return 0;