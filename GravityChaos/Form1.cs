using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GravityChaos;
using System.Media;

 
namespace Form1
{
    public partial class Form1 : Form
    {
        // this is a list of all the particles we will use to make the gravity map
        private List<Particle> Particles;


        // this records when the rendering started
        DateTime TimeRenderStart;
        // this records when the rendering stopped
        DateTime TimeRenderStop;

        //------------------------------------------------------------------
        // Define bitmap size for rendering the image of gravity chaos
        //------------------------------------------------------------------
        int ImageHeight;
        int ImageWidth;
        // create new bitmap to which our image will be printed
        Bitmap Image;
        // this is the name of the file to which the final image is stored (e.g. "myImage.png")
        string outputFileName;
        // this lets us know if we need to continue updating the image
        bool imageComplete;
        // x and y keep track of which pixel we are testing.
        // The pixels of the screen are evaluated from left to right, then
        // from top to bottom, like reading a book in english.
        int x;
        int y;
        int ScreenRefreshPeriodMs;


        //------------------------------------------------------------------
        // Define space for particles
        //------------------------------------------------------------------
        double AspectRatio;
        double SpaceHeight;
        double SpaceWidth;
        // calculates the scaling factor to translate between image space to particle space
        double ImageToParticleSpace(int pixel)
        {
            return (double)(pixel * SpaceHeight / ImageHeight);
        }


        //------------------------------------------------------------------
        // Add stationary particles that will act as targets
        //------------------------------------------------------------------
        double ParticleRadius;
        double ParticleMass;



