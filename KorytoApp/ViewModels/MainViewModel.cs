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
using CommunityToolkit.Mvvm.Messaging;
using KorytoApp.Messages;


namespace KorytoApp.ViewModels
{

    public partial class MainViewModel : ObservableObject
    {
        bool firstStartSpeechChoice = false; // zmienna do sprawdzenia, czy to pierwszy start aplikacji
        public class ChartData // dane do wykresów
        {
            public string Label { get; set; } // etykieta do wykresu
            public double Value { get; set; } // wartość do wykresu
        }
        public ObservableCollection<ChartData> CaloriesData { get; set; } // dane do wykresów
        public ObservableCollection<ChartData> WaterData { get; set; } // dane do wykresów

        public List<Brush> CustomBrushesCalorier { get; set; } // kolory do wykresów
        public List<Brush> CustomBrushesWater { get; set; } // kolory do wykresów

        private readonly MealService _mealService; // serwis do posiłków
        private readonly UserService _userService; // serwis do użytkownika

        public ObservableCollection<Meal> MealsToday { get; } = []; // lista posiłków na dzisiaj

        [ObservableProperty]
        private int totalCalories; //   całkowita ilość spożytych kalorii

        [ObservableProperty]
        private int totalWater; // całkowita ilość wypitej wody w mililitrach
        [ObservableProperty]
        public double tdee = 0; // całkowita przemiana materii (TDEE) – ilość kalorii do spożycia w ciągu dnia
        [ObservableProperty]
        public double waterLimit = 0; // limit wody do wypicia w ciągu dnia

        [ObservableProperty]
        public string caloriesLabelText;    // tekst etykiety kalorii

        [ObservableProperty]
        public string waterLabelText;       // tekst etykiety wodą

        private readonly SpeechBubbleService _speechService = new();    // serwis do dymków z wiadomościami

        private string _speechBubbleText; // tekst dymku z wiadomościami
        private static bool _isMessengerRegistered = false;     // flaga do sprawdzenia, czy wiadomości są zarejestrowane

        /// <summary>
        /// Tekst do wyświetlenia w dymku z wiadomościami.
        /// </summary>
        public string SpeechBubbleText
        {
            get => _speechBubbleText;
            set { _speechBubbleText = value; OnPropertyChanged(); }
        }

        private bool _isSpeechVisible;
        public bool IsSpeechVisible
        {
            get => _isSpeechVisible;
            set { _isSpeechVisible = value; OnPropertyChanged(); }
        }

        public MainViewModel(MealService mealService, UserService userService)
        {
            _mealService = mealService;
            _userService = userService;

            TDEEAndWaterCalculate();
            //LoadMealsForToday();
            // inicjalizacja danych kolorów do wykresów
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

            //wyświetlanie wiadomości powitalenj



            if (!_isMessengerRegistered)
            {
                WeakReferenceMessenger.Default.Register<MealAddedMessage>(this, async (r, m) =>
                {
                    SpeechChoice("AddMeal", m.Calories, m.Water);
                    //await LoadMealsForToday();
                });
                _isMessengerRegistered = true;
            }
        }



        /// <summary>
        /// Ładuje posiłki na dzisiaj z serwisu MealService.
        /// </summary>
        public async Task LoadMealsForToday()
        {
            MealsToday.Clear();     // czyszczenie listy posiłków na dzisiaj
            var meals = await _mealService.GetMealsForDate(DateTime.Today);     // pobieranie posiłków z serwisu
                                                                                // System.Diagnostics.Debug.WriteLine($"[DEBUG] Pobrano {meals.Count} posiłków");          
            foreach (var meal in meals)
                MealsToday.Add(meal); // dodawanie posiłków do listy

            TotalCalories = MealsToday.Sum(m => m.Calories);    // całkowita ilość spożytych kalorii
            TotalWater = MealsToday.Sum(w => w.Water);          // całkowita ilość wypitej wody w mililitrach


            CaloriesData = new ObservableCollection<ChartData>  // dane do wykresów kalorii
{
            new ChartData { Label = "Spożyte", Value = TotalCalories },
            new ChartData { Label = "Pozostało", Value = Math.Max(0, Tdee - TotalCalories) }
        };

            WaterData = new ObservableCollection<ChartData>     // dane do wykresów wody
        {
            new ChartData { Label = "Wypite", Value = TotalWater / 1000.0 },
            new ChartData { Label = "Pozostało", Value = Math.Max(0, WaterLimit - TotalWater / 1000.0) }
        };




            OnPropertyChanged(nameof(CaloriesData)); // odświeżenie danych wykresu kalorii
            OnPropertyChanged(nameof(WaterData));   // odświeżenie danych wykresu wody

            CaloriesLabelText = $"🥩 Kalorie: {TotalCalories} / {Tdee} kcal 🥩"; // tekst etykiety kalorii
            WaterLabelText = $"💧Woda: {TotalWater / 1000.0:F2} / {WaterLimit:F2} l💧"; // tekst etykiety wody

            SpeechChoice("StartApp", 0, 0); // wybór wiadomości na podstawie kontekstu

        }
        /// <summary>
        /// Usuwa posiłek z bazy danych i odświeża listę posiłków na dzisiaj.
        /// </summary>
        /// <param name="meal"></param>
        /// <returns></returns>
        [RelayCommand]
        public async Task DeleteMeal(Meal meal)
        {
            await _mealService.DeleteMeal(meal);
            LoadMealsForToday(); // odświeżenie listy
        }

