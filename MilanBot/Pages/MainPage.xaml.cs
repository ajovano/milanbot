using MilanBot.ViewModels;
using MilanBotLib.ADO;

namespace MilanBot.Pages;

public partial class MainPage : ContentPage
{
    private AdoItemsList items;

    public MainPage()
    {
        this.InitializeComponent();
        this.items = new AdoItemsList();
        this.BindingContext = this.items;
    }

    private void PauseTracking(object sender, EventArgs e)
    {
        this.items.PauseTracking();
    }

    private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        items.OnItemSelected(e.SelectedItem as ADOWorkItem);
    }

    private void RefreshADOItems(object sender, EventArgs e)
    {
        _ = this.items.RefreshItems();
    }
}

