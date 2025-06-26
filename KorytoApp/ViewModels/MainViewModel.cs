using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KorytoApp.Models;
using KorytoApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KorytoApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly MealService _mealService;

        public ObservableCollection<Meal> MealsToday { get; } = [];

        [ObservableProperty]
        private int totalCalories;

        public MainViewModel(MealService mealService)
        {
            _mealService = mealService;
            //LoadMealsForToday();
        }

        public async void LoadMealsForToday()
        {
            MealsToday.Clear();
            var meals = await _mealService.GetMealsForDate(DateTime.Today);
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Pobrano {meals.Count} posiłków");
            foreach (var meal in meals)
                MealsToday.Add(meal);

            TotalCalories = MealsToday.Sum(m => m.Calories);
        }
        [RelayCommand]
        public async Task DeleteMeal(Meal meal)
        {
            await _mealService.DeleteMeal(meal);
            LoadMealsForToday(); // odświeżenie listy
        }


    }
}
