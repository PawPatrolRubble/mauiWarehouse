using MauiApp2.ViewModels;

namespace MauiApp2.Views;

public partial class FileReaderView : ContentPage
{
	public FileReaderView()
	{
		InitializeComponent();

		BindingContext = new FileReaderViewModel(this);
	
	}

}