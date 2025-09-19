using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TestTechApp.Services;

namespace TestTechApp
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            // logging – works once the package is installed
            services.AddLogging(builder =>
            {
                builder.AddConsole();            
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // register typed HttpClient services
            services.AddHttpClient<IAuthService, AuthService>();
            services.AddHttpClient<IProductService, ProductService>();
            services.AddHttpClient<IDummyLoginService, DummyLoginService>();

            var provider = services.BuildServiceProvider();
            var authService = provider.GetRequiredService<IAuthService>();
            var productService = provider.GetRequiredService<IProductService>();
            var dummyLogin = provider.GetRequiredService<IDummyLoginService>();

            var (username, password) = await dummyLogin.GetRandomUserAsync();
            Console.WriteLine($"Using random user: {username} / {password}");

            var login = await authService.LoginAsync(username, password);
            if (login == null)
            {
                Console.WriteLine("Login failed. Exiting.");
                return;
            }

            var token = login.AccessToken;
            var smartphones = await productService.GetMostExpensiveSmartphonesAsync(token, 3);

            Console.WriteLine("\nTop 3 expensive smartphones:");
            foreach (var p in smartphones)
                Console.WriteLine($"{p.Brand} - {p.Title} - ${p.Price}");
            Console.Write("\nEnter percentage increase (e.g. 10 for 10%): ");

            if (!decimal.TryParse(Console.ReadLine(), out var percent))
            {
                Console.WriteLine("Invalid percentage. Exiting.");
                return;
            }

            foreach (var phone in smartphones)
            {
                var newPrice = phone.Price + (phone.Price * percent / 100);
                var updated = await productService.UpdateProductPriceAsync(token, phone.Id, newPrice);
                if (updated != null)
                    Console.WriteLine($"Updated: {updated.Brand} - {updated.Title} - ${updated.Price}");
            }
        }
    }
}