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

    private void RefreshADOItems(object sender, EventArgs e)
    {
        _ = this.items.RefreshItems();
    }
}

