using KorytoApp.ViewModels;
using KorytoApp.Views;

namespace KorytoApp
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _vm;

        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
        }

        private async void OnAddMealClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddMealPage());
        }
        private async void OnUserDataClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserData());
        }
        private async void OnHistoryMealClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HistoryMealPage());
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            _vm.LoadMealsForToday();
        }

    }
}
