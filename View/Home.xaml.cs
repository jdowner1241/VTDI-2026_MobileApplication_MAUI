using Mystic_ToDo_MAUI_.ViewModel;
using System.Collections.ObjectModel;

namespace Mystic_ToDo_MAUI_.View;

public partial class Home : ContentPage
{
	public Home(HomeViewModel homeViewModel)
	{
		InitializeComponent();
        BindingContext = homeViewModel;
  
	}

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (BindingContext is HomeViewModel vm)
        {
            vm.Cancel();
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is HomeViewModel vm)
        {
            await vm.LoadDataAsync();
        }
    }
}