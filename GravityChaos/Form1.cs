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


        // this records when the program started
        DateTime TimeRenderStart;

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

            TimeRenderStart = DateTime.Now;
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
            ImageHeight = 400;
            //ImageHeight = 300;
            ImageWidth = (int)(ImageHeight * AspectRatio);
            // create new bitmap to which our image will be printed
            Image = new Bitmap(ImageWidth, ImageHeight);
            // where to save final image
            outputFileName = "GravityChaos.png";
            // we need to generate the image
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
            // Add stationary particles that will act as targets
            //------------------------------------------------------------------
            ParticleRadius = SpaceHeight / 10.0;
            ParticleMass = Math.Pow(ParticleRadius, 2.0);
            
            // stationary particle 1
            this.Particles.Add(
                new Particle
                {
                    Color = Color.Red,
                    PositionX = SpaceWidth / 3.0,
                    PositionY = SpaceHeight / 2.0,
                    Fixed = true,
                    Mass = ParticleMass,
                    Radius = ParticleRadius
                }
            );
            // stationary particle 2
            this.Particles.Add(
                new Particle
                {
                    Color = Color.Blue,
                    PositionX = SpaceWidth * 2.0 / 3.0,
                    PositionY = SpaceHeight / 2.0,
                    Fixed = true,
                    Mass = ParticleMass,
                    Radius = ParticleRadius
                }
            );
            // stationary particle 3
            this.Particles.Add(
                new Particle
                {
                    Color = Color.Lime,
                    PositionX = SpaceWidth,
                    PositionY = SpaceHeight,
                    Fixed = true,
                    Mass = ParticleMass*2,
                    Radius = ParticleRadius
                }
            );
            // stationary particle 3
            this.Particles.Add(
                new Particle
                {
                    Color = Color.Magenta,
                    PositionX = SpaceWidth / 4,
                    PositionY = SpaceHeight / 4,
                    Fixed = true,
                    Mass = ParticleMass / 2,
                    Radius = ParticleRadius
                }
            );


            this.DoubleBuffered = true;
        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Image, 0, 0);
            Particle.Draw(Particles, e.Graphics);
            if (!imageComplete)
            {
                string PercentDone = String.Format("{0:0.00}", ((y / (double)ImageHeight) + (x / (double)(ImageHeight * ImageWidth)))*100.0 );
                this.Text = "GravityChaos: rendering image. " + PercentDone + "% complete...";
            }
                
            else
            {
                string TimeRenderTotal = String.Format("{0:0.000}", (DateTime.Now - TimeRenderStart).TotalMilliseconds / 1000.0);
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
                    int iterations = 0, iterations_max = 50;
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
                        Particle.Update(Particles, 30);
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
                        // save the image
                        Image.Save(outputFileName, System.Drawing.Imaging.ImageFormat.Png);
                        // this indicates we can quit
                        imageComplete = true;
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
