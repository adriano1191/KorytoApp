using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KorytoApp.Models;
using KorytoApp.Services;
using System.Collections.ObjectModel;
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
        public ObservableCollection<string> Genders { get; } = new() { "Mężczyzna", "Kobieta" };

        [ObservableProperty] private int userId;
        [ObservableProperty] private string name;
        //[ObservableProperty] private string gender;
        [ObservableProperty] private int height;
        [ObservableProperty] private int weight;
        [ObservableProperty] private string gender;
        [ObservableProperty] private bool isEdit = false;
        [ObservableProperty] public bool isReadOnly = true;

      

        [RelayCommand]
        private async Task SaveUser()
        {
            if (string.IsNullOrWhiteSpace(Name) || Height <= 0 || Weight <= 0)
                return;
            var existingUser = await _userService.GetUserAsync();
            var user = new User
            {
                UserName = Name,
                UserGender = Gender,
                UserHeight = Height,
                UserWeight = Weight
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
        }



        [RelayCommand]
        private async Task LoadUser()
        {
            var user = await _userService.GetUserAsync();
            if (user != null)
            {
                UserId = user.Id;
                Name = user.UserName;
                Gender = user.UserGender;
                Height = user.UserHeight;
                Weight = user.UserWeight;
            }
        }
        [RelayCommand]
        private void ToggleEdit()
        {
            IsEdit = !IsEdit;
            IsReadOnly = !IsReadOnly;
        }

    }
}
