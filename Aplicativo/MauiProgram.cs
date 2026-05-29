 using Aplicativo.Resources.Scaffolding;
using Aplicativo.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using System.Reflection;
using ZXing.Net.Maui.Controls;
namespace Aplicativo
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            //Adiciona o appsettings.json como recurso incorporado
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("Aplicativo.appsettings.json");
            if (stream != null)
            {
                var config = new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build();
                builder.Configuration.AddConfiguration(config);
            }

            builder
                .UseMauiApp<App>()
                .UseBarcodeReader() // Serviço de leitura de código de barras
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddMudServices();
            
            builder.Services.AddHttpClient<HttpService>(options =>
            {
                var url = builder.Configuration["Configs:BaseUrl"]; //Colocar um endereço no appsettings.json
                options.BaseAddress = new Uri(url is null ? "Colocar o endereço do backend no BaseURL" : url);
            });
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