        public Form1()
        {
            InitializeComponent();
            this.Particles = new List<Particle> { };


            //------------------------------------------------------------------
            // Define space for particles
            //------------------------------------------------------------------
            AspectRatio = 16.0 / 9.0;
            SpaceHeight = 720;
            SpaceWidth = SpaceHeight * AspectRatio;


            //------------------------------------------------------------------
            // Define bitmap size for rendering the image of gravity chaos
            //------------------------------------------------------------------
            ImageHeight = 1080 / 4;

            ImageWidth = (int)(ImageHeight * AspectRatio);
            // create new bitmap to which our image will be printed
            Image = new Bitmap(ImageWidth, ImageHeight);
            // where to save final image
            outputFileName = "render";
            // we have yet to generate the image, so set this to false.
            imageComplete = false;
            // x and y keep track of which pixel we are testing.
            // The pixels of the screen are evaluated from left to right, then
            // from top to bottom, like reading a book in english.
            x = 0;
            y = 0;
            ScreenRefreshPeriodMs = 250;

            
            //------------------------------------------------------------------
            // Add one particle that will be the projectile (free to move)
            //------------------------------------------------------------------
            this.Particles.Add(new Particle { Color = Color.White, Radius = 0});


            //------------------------------------------------------------------
            // Add targets (stationary particles that the projectile may hit)
            //------------------------------------------------------------------

            // "granualr lava lamp"
            #region map1
            //ParticleRadius = SpaceHeight / 10.0;
            //ParticleMass = Math.Pow(ParticleRadius, 2.0);

            //// stationary particle 1
            //this.Particles.Add(
            //    new Particle
            //    {
            //        Color = Color.Red,
            //        PositionX = SpaceWidth / 3.0,
            //        PositionY = SpaceHeight / 2.0,
            //        Fixed = true,
            //        Mass = ParticleMass,
            //        Radius = ParticleRadius
            //    }
            //);
            //// stationary particle 2
            //this.Particles.Add(
            //    new Particle
            //    {
            //        Color = Color.Blue,
            //        PositionX = SpaceWidth * 2.0 / 3.0,
            //        PositionY = SpaceHeight / 2.0,
            //        Fixed = true,
            //        Mass = ParticleMass,
            //        Radius = ParticleRadius
            //    }
            //);
            //// stationary particle 3
            //this.Particles.Add(
            //    new Particle
            //    {
            //        Color = Color.Lime,
            //        PositionX = SpaceWidth,
            //        PositionY = SpaceHeight,
            //        Fixed = true,
            //        Mass = ParticleMass * 2,
            //        Radius = ParticleRadius
            //    }
            //);
            //// stationary particle 3
            //this.Particles.Add(
            //    new Particle
            //    {
            //        Color = Color.Magenta,
            //        PositionX = SpaceWidth / 4,
            //        PositionY = SpaceHeight / 4,
            //        Fixed = true,
            //        Mass = ParticleMass / 2,
            //        Radius = ParticleRadius
            //    }
            //);
            #endregion


            // "organized mess"
            #region map2
            //ParticleRadius = SpaceHeight / 30.0;
            //ParticleMass = Math.Pow(ParticleRadius, 2.0);

            //int i_max = 7;
            //int j_max = 5;

            //for(int i = 0; i < i_max; i++)
            //{
            //    for(int j = 0; j < j_max; j++)
            //    {
            //        this.Particles.Add(
            //            new Particle
            //            {
            //                Color = Color.FromArgb(255 * (i_max + j_max - i - j) / (i_max + j_max), 255 * i / i_max, 255 * j / j_max),
            //                PositionX = SpaceWidth * 0.5 *(i + 0.5 + (j%2)/2.0) / (double)i_max,
            //                PositionY = SpaceHeight * 0.5 * (j + 0.5) / (double)j_max,
            //                Fixed = true,
            //                Mass = ParticleMass,
            //                Radius = ParticleRadius
            //            }
            //        );
            //    }
            //}

            #endregion

            // "hexagonal grayscale"
            #region map3

            //int t_mod = 3;
            //int t_max = t_mod*2;
            //ParticleRadius = SpaceHeight / (10 * Math.Sqrt(t_max));
            //ParticleMass = Math.Pow(ParticleRadius, 2.0);
            //for (int t = 0; t < t_max; t++)
            //{
            //    int a, r, g, b;
            //    a = 255;
            //    int Level = (int)(28 + (255-28) * Math.Pow((t % t_mod) / (double)(t_mod), 2));
            //    r = Level;
            //    g = Level;
            //    b = Level;
            //    this.Particles.Add(
            //        new Particle
            //        {
            //            Color = Color.FromArgb(a, r, g, b),
            //            PositionX = 0.5 * SpaceWidth + SpaceHeight * 0.25 * Math.Cos(2 * Math.PI * t / (double)t_max),
            //            PositionY = 0.5 * SpaceHeight + SpaceHeight * 0.25 * Math.Sin(2 * Math.PI * t / (double)t_max),
            //            Fixed = true,
            //            Mass = ParticleMass,// * (1 + 10 * t / (double)t_max),
            //            Radius = ParticleRadius// * (1 + 10 * t / (double)t_max) / 10
            //        }
            //    );
            //}

            //this.Particles.Add(
            //        new Particle
            //        {
            //            Color = Color.White,
            //            PositionX = 0.5 * SpaceWidth,
            //            PositionY = 0.5 * SpaceHeight,
            //            Fixed = true,
            //            Mass = ParticleMass / 9.0,
            //            Radius = ParticleRadius * 2.0
            //        }
            //    );
            #endregion



            // "snow drift"
            #region map3

            //ParticleRadius = SpaceHeight / 30.0;
            //ParticleMass = Math.Pow(ParticleRadius, 2.0);

            //this.Particles.Add(
            //        new Particle
            //        {
            //            Color = Color.Cyan,
            //            PositionX = 0.5 * SpaceWidth,
            //            PositionY = 0.5 * SpaceHeight,
            //            Fixed = true,
            //            Mass = ParticleMass,
            //            Radius = ParticleRadius
            //        }
            //    );
            //this.Particles.Add(
            //        new Particle
            //        {
            //            Color = Color.DodgerBlue,
            //            PositionX = 0.9 * SpaceWidth,
            //            PositionY = 0.1 * SpaceHeight,
            //            Fixed = true,
            //            Mass = ParticleMass,
            //            Radius = ParticleRadius
            //        }
            //    );
            //this.Particles.Add(
            //        new Particle
            //        {
            //            Color = Color.CornflowerBlue,
            //            PositionX = 0.1 * SpaceWidth,
            //            PositionY = 0.9 * SpaceHeight,
            //            Fixed = true,
            //            Mass = ParticleMass,
            //            Radius = ParticleRadius
            //        }
            //    );
            //this.Particles.Add(
            //        new Particle
            //        {
            //            Color = Color.DeepSkyBlue,
            //            PositionX = 0.3 * SpaceWidth,
            //            PositionY = 0.3 * SpaceHeight,
            //            Fixed = true,
            //            Mass = ParticleMass,
            //            Radius = ParticleRadius
            //        }
            //    );
            //this.Particles.Add(
            //        new Particle
            //        {
            //            Color = Color.LightBlue,
            //            PositionX = 0.7 * SpaceWidth,
            //            PositionY = 0.7 * SpaceHeight,
            //            Fixed = true,
            //            Mass = ParticleMass,
            //            Radius = ParticleRadius
            //        }
            //    );

            // ""
            #endregion

            #region map3

            int t_mod = 12;
            int t_max = t_mod * 1;
            ParticleRadius = SpaceHeight / (100 * Math.Sqrt(t_max));
            ParticleMass = 50;
            for (int t = 0; t < t_max; t++)
            {
                int a, r, g, b;
                a = 255;
                int Level = (int)(28 + (255 - 28) * Math.Pow((t % t_mod) / (double)(t_mod), 2));
                r = 90;
                g = 255 - (Level);
                b = Level;
                this.Particles.Add(
                    new Particle
                    {
                        Color = Color.FromArgb(a, r, g, b),
                        PositionX = 0.5 * SpaceWidth - SpaceHeight * 0.25 * (t / (double)t_max) * Math.Cos(3 * Math.PI * t / (double)t_max),
                        PositionY = 0.3 * SpaceHeight + SpaceHeight * 0.25 * (t / (double)t_max) * Math.Sin(3 * Math.PI * t / (double)t_max),
                        Fixed = true,
                        Mass = ParticleMass,// * (1 + 10 * t / (double)t_max),
                        Radius = ParticleRadius * Math.Sqrt(t + 1)
                    }
                );
            }
            #endregion


            this.DoubleBuffered = true;
            TimeRenderStart = DateTime.Now;
        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Image, 0, 0);
            //Particle.Draw(Particles, e.Graphics);
            if (!imageComplete)
            {
                string PercentDone = String.Format("{0:0.00}", ((y / (double)ImageHeight) + (x / (double)(ImageHeight * ImageWidth)))*100.0 );
                this.Text = "GravityChaos: rendering image. " + PercentDone + "% complete...";
            }
            else
            {
                string TimeRenderTotal = String.Format("{0:0.000}", (TimeRenderStop - TimeRenderStart).TotalMilliseconds / 1000.0);
                this.Text = "GravityChaos: Complete! Total Render Time = " + TimeRenderTotal + " seconds";
            }
                
        }


