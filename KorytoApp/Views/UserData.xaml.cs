using KorytoApp.ViewModels;

namespace KorytoApp.Views
{
    public partial class UserData : ContentPage
    {
        public UserData()
        {
            InitializeComponent();
            BindingContext = new UserViewModel(); // bez DI
        }
    }
}