        /// <summary>
        /// Oblicza TDEE (Total Daily Energy Expenditure) i limit wody do wypicia na podstawie danych użytkownika.
        /// </summary>
        private async void TDEEAndWaterCalculate()
        {
            double bmr = 0;     // podstawowa przemiana materii (BMR)
            double activity = 0;    // współczynnik aktywności użytkownika


            // pobranie danych użytkownika z serwisu UserService
            var user = await _userService.GetUserAsync();
            if (user != null)
            {

                // ustawienie współczynnika aktywności na podstawie danych użytkownika
                if (user.UserGender == "M")
                {
                    bmr = 10 * user.UserWeight + 6.25 * user.UserHeight - 5 * user.UserAge + 5;
                }
                else
                {
                    bmr = 10 * user.UserWeight + 6.25 * user.UserHeight - 5 * user.UserAge - 161;
                }


                Tdee = bmr * user.UserActivity;     // obliczenie TDEE

                OnPropertyChanged(nameof(Tdee));

                double totalWater = user.UserWeight * 0.033;    // obliczenie całkowitej ilości wody do wypicia w ciągu dnia (w litrach)
                double plainWater = totalWater * 0.7;       // // 70% wody to woda pitna, reszta to woda z jedzenia

                // Dodatkowy limit bezpieczeństwa
                WaterLimit = Math.Min(plainWater, 3.5); // nie więcej niż 3.5 litry
                OnPropertyChanged(nameof(WaterLimit));

            }
        }


        /// <summary>
        /// Wyświetla losową wiadomość w dymku z wiadomościami na podstawie kontekstu.
        /// </summary>
        /// <param name="context"></param>
        public void ShowRandomSpeech(SpeechContext context)
        {
            SpeechBubbleText = _speechService.GetRandomSpeech(context);
            IsSpeechVisible = true;
        }

        /// <summary>
        /// Wybiera wiadomość do wyświetlenia w dymku z wiadomościami na podstawie kontekstu i danych o posiłku.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="calories"></param>
        /// <param name="water"></param>
        public void SpeechChoice(string type, int calories, int water)
        {

            // Wybór wiadomości na podstawie kontekstu
            if (type == "StartApp" && !firstStartSpeechChoice)
            {
                if (TotalCalories > Tdee)  // jeśli przekroczone kalorie
                {
                    ShowRandomSpeech(SpeechContext.MaxCalories); // przekroczone kalorie
                }
                else
                {
                    ShowRandomSpeech(SpeechContext.Start); // powitanie, jeśli nie przekroczone kalorie
                }
                firstStartSpeechChoice = true; // ustawienie flagi, aby nie wyświetlać powitania ponownie
            }
            else if (type == "AddMeal")     // jeśli dodano posiłek
            {
                if (calories == 0 && water > 0) // tylko woda
                {
                    ShowRandomSpeech(SpeechContext.AddWater);
                }
                else if (calories > 0 && water == 0 && TotalCalories < Tdee) // tylko żarcie
                {
                    ShowRandomSpeech(SpeechContext.AddMeal);
                }
                else if (calories > 0 && water > 0 && TotalCalories < Tdee) // jedno i drugie, możesz dodać nowy kontekst
                {
                    ShowRandomSpeech(SpeechContext.AddMealAndWater);
                }
                else if (TotalCalories >= Tdee) // przekroczone kalorie
                {
                    ShowRandomSpeech(SpeechContext.MaxCalories);
                }
            }
        }

    }
}