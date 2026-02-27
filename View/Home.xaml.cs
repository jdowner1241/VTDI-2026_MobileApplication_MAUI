using System.Collections.ObjectModel;

namespace Mystic_ToDo_MAUI_.View;

public partial class Home : ContentPage
{
	public ObservableCollection<Folders> FolderList { get; set; } 
	public Home()
	{
		InitializeComponent();
        CustomInitial();
	}

    private void CustomInitial()
    {
        FolderList = new ObservableCollection<Folders>
        {
            new Folders { FolderName = "Default", FolderIcon= new Image{ Source = "dotnet_bot.png" } },
            new Folders { FolderName = "Folder1", FolderIcon= new Image{ Source = "dotnet_bot.png" } },
            new Folders { FolderName = "Folder2"}
        };
    }
}
public class Folders
{
    public required string FolderName { get; set; }
    public Image? FolderIcon { get; set; }
    //public ObservableCollection<String> TodoList { get; set; }

}