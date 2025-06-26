using KorytoApp.Data;
using KorytoApp.Models;
using KorytoApp.Services;
using System.Formats.Tar;

namespace KorytoApp.Views
{
    public partial class AddMealPage : ContentPage
    {
        private readonly MealService _mealService;

        public AddMealPage()
        {
            InitializeComponent();
            _mealService = new MealService(AppDatabase.GetConnection());
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameEntry.Text) || !int.TryParse(calEntry.Text, out int calories))
            {
                await DisplayAlert("Błąd", "Wprowadź poprawne dane", "OK");
                return;
            }

            var meal = new Meal
            {
                Name = nameEntry.Text,
                Calories = calories,
                Time = DateTime.Now
            };

            await _mealService.AddMeal(meal);
            await Navigation.PopAsync();
        }
    }
}

