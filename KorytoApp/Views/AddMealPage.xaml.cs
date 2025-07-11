using CommunityToolkit.Mvvm.Messaging;
using KorytoApp.Data;
using KorytoApp.Messages;
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
            if (string.IsNullOrWhiteSpace(nameEntry.Text) || !int.TryParse(calEntry.Text, out int calories) || !int.TryParse(waterEntry.Text, out int water))
            {
                await DisplayAlert("Błąd", "Wprowadź poprawne dane", "OK");
                return;
            }

            var meal = new Meal
            {
                Name = nameEntry.Text,
                Calories = calories,
                Water = water,
                Time = DateTime.Now
            };

            await _mealService.AddMeal(meal);
            WeakReferenceMessenger.Default.Send(new MealAddedMessage(calories, water));

            await Navigation.PopAsync();
        }
    }
}

