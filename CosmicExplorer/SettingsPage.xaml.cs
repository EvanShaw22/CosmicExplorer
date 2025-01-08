namespace CosmicExplorer;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();

		uncovered.Text = "Planets Uncovered: " + Planet.getNumberOfUncoveredPlanets().ToString() + "/366";
	}

    private async void OnResetClicked(object sender, EventArgs e)
    {
		Preferences.Clear();
        await DisplayAlert("Reset", "Your progress has been reset", "OK");
    }
}