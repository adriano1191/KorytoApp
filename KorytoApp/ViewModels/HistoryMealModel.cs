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
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace KorytoApp.ViewModels
{
    public partial class HistoryMealModel : ObservableObject
    {
        private readonly MealService _mealService;
        public ObservableCollection<Meal> Meals { get; } = new();

        [ObservableProperty]
        private int totalCalories;

        [ObservableProperty]
        private int totalWater;

        private DateTime dateSelected;

        public HistoryMealModel(MealService mealService)
        {
            _mealService = mealService;
        }

        public async Task LoadMealsForDate(DateTime date)
        {
            dateSelected = date;
            var meals = await _mealService.GetMealsForDate(date);
            Meals.Clear();
            foreach (var meal in meals)
                Meals.Add(meal);

            TotalCalories = meals.Sum(m => m.Calories);
            TotalWater = meals.Sum(w => w.Water);
        }

        [RelayCommand]
        private async Task DeleteMeal(Meal meal)
        {
            if (meal == null)
                return;

            await _mealService.DeleteMeal(meal);
            Meals.Remove(meal);
            await LoadMealsForDate(dateSelected); // odświeżenie listy po usunięciu
        }
    }
}
