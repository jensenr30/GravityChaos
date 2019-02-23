﻿using System;
using System.IO;
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

        // this describes the initial starting velocity of the test particle.
        // Normally, these will both be set to 0, but if you choose to set them
        // to a value other than 0, the rendered image will have a kind of
        // directional offset/bias in how it is rendered due to the change in
        // initial conditions of the test particle.
        double TargetDefaultVelocityX;
        double TargetDefaultVelocityY;
        // total rotation angle (radians  example: 2*Math.PI for a full circle, -Math.PI for a half-circle backwards)
        double RotationAngle;
        // total rotation sample points (exmple: 36 for a point every 10 degrees)
        int RotationSamplePoints;
        // initial velocity amplitude (example: 1 to start with a velocity magnitude of 1)
        double RotationVelocityAmplitude;
        // initial velocity phase (example: Math.PI/2 to start at 90 degrees (straight down))
        double RotationVelocityInitialPhase;
        // keeps track of the current image being generated
        int ImageNumber;

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
        // default color
        Color ImageDefaultColor;
        // this is the directory where the output render image file will be saved
        string OutputFileDirectory;
        // this is the name of the file to which the final image is stored (e.g. "myImage.png")
        string OutputFileName;
        // this lets us know if we need to continue updating the image
        bool ImageComplete;
        // x and y keep track of which pixel we are testing.
        // The pixels of the screen are evaluated from left to right, then
        // from top to bottom, like reading a book in english.
        int x;
        int y;
        int ScreenRefreshPeriodMs;
        // this is how much time taken per simulation iteration
        // (see GravityChaos.GravityChaosRenderer class comments)
        double IterationTimeStep;
        // this is how many iterations the test particle goes through before
        // abandoning the test (see GravityChaos.GravityChaosRenderer class comments)
        int IterationsMax;

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
        //double ParticleRadius;
        //double ParticleMass;



        public Form1()
        {
            InitializeComponent();
            this.Particles = new List<Particle> { };

            TargetDefaultVelocityX = 0;
            TargetDefaultVelocityY = 0;
            RotationAngle = Math.PI * 2;
            RotationSamplePoints = 360;
            RotationVelocityAmplitude = .07;
            RotationVelocityInitialPhase = 0;
            ImageNumber = 0;

            // prevent the last rendered image from being the same as the first rendered image
            // (this is good for when you are trying to make cyclic animations)
            RotationAngle = RotationAngle * (RotationSamplePoints - 1) / Convert.ToDouble(RotationSamplePoints);


            //------------------------------------------------------------------
            // Define space for particles
            //------------------------------------------------------------------
            AspectRatio = 16.0 / 9.0;
            SpaceHeight = 1;
            SpaceWidth = SpaceHeight * AspectRatio;


            //------------------------------------------------------------------
            // define time parameters to be used by the gravity chaos renderer
            //------------------------------------------------------------------
            IterationTimeStep = 2;
            IterationsMax = 7;


            //------------------------------------------------------------------
            // Define bitmap size for rendering the image of gravity chaos
            //------------------------------------------------------------------
            // size of the image
            ImageHeight = 1080 / 1;
            ImageWidth = (int)(ImageHeight * AspectRatio);
            // create new bitmap to which our image will be printed
            Image = new Bitmap(ImageWidth, ImageHeight);
            // background color of the rendered image
            ImageDefaultColor = Color.Black;
            // folder in which to save the final image
            OutputFileDirectory = "renders";
            OutputFileDirectory = PathAddBackslash(OutputFileDirectory);
            // the name of the final image
            OutputFileName = "render";
            // we have yet to generate the image, so set this to false.
            ImageComplete = false;
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
            #region map1 - granualr lava lamp
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
            #region map2 - organized mess
            //double ParticleRadius = SpaceHeight / 30.0;
            //double ParticleMass = Math.Pow(ParticleRadius, 2.0);

            //int i_max = 7;
            //int j_max = 5;

            //for (int i = 0; i < i_max; i++)
            //{
            //    for (int j = 0; j < j_max; j++)
            //    {
            //        this.Particles.Add(
            //            new Particle
            //            {
            //                Color = Color.FromArgb(255 * (i_max + j_max - i - j) / (i_max + j_max), 255 * i / i_max, 255 * j / j_max),
            //                PositionX = SpaceWidth * 0.5 * (i + 0.5 + (j % 2) / 2.0) / (double)i_max,
            //                PositionY = SpaceHeight * 0.5 * (j + 0.5) / (double)j_max,
            //                Fixed = true,
            //                Mass = ParticleMass,
            //                Radius = ParticleRadius
            //            }
            //        );
            //    }
            //}

            #endregion
            #region map3 - hexagonal grayscale

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
            #region map4 - snow drift

            double ParticleRadius = SpaceHeight / 3000.0;
            double ParticleMass = 1/900.0;

            this.Particles.Add(
                new Particle
                {
                    Color = Color.Cyan,
                    PositionX = 0.5 * SpaceWidth,
                    PositionY = 0.5 * SpaceHeight,
                    Fixed = true,
                    Mass = ParticleMass,
                    Radius = ParticleRadius
                }
            );
            this.Particles.Add(
                new Particle
                {
                    Color = Color.DodgerBlue,
                    PositionX = 0.9 * SpaceWidth,
                    PositionY = 0.1 * SpaceHeight,
                    Fixed = true,
                    Mass = ParticleMass,
                    Radius = ParticleRadius
                }
            );
            this.Particles.Add(
                new Particle
                {
                    Color = Color.CornflowerBlue,
                    PositionX = 0.1 * SpaceWidth,
                    PositionY = 0.9 * SpaceHeight,
                    Fixed = true,
                    Mass = ParticleMass,
                    Radius = ParticleRadius
                }
            );
            this.Particles.Add(
                new Particle
                {
                    Color = Color.DeepSkyBlue,
                    PositionX = 0.3 * SpaceWidth,
                    PositionY = 0.3 * SpaceHeight,
                    Fixed = true,
                    Mass = ParticleMass,
                    Radius = ParticleRadius
                }
            );
            this.Particles.Add(
                new Particle
                {
                    Color = Color.LightBlue,
                    PositionX = 0.7 * SpaceWidth,
                    PositionY = 0.7 * SpaceHeight,
                    Fixed = true,
                    Mass = ParticleMass,
                    Radius = ParticleRadius
                }
            );

            #endregion
            #region map5 - test

            //int t_mod = 12;
            //int t_max = t_mod * 1;
            //ParticleRadius = SpaceHeight / (100 * Math.Sqrt(t_max));
            //ParticleMass = 50;
            //for (int t = 0; t < t_max; t++)
            //{
            //    int a, r, g, b;
            //    a = 255;
            //    int Level = (int)(28 + (255 - 28) * Math.Pow((t % t_mod) / (double)(t_mod), 2));
            //    r = 90;
            //    g = 255 - (Level);
            //    b = Level;
            //    this.Particles.Add(
            //        new Particle
            //        {
            //            Color = Color.FromArgb(a, r, g, b),
            //            PositionX = 0.5 * SpaceWidth - SpaceHeight * 0.25 * (t / (double)t_max) * Math.Cos(3 * Math.PI * t / (double)t_max),
            //            PositionY = 0.3 * SpaceHeight + SpaceHeight * 0.25 * (t / (double)t_max) * Math.Sin(3 * Math.PI * t / (double)t_max),
            //            Fixed = true,
            //            Mass = ParticleMass,// * (1 + 10 * t / (double)t_max),
            //            Radius = ParticleRadius * Math.Sqrt(t + 1)
            //        }
            //    );
            //}
            #endregion

            #region map6 - used for testing color gradient calculation feature

            //double ParticleRadius = SpaceHeight / 5;
            //double ParticleMass = .2;

            //this.Particles.Add(
            //    new Particle
            //    {
            //        Color = Color.FromArgb(128, 255, 0, 0),
            //        PositionX = SpaceWidth * 1,
            //        PositionY = SpaceHeight * 1 + ParticleRadius,
            //        Fixed = true,
            //        Mass = ParticleMass * 1,
            //        Radius = ParticleRadius
            //    }
            //);
            //this.Particles.Add(
            //    new Particle
            //    {
            //        Color = Color.FromArgb(255, 0, 0, 255),
            //        PositionX = SpaceWidth * 0,
            //        PositionY = SpaceHeight * 1 + ParticleRadius,
            //        Fixed = true,
            //        Mass = ParticleMass * 2,
            //        Radius = ParticleRadius
            //    }
            //);
            //this.Particles.Add(
            //    new Particle
            //    {
            //        Color = Color.FromArgb(255, 0, 255, 0),
            //        PositionX = SpaceWidth / 2,
            //        PositionY = SpaceHeight * 0 - ParticleRadius,
            //        Fixed = true,
            //        Mass = ParticleMass * Math.PI,
            //        Radius = ParticleRadius
            //    }
            //);


            #endregion


            this.DoubleBuffered = true;
            TimeRenderStart = DateTime.Now;
        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Image, 0, 0);
            //Particle.Draw(Particles, e.Graphics);
            if (!ImageComplete)
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
            if (!ImageComplete)
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
                    // set the moving particle's initial velocity.
                    Particles[0].VelocityX = RotationVelocityAmplitude * Math.Cos(RotationVelocityInitialPhase + RotationAngle * ImageNumber / Math.Max(1,Convert.ToDouble(RotationSamplePoints - 1)));
                    Particles[0].VelocityY = RotationVelocityAmplitude * Math.Sin(RotationVelocityInitialPhase + RotationAngle * ImageNumber / Math.Max(1,Convert.ToDouble(RotationSamplePoints - 1)));

                    // run the simulation until the moving particle hits one of the stationary particles
                    // have a timeout to prevent the programming from going in an endless loop
                    bool collision = false;
                    int iterations = 0;
                    while ((!collision) && (iterations < IterationsMax))
                    {
                        // check to see if the moving particle has collided with any of the others
                        foreach (Particle p in Particles.GetRange(1, Particles.Count - 1))
                        {
                            collision = Particle.CollisionCheck(Particles[0], p);
                            // if the particle collided, set the pixel to the appropriate color.
                            if (collision)
                            {
                                //Image.SetPixel(x, y, p.Color);
                                Image.SetPixel(x, y, ImageDefaultColor);
                                break;
                            }
                        }
                        // run the simulation for a little while
                        Particle.UpdateSingle(Particles[0], Particles.GetRange(1, Particles.Count - 1), IterationTimeStep);


                        iterations++;
                    }
                    // if our test particle did not collide with ANY target particles,
                    if (!collision)
                    {
                        // calculate the blended color of all particles
                        double ColorProA = 1;
                        double ColorSumR = 0;
                        double ColorSumG = 0;
                        double ColorSumB = 0;

                        foreach (Particle p in Particles.GetRange(1, Particles.Count - 1))
                        {
                            double dist_squared =
                                (Particles[0].PositionX - p.PositionX) * (Particles[0].PositionX - p.PositionX) +
                                (Particles[0].PositionY - p.PositionY) * (Particles[0].PositionY - p.PositionY);
                            double coloration_constant = 0.0001;
                            ColorProA *= (0xff - p.Color.A) / 0xff;
                            ColorSumR += Math.Pow(p.Color.R, 2) * (p.Color.A / 0xff) * coloration_constant / dist_squared;
                            ColorSumG += Math.Pow(p.Color.G, 2) * (p.Color.A / 0xff) * coloration_constant / dist_squared;
                            ColorSumB += Math.Pow(p.Color.B, 2) * (p.Color.A / 0xff) * coloration_constant / dist_squared;
                        }
                        ColorProA = (1 - ColorProA) * 0xff;
                        double ColorMax;
                        ColorMax = Math.Max(ColorSumR, ColorSumG);
                        ColorMax = Math.Max(ColorSumB, ColorMax);

                        // limit the maximum color intensity
                        if (ColorMax > 0xff)
                        {
                            ColorSumR *= 0xff / ColorMax;
                            ColorSumG *= 0xff / ColorMax;
                            ColorSumB *= 0xff / ColorMax;
                        }

                        // convert the color components to integers
                        int intR = Convert.ToInt32(ColorSumR);
                        int intG = Convert.ToInt32(ColorSumG);
                        int intB = Convert.ToInt32(ColorSumB);

                        Image.SetPixel(x, y, Color.FromArgb(0xff, intR, intG, intB));
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
                        // make a directory if it doesn't exist
                        Directory.CreateDirectory(OutputFileDirectory);
                        // save the image with date
                        Image.Save(OutputFileDirectory + OutputFileName + "_" + (DateTime.Now).ToString("yyyy-MM-ddTHH.mm.ss") + "_" + ImageNumber.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);

                        
                        y = 0;
                        //try
                        //{
                        //    SoundPlayer player = new SoundPlayer("gotmail.wav");
                        //    player.Play();
                        //}
                        //catch
                        //{
                        //    // oh well.
                        //}
                        ImageNumber++;
                        if (ImageNumber >= RotationSamplePoints)
                        {
                            ImageComplete = true;
                            TimeRenderStop = DateTime.Now;
                        }
                    }
                    ElapsedTimeMs = (int)(DateTime.Now - TimeOfEntry).TotalMilliseconds;
                }
                // keep working on the map until you need to refresh the screen again
                while ((ElapsedTimeMs < this.ScreenRefreshPeriodMs) && !ImageComplete);

                Invalidate();
            }
        }


        private string PathAddBackslash(string path)
        {
            // They're always one character but EndsWith is shorter than
            // array style access to last path character. Change this
            // if performance are a (measured) issue.
            string separator1 = "\\";
            string separator2 = "/";

            // Trailing white spaces are always ignored but folders may have
            // leading spaces. It's unusual but it may happen. If it's an issue
            // then just replace TrimEnd() with Trim(). Tnx Paul Groke to point this out.
            path = path.TrimEnd();

            // Argument is always a directory name then if there is one
            // of allowed separators then I have nothing to do.
            if (path.EndsWith(separator1) || path.EndsWith(separator2))
                return path;

            // If there is the "alt" separator then I add a trailing one.
            // Note that URI format (file://drive:\path\filename.ext) is
            // not supported in most .NET I/O functions then we don't support it
            // here too. If you have to then simply revert this check:
            // if (path.Contains(separator1))
            //     return path + separator1;
            //
            // return path + separator2;
            if (path.Contains(separator2))
                return path + separator2;

            // If there is not an "alt" separator I add a "normal" one.
            // It means path may be with normal one or it has not any separator
            // (for example if it's just a directory name). In this case I
            // default to normal as users expect.
            return path + separator1;
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
