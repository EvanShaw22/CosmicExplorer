using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmicExplorer
{
    
    public class Spacecraft
    {
        Task<bool>? animation = null;
        private double size;
        private Grid box;
        public int fuelPercentage = 100;
        private Image image;
        private double speed = 0.10;

        private Point pos;
        private bool allowmoves;
        private bool moving;

        public bool IsMoving { get { return moving; } }
        public Point Position
        {
            get
            {
                return pos;
            }
        }
        private MainPage mainPage;

        //Constructor
        public Spacecraft(double size, Grid main, MainPage mainPage) {

            box = new Grid()
            {
                WidthRequest = size,
                HeightRequest = size,
                BackgroundColor = Colors.Transparent
            };

            this.mainPage = mainPage;

            allowmoves = true;
            moving = false;

            image = new Image { Source = "spacecraft.png" };

            box.Add(image);

            main.Add(box);

            box.TranslateTo(100, 100);

            pos = new Point(box.TranslationX, box.TranslationY);

        }

        public void DepleteFuel()
        {
            if (fuelPercentage > 0)
            {
                fuelPercentage = fuelPercentage - 1;
            }

        }

        public void FuelcanCollected()
        {
            fuelPercentage += 30;
        }

        public async void MoveTo(Point point)
        {
            if (allowmoves && fuelPercentage > 0)
            {
                //Disable movement until this move complete
                allowmoves = false;

                //Set moving to true for fuel consumption
                moving = true;

                //Centre the move
                /*
                point.X -= size * 20;
                point.Y -= size;*/

                //Need distance to determine time and fuel consumption
                double distance = point.Distance(new Point(box.TranslationX, box.TranslationY));

                uint time = (uint)(distance / speed);
                allowmoves = false;

                //Easing.CubicOut felt the most spacecraft-ish
                animation = box.TranslateTo(point.X - 40, point.Y - 10, time, Easing.CubicOut);

                await animation;

                allowmoves = true;
                moving = false;
                animation = null;

                pos.X = box.TranslationX; pos.Y = box.TranslationY;

            }

        }

    }
}
