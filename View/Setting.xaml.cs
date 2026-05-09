using Mystic_ToDo_MAUI_.ViewModel;

namespace Mystic_ToDo_MAUI_.View;

public partial class Setting : ContentPage
{
	public Setting(SettingViewModel settingViewModel)
	{
        InitializeComponent();
        BindingContext = settingViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is SettingViewModel vm)
        {
            await vm.LoadDataAsync();

        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is SettingViewModel vm)
        {
            vm.CancelSync();
        }
    }
}