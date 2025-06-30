using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KorytoApp.Models;
using KorytoApp.Services;
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

        [ObservableProperty] private string name;
        [ObservableProperty] private string gender;
        [ObservableProperty] private int height;
        [ObservableProperty] private int weight;

        [ObservableProperty] private bool isEdit = false;
        [ObservableProperty] public bool isReadOnly = true;

        [RelayCommand]
        private async Task SaveUser()
        {
            if (string.IsNullOrWhiteSpace(Name) || Height <= 0 || Weight <= 0)
                return;

            var user = new User
            {
                UserName = Name,
                UserGender = Gender,
                UserHeight = Height,
                UserWeight = Weight
            };

            await _userService.AddUser(user);
            ToggleEdit();
        }

        [RelayCommand]
        private async Task LoadUser()
        {
            var user = await _userService.GetUserAsync();
            if (user != null)
            {
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
