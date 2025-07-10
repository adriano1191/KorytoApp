using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KorytoApp.Models;
using KorytoApp.Services;
using KorytoApp.Views;
using Microsoft.Maui;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Syncfusion.Maui.Charts;


namespace KorytoApp.ViewModels
{

    public partial class MainViewModel : ObservableObject
    {
        public class ChartData
        {
            public string Label { get; set; }
            public double Value { get; set; }
        }
        public ObservableCollection<ChartData> CaloriesData { get; set; }
        public ObservableCollection<ChartData> WaterData { get; set; }

        public List<Brush> CustomBrushesCalorier { get; set; }
        public List<Brush> CustomBrushesWater { get; set; }

        private readonly MealService _mealService;
        private readonly UserService _userService;

        public ObservableCollection<Meal> MealsToday { get; } = [];

        [ObservableProperty]
        private int totalCalories;

        [ObservableProperty]
        private int totalWater;
        [ObservableProperty] 
        public double tdee = 0;
        [ObservableProperty]
        public double waterLimit = 0;

        [ObservableProperty]
        public string caloriesLabelText;

        [ObservableProperty]
        public string waterLabelText;


        public MainViewModel(MealService mealService, UserService userService)
        {
            _mealService = mealService;
            _userService = userService;
            //LoadMealsForToday();
            TDEEAndWaterCalculate();


            CustomBrushesCalorier = new List<Brush>
            {
 
                new SolidColorBrush(Color.FromArgb("#ed3131")),
                new SolidColorBrush(Color.FromArgb("#a5a5a5"))
            };


            CustomBrushesWater = new List<Brush>
            {

                new SolidColorBrush(Color.FromArgb("#25a8e6")),
                new SolidColorBrush(Color.FromArgb("#a5a5a5"))
            };



        }



        public async void LoadMealsForToday()
        {
            MealsToday.Clear();
            var meals = await _mealService.GetMealsForDate(DateTime.Today);
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Pobrano {meals.Count} posiłków");
            foreach (var meal in meals)
                MealsToday.Add(meal);

            TotalCalories = MealsToday.Sum(m => m.Calories);
            TotalWater = MealsToday.Sum(w => w.Water);

            
            CaloriesData = new ObservableCollection<ChartData>
{
            new ChartData { Label = "Spożyte", Value = TotalCalories },
            new ChartData { Label = "Pozostało", Value = Math.Max(0, Tdee - TotalCalories) }
        };

                    WaterData = new ObservableCollection<ChartData>
        {
            new ChartData { Label = "Wypite", Value = TotalWater / 1000.0 },
            new ChartData { Label = "Pozostało", Value = Math.Max(0, WaterLimit - TotalWater / 1000.0) }
        };




            OnPropertyChanged(nameof(CaloriesData));
            OnPropertyChanged(nameof(WaterData));

            CaloriesLabelText = $"🥩 Kalorie: {TotalCalories} / {Tdee} kcal 🥩";
            WaterLabelText = $"💧Woda: {TotalWater / 1000.0:F2} / {WaterLimit:F2} l💧";

        }
        [RelayCommand]
        public async Task DeleteMeal(Meal meal)
        {
            await _mealService.DeleteMeal(meal);
            LoadMealsForToday(); // odświeżenie listy
        }


        private async void TDEEAndWaterCalculate()
        {
            double bmr = 0;
            double activity = 0;


            var user = await _userService.GetUserAsync();
            if (user != null)
            {


                if (user.UserGender == "M")
                {
                    bmr = 10 * user.UserWeight + 6.25 * user.UserHeight - 5 * user.UserAge + 5;
                }
                else
                {
                    bmr = 10 * user.UserWeight + 6.25 * user.UserHeight - 5 * user.UserAge - 161;
                }


                Tdee = bmr * user.UserActivity;

                OnPropertyChanged(nameof(Tdee));

                double totalWater = user.UserWeight * 0.033;
                double plainWater = totalWater * 0.7;

                // Dodatkowy limit bezpieczeństwa
                WaterLimit = Math.Min(plainWater, 3.5); // nie więcej niż 3 litry
                OnPropertyChanged(nameof(WaterLimit));

            }
        }






    }
}