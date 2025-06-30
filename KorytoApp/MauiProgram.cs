using KorytoApp.Data;
using KorytoApp.Services;
using KorytoApp.ViewModels;
using KorytoApp.Views;
using Microsoft.Extensions.Logging;

namespace KorytoApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            var db = AppDatabase.GetConnection();
            builder.Services.AddSingleton(new MealService(db));
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<AddMealPage>();
            builder.Services.AddSingleton(new UserService(db));
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddTransient<UserViewModel>();
            builder.Services.AddTransient<UserData>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
