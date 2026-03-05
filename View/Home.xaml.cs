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
}