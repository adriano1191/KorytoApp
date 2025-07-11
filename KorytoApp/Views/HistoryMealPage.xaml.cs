using KorytoApp.Data;
using KorytoApp.Services;
using KorytoApp.ViewModels;

namespace KorytoApp.Views
{
    public partial class HistoryMealPage : ContentPage
    {
        private readonly HistoryMealModel _viewModel;
        public HistoryMealPage()
        {
            InitializeComponent();
            var mealService = new MealService(AppDatabase.GetConnection());
            _viewModel = new HistoryMealModel(mealService);
            BindingContext = _viewModel;

            //var today = DateTime.Today;
           // _ = _viewModel.LoadMealsForDate(today);
        }
        private async void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            await _viewModel.LoadMealsForDate(e.NewDate);

        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadMealsForDate(DateTime.Today);

        }
    }
}
