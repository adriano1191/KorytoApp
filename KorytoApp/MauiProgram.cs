using KorytoApp.Data;
using KorytoApp.Services;
using KorytoApp.ViewModels;
using KorytoApp.Views;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;
using Syncfusion.Maui;

namespace KorytoApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCeUx3Qnxbf1x1ZFJMYlpbRH5PMyBoS35Rc0VkWH9ed3ZTRmheWEx1VEFd");

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureSyncfusionCore();



            var db = AppDatabase.GetConnection();
            builder.Services.AddSingleton(new MealService(db));
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<AddMealPage>();
            builder.Services.AddSingleton<UserService>(s => new UserService(db));
            builder.Services.AddTransient<UserViewModel>();
            builder.Services.AddTransient<UserData>();
            builder.ConfigureSyncfusionCore();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
