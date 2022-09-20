using MilanBot.ViewModels;

namespace MilanBot.Pages;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();

		this.BindingContext = new UserSettings();
    }
}

