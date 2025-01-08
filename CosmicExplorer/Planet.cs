using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmicExplorer
{
    public class Planet
    {
       
        static Random random = new Random();
        private int index;

        private int row;
        private int column;
        private Image planetImage;
        private Point pos;

        private bool uncovered = false;

        public bool isUncovered {  get { return uncovered; } set { uncovered = value; planetImage.Source = PlanetIcon(); uncoveredPlanet(); } }

        public Point Position { get { return pos; } }

        public Image PlanetImage {  get { return planetImage; } }


        //Used to reference the planet from the 366 in the API List
        public int Index { get { return index; } }

        public List<int> spawnedPlanets = new List<int>();

        public Planet(int row, int column) { 
            this.row = row;
            this.column = column;
            
            //code is also needed to ensure that previously discovered planets are removed.
            //this part is importnt as we cannot have the same planet spawned twice.
            index = random.Next(1, 367);

            //change index if planet has been discovered in a previous run
            while(hasBeenUncovered(index) || spawnedPlanets.Contains(index))
            {
                index = random.Next(1, 367);
            }

            spawnedPlanets.Add(index);

            if(isUncovered)
            {
                planetImage = new Image { Source = PlanetIcon() }; 
            } else
            {
                planetImage = new Image { Source = "undiscovered3.png" };
            }
            

            //The map is very grid like - this aims to mix it up a bit
            adjustment();

            pos = new Point(planetImage.X, planetImage.Y);
        }

        public void adjustment()
        {
            double xDistort = random.Next(-6, 6);
            double yDistort = random.Next(-6, 6);

            planetImage.TranslateTo(xDistort, yDistort);
        }

        public void uncoveredPlanet()
        {
            string uncoveredPlanets = Preferences.Get("uncoveredPlanets", "");
            Preferences.Set("uncoveredPlanets", uncoveredPlanets += index.ToString() + " ");
        }

        public static bool hasBeenUncovered(int i)
        {
            int[] planets = getUncoveredPlanets();

            //if all planets have been uncovered returns false to prevent never ending while loop;
            if(planets.Count() == 366)
            {
                return false;
            }

            if(planets.Contains(i))
            {
                return true;
            } else
            {
                return false;
            }
            
        }

        public static int getNumberOfUncoveredPlanets()
        {
            int[] planets = getUncoveredPlanets();

            return planets.Count();
        }

        public static int[] getUncoveredPlanets()
        {
            string uncoveredPlanets = Preferences.Get("uncoveredPlanets", "");
            int[] planets = uncoveredPlanets.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

            return planets;
        }




        // PlanetIcon() static method returns an Image - all planet icons are in Resources/Images/Planets
        public static string PlanetIcon(int? selection = null)
        {
            string imagePath;
            int randomSelection;

            // Check if selection has a value
            if (selection.HasValue)
            {
                // Use the provided selection value
                imagePath = $"planet{selection.Value}.png";
            }
            else
            {
                // Select a random icon based on the amount of Planet icons available
                randomSelection = random.Next(1, 4);
                imagePath = $"planet{randomSelection}.png"; ;
            }

            return imagePath;

        }
    }
}
