using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KorytoApp.Models;
using KorytoApp.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KorytoApp.ViewModels
{
    public partial class UserViewModel : ObservableObject
    {
        private readonly UserService _userService;

        public UserViewModel()
        {
            _userService = new UserService(Data.AppDatabase.GetConnection());
            LoadUserCommand.Execute(null);
        }
        public ObservableCollection<GenderOption> GenderOptions { get; } = new()
{
        new GenderOption { GenderLabel = "Mężczyzna", GenderValue = "M" },
        new GenderOption { GenderLabel = "Kobieta", GenderValue = "F" }
        };

        public ObservableCollection<ActivityOption> ActivityOptions { get; } = new()
{
        new ActivityOption { ActivityLabel = "Brak ruchu", ActivityValue = 1.2 },
        new ActivityOption { ActivityLabel = "Lekka aktywność", ActivityValue = 1.375 },
        new ActivityOption { ActivityLabel = "Średnia aktywność", ActivityValue = 1.55 },
        new ActivityOption { ActivityLabel = "Duża aktywność", ActivityValue = 1.725 },
        new ActivityOption { ActivityLabel = "Bardzo duża aktywność", ActivityValue = 1.9 }
        };

        [ObservableProperty] private int userId = 0;
        [ObservableProperty] private string name = "name";
        [ObservableProperty] private int age = 0;
        [ObservableProperty] private ActivityOption selectedActivity = new ActivityOption
        {
            ActivityLabel = "Brak ruchu",
            ActivityValue = 1.2
        };
        //[ObservableProperty] private string gender;
        [ObservableProperty] private int height = 0;
        [ObservableProperty] private int weight = 0;
        [ObservableProperty] private GenderOption selectedGender = new GenderOption
        {
            GenderLabel = "Mężczyzna",
            GenderValue = "M"
        };
        [ObservableProperty] private bool isEdit = false;
        [ObservableProperty] public bool isReadOnly = true;

        [ObservableProperty] public double bmi = 0;
        [ObservableProperty] public double bmr = 0;
        [ObservableProperty] public double water = 0;
        [ObservableProperty] public double tdee = 0;

        public string BmiText => $"BMI: {Bmi:F1}";
        public string BmrText => $"BMR: {Bmr:F0} kcal";
        public string WaterText => $"Woda: {Water:F2}l";
        public string TdeeText => $"TDEE (dzienne zapotrzebowanie): {Tdee:F0} kcal";

        public class GenderOption
        {
            public string GenderLabel { get; set; }     // widoczne w Pickerze
            public string GenderValue { get; set; }     // zapisywane do bazy

            public override string ToString() => GenderLabel; // żeby Picker wyświetlał Label
        }

        public class ActivityOption
        {
            public string ActivityLabel { get; set; }     // widoczne w Pickerze
            public double ActivityValue { get; set; }     // zapisywane do bazy

            public override string ToString() => ActivityLabel; // żeby Picker wyświetlał Label
        }

        [RelayCommand]
        private async Task SaveUser()
        {
            if (string.IsNullOrWhiteSpace(Name) || Height <= 0 || Weight <= 0)
                return;
            var existingUser = await _userService.GetUserAsync();
            var user = new User
            {
                UserName = Name,
                UserAge = Age,
                UserGender = SelectedGender?.GenderValue,
                UserHeight = Height,
                UserWeight = Weight,
                UserActivity = (double)(SelectedActivity?.ActivityValue)

            };

            if (existingUser != null)
            {
                user.Id = existingUser.Id; // ważne: przypisz ID istniejącego użytkownika
                await _userService.UpdateUser(user);
            }
            else
            {
                await _userService.AddUser(user);
            }
            ToggleEdit();
            BMICalculate(Weight, Height);
            BMRCalculate(Weight, Height, Age);
            WaterCalculate(Weight);
            TDEECalculate(Bmr);
        }



        [RelayCommand]
        private async Task LoadUser()
        {
            var user = await _userService.GetUserAsync();
            if (user != null)
            {
                UserId = user.Id;
                Name = user.UserName;
                Age = user.UserAge;
                SelectedGender = GenderOptions.FirstOrDefault(g => g.GenderValue == user.UserGender);
                Height = user.UserHeight;
                Weight = user.UserWeight;
                SelectedActivity = ActivityOptions.FirstOrDefault(h => h.ActivityValue == user.UserActivity);
            }


            BMICalculate(Weight, Height);
            BMRCalculate(Weight, Height, Age);
            WaterCalculate(Weight);
            TDEECalculate(Bmr);
        }
        [RelayCommand]
        private void ToggleEdit()
        {
            IsEdit = !IsEdit;
            IsReadOnly = !IsReadOnly;
        }

        private void BMICalculate(int weight, int height)
        {
            double heightInMeters = height / 100.0;
            Bmi = weight / (heightInMeters * heightInMeters);
            OnPropertyChanged(nameof(BmiText));
        }

        private void BMRCalculate(int weight, int height, int age)
        {
            if (SelectedGender?.GenderValue == "M")
            {
                Bmr = 10 * weight + 6.25 * height - 5 * age + 5;
            }
            else
            {
                Bmr = 10 * weight + 6.25 * height - 5 * age - 161;
            }
            
            OnPropertyChanged(nameof(BmrText));
        }

        private void WaterCalculate(int weight)
        {
            double totalWater = weight * 0.033;
            double plainWater = totalWater * 0.7;

            // Dodatkowy limit bezpieczeństwa
            Water = Math.Min(plainWater, 3.5); // nie więcej niż 3 litry
            OnPropertyChanged(nameof(WaterText));

        }

        private void TDEECalculate(double bmr)
        {
            if (SelectedActivity != null)
            {
                Tdee = (double)(bmr * SelectedActivity.ActivityValue);
            }
            else
            {
                Tdee = (double)(bmr * 1.2);
            }
            OnPropertyChanged(nameof(TdeeText));

        }


    }
}
