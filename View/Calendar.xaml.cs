using Mystic_ToDo_MAUI_.ViewModel;

namespace Mystic_ToDo_MAUI_.View;

public partial class Calendar : ContentPage
{
	public Calendar(CalendarViewModel calendarViewModel)
	{
		InitializeComponent();
		BindingContext = calendarViewModel;
    }
    //protected override void OnDisappearing()
    //{
    //    base.OnDisappearing();

    //    if (BindingContext is CalendarViewModel vm)
    //    {
    //        vm.Cancel();
    //    }
    //}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is CalendarViewModel vm)
        {
            await vm.LoadDataAsync();
        }
    }

}