        // this timer happens every millisecond. This is my workaround to
        // always get control back to the map-generating routine.
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!imageComplete)
            {
                // record when you entered the loop
                DateTime TimeOfEntry = DateTime.Now;
                int ElapsedTimeMs;
                // work on the map
                do
                {
                    // start the particle out at the current <x, y> coordinates
                    Particles[0].PositionX = ImageToParticleSpace(x);
                    Particles[0].PositionY = ImageToParticleSpace(y);
                    // set the moving particle's initial velocity to 0.
                    Particles[0].VelocityX = 0;
                    Particles[0].VelocityY = 0;

                    // run the simulation until the moving particle hits one of the stationary particles
                    // have a timeout to prevent the programming from going in an endless loop
                    bool collision = false;
                    int iterations = 0, iterations_max = 10;
                    while ((!collision) && (iterations < iterations_max))
                    {
                        // check to see if the moving particle has collided with any of the others
                        foreach (Particle p in Particles.GetRange(1, Particles.Count - 1))
                        {
                            collision = Particle.CollisionCheck(Particles[0], p);
                            // if the particle collided, set the pixel to the appropriate color.
                            if (collision)
                            {
                                Image.SetPixel(x, y, p.Color);
                                break;
                            }
                        }
                        // run the simulation for a little while
                        Particle.UpdateSingle(Particles[0], Particles.GetRange(1, Particles.Count - 1), 50);


                        iterations++;
                    }
                    // depending on which target our moving particle hits, color the <x, y> pixel accordingly
                    if (!collision)
                    {
                        Image.SetPixel(x, y, Color.Black);
                    }

                    // increment x and y
                    x++;
                    // if you have gotten to the right edge of the bitmap,
                    if (x >= ImageWidth)
                    {
                        x = 0;
                        y++;
                    }
                    // if you have completed the image, close it.
                    if (y >= ImageHeight)
                    {
                        // save the image with date
                        Image.Save(outputFileName + "_" + (DateTime.Now).ToString("yyyy-MM-ddTHH.mm.ss") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                        // this indicates we can stop rendering.
                        imageComplete = true;
                        TimeRenderStop = DateTime.Now;
                        try
                        {
                            SoundPlayer player = new SoundPlayer("gotmail.wav");
                            player.Play();
                        }
                        catch
                        {
                            // oh well.
                        }
                        
                    }
                    ElapsedTimeMs = (int)(DateTime.Now - TimeOfEntry).TotalMilliseconds;
                }
                // keep working on the map until you need to refresh the screen again
                while ((ElapsedTimeMs < this.ScreenRefreshPeriodMs) && !imageComplete);

                Invalidate();
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
