using MilanBot.ViewModels;
using System.Collections.ObjectModel;

namespace MilanBot;

public partial class MainPage : ContentPage
{
	private AdoItemsList items;

	public MainPage()
	{
		InitializeComponent();
		this.items = new AdoItemsList();
		BindingContext = this.items;
    }

	private void PauseTracking(object sender, EventArgs e)
	{
        // this.items.PauseTracking();
    }

	private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
	{
		this.items.OnItemSelected(e.SelectedItem as AdoItem);

    }
}

