namespace CosmicExplorer;

public partial class PlanetInfoPage : ContentPage
{
	private Planet Planet { get; set; }
	private MainPage MainPage { get; set; }
	private Body Body { get; set; }
	public PlanetInfoPage(Body body, Planet planet)
	{
		InitializeComponent();
		this.Planet = planet;
		this.Body = body;

		DisplayPlanetInfo();
    }

	private void DisplayPlanetInfo()
	{
        PlanetName.Text += Body.englishName;
		discoveredBy.Text += Body.discoveredBy;
		discoveryDate.Text += Body.discoveryDate;
		gravity.Text += Body.gravity + " m/s^2";
		density.Text += Body.density + " g/cm^3";
		meanRadius.Text += Body.meanRadius + " km";
	}
}