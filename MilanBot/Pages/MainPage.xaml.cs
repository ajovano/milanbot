using MilanBot.ViewModels;

namespace MilanBot.Pages;

public partial class MainPage : ContentPage
{
    private AdoItemsList items;

    public MainPage()
    {
        InitializeComponent();
        items = new AdoItemsList();
        BindingContext = items;
    }

    private void PauseTracking(object sender, EventArgs e)
    {
        // this.items.PauseTracking();
    }

    private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        items.OnItemSelected(e.SelectedItem as AdoItem);

    }

    private void Configure_Clicked(object sender, EventArgs e)
    {

    }
}

