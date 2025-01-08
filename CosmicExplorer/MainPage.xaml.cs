using System;
using System.Linq;

namespace CosmicExplorer
{
    public partial class MainPage : ContentPage
    {
        private bool gridGenerated;
        private bool populated;
        private Spacecraft? spacecraft;
        public BodyService bodyService;

        private List<Planet> planets = new List<Planet>();
        private List<Image> fuelcans = new List<Image>();

        //used to store nearby planet
        private Planet? nearbyPlanet;
        private Button? planetExploreBtn;

        IDispatcherTimer gameTimer, fuelTimer;

        //hard code for cols and rows of map
        int cols = 15; int rows = 15;

        Random random = new Random();

        public MainPage()
        {
            InitializeComponent();
            bodyService = new BodyService();

            // User is on a mobile device
            if (DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
            {
                GameLayout.WidthRequest = 400;
                GameLayout.HeightRequest = 600;

                GridPageContent.WidthRequest = 400;
                GridPageContent.HeightRequest = 600;

                rows = 10;
                cols = 20;
            }
            
            
            
        }

        //This function will create a Grid that will later be populated with planets and a spacecraft
        private void GenerateGrid()
        {
            if (gridGenerated)
            {
                return;
            }

            gridGenerated = true;
            //positions = new int[numbrows, numbrows];

            for (int i = 0; i < rows; ++i)
            {
                GridPageContent.AddRowDefinition(new RowDefinition());
                GridPageContent.AddColumnDefinition(new ColumnDefinition());
            }

        }

        private void SpawnSpacecraft()
        {
            spacecraft = new Spacecraft(50, GridPageContent, this);
        }

        //Function will populate a location on the grid with a planet
        private void Populate()
        {
            if (populated) return;

            //Cycle through each element and spawn a planet based on a probability
            for (int i = 0; i < rows; i++)
            {

                for (int j = 0; j < cols; j++)
                {
                    if (random.NextDouble() > 0.6)
                    {
                        SpawnPlanet(i, j);
                    } else if (random .NextDouble() > 0.95)
                    {
                        SpawnFuel(i, j);
                    }

                }

            }

            populated = true;
        }

        private void SpawnPlanet(int col, int row)
        {
            //Should add a seperate planet class ideally, WORK IN PROGRESS

            Planet planet = new Planet(row, col);


            GridPageContent.Add(planet.PlanetImage, col, row);

            planets.Add(planet);
        }

        private void SpawnFuel(int col, int row)
        {
            Image jerryCan = new Image();
            jerryCan.Source = "fuelcan.png";

            fuelcans.Add(jerryCan);


            GridPageContent.Add(jerryCan, col, row);
        }

        private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {


            Point p = e.GetPosition((AbsoluteLayout)sender).Value;
            spacecraft.MoveTo(p);
        }

        private async void OnStartClicked(object sender, EventArgs e)
        {
            MainLayout.Children.Remove(StartBtn);
            //fuelDisplay.Text = "Amt in list" + bodyService.GetBodyList().ElementAt(1).englishName;

            //First Step - Generate the Grid needed!
            GenerateGrid();

            //Second Step - Populate the said grid with various planets
            Populate();

            //Third Step - Add the user controlled Spacecraft
            SpawnSpacecraft();

            //Adds a timer which runs a function deteriming if the spacecraft is near a planet
            gameTimer = Dispatcher.CreateTimer();
            gameTimer.Interval = TimeSpan.FromSeconds(0.25);
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            //Adds a timer which runs a function that depletes the fuel level when the spacecraft is moving
            /*
            fuelTimer = Dispatcher.CreateTimer();
            fuelTimer.Interval = TimeSpan.FromSeconds(0.5);
            fuelTimer.Tick += FuelTimer_Tick;
            fuelTimer.Start();*/

        }

        private void GameTimer_Tick(object sender , EventArgs e)
        {

            foreach (Planet planet in planets)
            {
                
                if(spacecraft.Position.Distance(new Point(planet.PlanetImage.X, planet.PlanetImage.Y)) < 25)
                {
                    //planetDisplay.Text = bodyService.GetBodyList().ElementAt(planet.Index).englishName;
                    planetDisplay.Text = Planet.getNumberOfUncoveredPlanets().ToString();

                    //Add button which when pressed opens page with info on plnet
                    Button button = new Button();
                    button.Text = "Explore Planet";
                    button.BackgroundColor = Colors.Aquamarine;
                    button.TextColor = Colors.White;
                    button.Margin = new Thickness(10);

                    //Event handler for button
                    button.Clicked += Button_Clicked;

                    GameLayout.Children.Add(button);

                    planetExploreBtn = button;
                    nearbyPlanet = planet;

                } else
                {
                    //check if old button is there, remove it
                    if (planetExploreBtn != null)
                    {
                        MainLayout.Children.Remove(planetExploreBtn);
                    }
                }

            }

            foreach(Image fuelcan in fuelcans)
            {
                if (spacecraft.Position.Distance(new Point(fuelcan.X, fuelcan.Y)) < 10)
                {
                    fuelcan.ScaleTo(0);

                    fuelcans.Remove(fuelcan);

                    GridPageContent.Children.Remove(fuelcan);

                    spacecraft.FuelcanCollected();
                    break;
                }
            }

            if(spacecraft.IsMoving)
            {
                spacecraft.DepleteFuel();

                fuelDisplay.Text = "Fuel Remaining: " + spacecraft.fuelPercentage;
            }

        }

        private async void Button_Clicked(object? sender, EventArgs e)
        {
            
            if (nearbyPlanet != null)
            {
                await Navigation.PushAsync(new PlanetInfoPage(bodyService.GetBodyList().ElementAt(nearbyPlanet.Index), nearbyPlanet));
                nearbyPlanet.isUncovered = true;
            }
            
        }

        private async void Settings_Clicked(object sender, EventArgs e)
        {

            await Navigation.PushAsync(new SettingsPage());
 

        }

    }



}
