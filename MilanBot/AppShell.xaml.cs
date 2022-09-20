using System.Diagnostics;

namespace MilanBot;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
    }
    async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync($"///settings");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"err: {ex.Message}");
        }
    }
}
